using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Outside Sources")]
    public CharacterController controller;
    public GameObject ballHolder;
    public GameObject ballAimer;
    public GameObject currentBall;
    public GameObject fpsCamera;
    public TMP_Text interact;
    public Slider staminaBar;

    [Header("Stats")]
    public float walkSpeed = 10;
    public float runSpeed = 14;
    public float speedStaminaUsage = .25f;
    public float gravity = -9.8f;
    public float jumpForce = 3f;
    public float dashSpeed;
    public float dashTime;
    public float dashStaminaUsage = 200;
    Coroutine rechargeStamina;
    public int maxStamina = 100;
    public float currentStamina;
    public float regenWaitTime = 2;
    bool holdingBall;
    public float noAimThrowForce = 100;
    public float aimThrowForce = 200;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Velocity")]
    Vector3 velocity;

    private void Start()
    {
        ballHolder = GameObject.Find("Player Ball Holder");
        ballAimer = GameObject.Find("Player Ball Aimer");
        holdingBall = false;
        fpsCamera = GameObject.Find("Main Camera");
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
    }

    void Update()
    {
        int fov = ((int)fpsCamera.GetComponent<Camera>().fieldOfView);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Checks if grounded
        if (isGrounded && velocity.y < 0) //Resets Velocity
        {
            velocity.y = -2f;
        }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

        staminaBar.value = currentStamina;

        if (Input.GetButton("Run") && (currentStamina >= 0))
        {
            
            controller.Move(move * runSpeed * Time.deltaTime); //Handles ground and air movement
            if (fov != 70)
            {
                fpsCamera.GetComponent<Camera>().fieldOfView += 1;
            }
            currentStamina -= speedStaminaUsage;

            if (rechargeStamina != null) StopCoroutine(rechargeStamina);
            rechargeStamina = StartCoroutine(RegenStamina());
        }
        else
        {
            controller.Move(move * walkSpeed * Time.deltaTime); //Handles ground and air movement
            if (fov != 60)
            {
                fpsCamera.GetComponent<Camera>().fieldOfView -= 1;
            }
        }

        if (Input.GetButtonDown("Dash"))
        {
            StartCoroutine(Dash(move));
        }

            Jump();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(currentBall != null)
        {
            currentBall.GetComponent<BallStatus>().ChangeStateToFalse(gameObject);
        }

        if (holdingBall) //Checks to see if player is holding a ball
        {
            HoldingBall(); 
        }
    }

    public void Jump()
    {

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
            }
        }
    }

    public IEnumerator Dash(Vector3 move)
    {
        if (currentStamina >= dashStaminaUsage)
        {
            currentStamina -= dashStaminaUsage;
            float startTime = Time.time;
            while (Time.time < startTime + dashTime)
            {
                controller.Move(move * dashSpeed * Time.deltaTime);

                yield return null;
            }

            if (rechargeStamina != null) StopCoroutine(rechargeStamina);
            rechargeStamina = StartCoroutine(RegenStamina());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Throwable")
        {
            if (other.GetComponent<BallStatus>().canPickUp && currentBall == null)
            {
                interact.text = "Press X to pick up";
                if (Input.GetButton("Pick Up") && currentBall == null)
                {
                    holdingBall = true;
                    currentBall = other.gameObject;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if(other.gameObject.tag == "Throwable")
        {
            interact.text = "";
        }
    }

    void HoldingBall()
    {
        float aiming = Input.GetAxis("Aim");
        float throwing = Input.GetAxis("Throw");
        currentBall.GetComponent<BallStatus>().canPickUp = false;
        interact.text = "";

        if (aiming == 0) //Determines if player is aiming
        {
            if (throwing == 0)
            {
                currentBall.transform.position = ballHolder.transform.position;
                currentBall.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                currentBall.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * noAimThrowForce);
                holdingBall = false;
                currentBall.GetComponent<Rigidbody>().useGravity = true;
                currentBall.GetComponent<BallStatus>().canHit = true;
                currentBall.GetComponent<BallStatus>().hitPlayerTeam = false;
                currentBall.GetComponent<BallStatus>().CallChangeStateToTrue();
                currentBall = null;
            }
        }
        else
        {
            if (throwing == 0)
            {
                currentBall.transform.position = ballAimer.transform.position;
                currentBall.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                currentBall.GetComponent<Rigidbody>().AddForce(fpsCamera.transform.forward * aimThrowForce);
                holdingBall = false;
                currentBall.GetComponent<Rigidbody>().useGravity = true;
                currentBall.GetComponent<BallStatus>().canHit = true;
                currentBall.GetComponent<BallStatus>().hitPlayerTeam = false;
                currentBall.GetComponent<BallStatus>().CallChangeStateToTrue();
                currentBall = null;
            }
        }
    }

    IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(regenWaitTime);
        while(currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.value = currentStamina;
            yield return new WaitForSeconds(.05f);
        }
    }
}