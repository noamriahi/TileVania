using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Vector2 moveInput;
    Rigidbody2D myRigibody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetColider;
    bool isAlive = true;

    float gravityScaleAtStart;
    // Start is called before the first frame update
    void Start()
    {
        myRigibody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetColider = GetComponent<BoxCollider2D>();

        gravityScaleAtStart = myRigibody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;
        Run();
        FlipsSprite();
        ClimbLadder();
        Die();


    }

    void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (!isAlive) return;
        if (!myFeetColider.IsTouchingLayers(LayerMask.GetMask("Ground"))){ return;}
        if (value.isPressed)
        {
            //do stuff
            
            myRigibody.velocity += new Vector2(0, jumpSpeed);
        }
    }
    void OnFire(InputValue value)
    {
        if (!isAlive) return;
        Instantiate(bullet, gun.position,transform.rotation);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigibody.velocity.y);
        myRigibody.velocity = playerVelocity;

        bool playerHasHorizontalSpeeed = Mathf.Abs(myRigibody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeeed)
            myAnimator.SetBool("isRunning", true);
        else
            myAnimator.SetBool("isRunning", false);
    }
    private void FlipsSprite()
    {
        bool playerHasHorizontalSpeeed = Mathf.Abs(myRigibody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigibody.velocity.x), 1f);
        }
    }
    void ClimbLadder()
    {
        if (!myFeetColider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){ 
            myRigibody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return; 
        }
        
        Vector2 climbVelocity = new Vector2(myRigibody.velocity.x, moveInput.y * climbSpeed);
        myRigibody.velocity = climbVelocity;
        myRigibody.gravityScale = 0;
        bool playerHasVericalSpeeed = Mathf.Abs(myRigibody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVericalSpeeed);
    }
    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigibody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
