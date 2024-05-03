using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 2.5f;
    private float crouchTimer = 1;
    private bool crouching = false;
    private bool lerpCrouch = false;
    private bool sprinting = false;
    private bool doubleJump = true;
    private bool quickCrouch = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (sprinting)
        {
            lerpCrouch = true;
            quickCrouch = true;
        }
        else
        {
            lerpCrouch = true;
            quickCrouch = false;
        }
        if (lerpCrouch) 
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching)
            {
                if (quickCrouch)
                {
                    speed = Mathf.Lerp(speed, 3.75f, p);
                } else
                {
                    speed = Mathf.Lerp(speed, 2.5f, p);
                }
                controller.height = Mathf.Lerp(controller.height, 1, p);
            } else
            {
                if (sprinting) {
                    speed = Mathf.Lerp(speed, 10f, p);
                } else
                {
                    speed = Mathf.Lerp(speed, 5f, p);
                }
                controller.height = Mathf.Lerp(controller.height, 2, p);
            }
            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    // Recieves the input from InputManager.cs and applies to character controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded) {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
            doubleJump = true;
        } else if (doubleJump)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
            doubleJump = false;
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
        {
            if (!crouching)
            {
                speed = 10;
            }
        }
        else
        {
            if (!crouching)
            {
                speed = 5;
            } 
        }
    }
}
