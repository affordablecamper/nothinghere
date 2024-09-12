using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    public float leanAngle = 30f;

    public float headBobFrequency = 5f;
    public float headBobAmount = 0.05f;

    private Rigidbody rb;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool isSprinting = false;
    private float originalHeight;
    private Vector3 originalCenter;

    private float headBobTimer = 0f;
    private Vector3 originalCameraPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalHeight = GetComponent<CapsuleCollider>().height;
        originalCenter = GetComponent<CapsuleCollider>().center;

        originalCameraPosition = Camera.main.transform.localPosition;
    }

    private void Update()
    {
        // Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * currentSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        // Rotation
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching)
            {
                StandUp();
            }
            else
            {
                Crouch();
            }
        }

        // Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

        // Leaning
        float leanInput = Input.GetAxis("Lean");

        if (leanInput > 0f)
        {
            LeanRight();
        }
        else if (leanInput < 0f)
        {
            LeanLeft();
        }
        else
        {
            ResetLean();
        }

        // Headbob
        if (verticalInput != 0f && !isCrouching && !isJumping)
        {
            float headBobSpeed = isSprinting ? headBobFrequency * 2f : headBobFrequency;
            Camera.main.transform.localPosition = originalCameraPosition + CalculateHeadBobOffset(headBobSpeed);
        }
        else
        {
            Camera.main.transform.localPosition = originalCameraPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Map"))
        {
            isJumping = false;
        }
    }

    private void Crouch()
    {
        GetComponent<CapsuleCollider>().height = originalHeight / 2f;
        GetComponent<CapsuleCollider>().center = originalCenter / 2f;
        isCrouching = true;
    }

    private void StandUp()
    {
        GetComponent<CapsuleCollider>().height = originalHeight;
        GetComponent<CapsuleCollider>().center = originalCenter;
        isCrouching = false;
    }

    private void LeanLeft()
    {
        transform.localRotation = Quaternion.Euler(0f, -leanAngle, 0f);
    }

    private void LeanRight()
    {
        transform.localRotation = Quaternion.Euler(0f, leanAngle, 0f);
    }

    private void ResetLean()
    {
        transform.localRotation = Quaternion.identity;
    }

    private Vector3 CalculateHeadBobOffset(float speed)
    {
        float xOffset = Mathf.Sin(headBobTimer * speed) * headBobAmount;
        float yOffset = Mathf.Cos(headBobTimer * speed * 2f) * headBobAmount * 0.5f;

        headBobTimer += Time.deltaTime;

        return new Vector3(xOffset, yOffset, 0f);
    }
}