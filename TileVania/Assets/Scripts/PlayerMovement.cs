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

    Vector2 moveInput;
    Rigidbody2D myRigibody;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;

    float gravityScaleAtStart;
    // Start is called before the first frame update
    void Start()
    {
        myRigibody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();

        gravityScaleAtStart = myRigibody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipsSprite();
        ClimbLadder();
    }



    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){ return;}
        if (value.isPressed)
        {
            //do stuff
            
            myRigibody.velocity += new Vector2(0, jumpSpeed);
        }
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
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){ 
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
}
