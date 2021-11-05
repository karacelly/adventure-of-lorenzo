using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public CharacterController controller;
    Animator animator;
    public Transform cam;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    
    Vector3 velocity;
    float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = 0;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.inDialogue)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            animator.SetFloat("InputX", horizontal);
            animator.SetFloat("InputY", vertical);

            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }

            velocity.y -= 9.81f * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            //if (PlayerController.shooterMode)
            //{
            //    var mouseX = Input.GetAxis("Mouse X");
            //    transform.Rotate(new Vector3(0, mouseX, 0));
            //}

            if (direction.magnitude >= 0.1f)
            {
                animator.SetBool("isWalking", true);
                

                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                //if (!PlayerController.shooterMode)
                //{
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                //}

                Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
                moveDirection = moveDirection.x * cam.right.normalized + moveDirection.z * cam.forward.normalized;

                controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
}
