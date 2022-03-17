using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterMovement : NetworkBehaviour
{
    //Variables
    #region Variables
    //Reference to Character Controller
    CharacterController2D controller;

    //Reference to Animator
    public Animator animator;

    //Movement
    float horizontalMove = 0f;
    public float runSpeed = 40f;

    //Jumping
    bool jump = false;

    //Crouching
    bool crouch = false;
    #endregion

    //START AND UPDATES
    #region Start and Updates
    //Start is called once at the beginning of the program
    private void Start()
    {
        if (IsLocalPlayer)
        {
            controller = GetComponent<CharacterController2D>();
        }



       // GameObject.Find("TimerBG").SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
            getMove();
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            //Move character
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }

    }
    #endregion

    //METHODS
    #region Methods

    //MovementInputs collected
    void getMove()
    {
        horizontalMove = CrossPlatformInputManager.GetAxisRaw("Horizontal") * runSpeed;
        //in case of errors - was Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); //Sets Animator Speed variable to current horizontal Move speed (and changes move speed to positive if negative

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("Jump", true);
            animator.SetTrigger("Jump");
        }


        if (Input.GetButtonDown("Crouch"))
            crouch = true;
        else if (Input.GetButtonUp("Crouch"))
            crouch = false;
    }


    public void OnLanding()
    {
        animator.SetBool("Jump", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("Crouch", isCrouching);
    }

    #endregion
}
