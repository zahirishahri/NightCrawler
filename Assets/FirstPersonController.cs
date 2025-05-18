using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Camera Look")]
    public float lookSpeed = 2f;
    public Transform playerCamera;
    float cameraPitch = 0f;

    [Header("Crouch")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    private bool isCrouching = false;

    [Header("Camera Bob")]
    public bool isRecording = false;
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    private float bobTimer = 0f;
    private Vector3 originalCameraPosition;

    [Header("Camera Collision")]
    public float cameraDistance = 0.5f;
    public LayerMask collisionMask;
    private Vector3 desiredCameraLocalPos;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        originalCameraPosition = playerCamera.localPosition;
        desiredCameraLocalPos = originalCameraPosition;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Move();
        Look();
        HandleCrouch();
        HandleJump();
        ApplyGravity();
        HandleCameraBob();

    }

    void Move()
    {
        float x = Input.GetAxis ("Horizontal");
        float z = Input.GetAxis ("Vertical");

        float speed = isCrouching ? crouchSpeed : moveSpeed;
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : standingHeight;

            Vector3 center = controller.center;
            center.y = controller.height / 2f;
            controller.center = center;
        }
    }

    void HandleCameraBob()
    {
        if (isRecording && controller.velocity.magnitude > 0.1f && isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
            Vector3 newPos = originalCameraPosition + new Vector3(0f, bobOffset, 0f);
            playerCamera.localPosition = newPos;
        }
        else
        {
            //Reset camera to original position smoothly
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, originalCameraPosition, Time.deltaTime * 8f);
            bobTimer = 0f; // Reset bob timer when not moving
        }
        HandleCameraCollision();
    }

    void HandleCameraCollision()
    {
        Vector3 start = transform.position + Vector3.up * controller.height * 0.75f;
        Vector3 dir = playerCamera.position - start;
        float distance = cameraDistance;

        if (Physics.SphereCast(start, 0.1f, dir.normalized, out RaycastHit hit, cameraDistance, collisionMask))
        {
            distance = hit.distance - 0.05f;
        }

        Vector3 targetLocalPos = originalCameraPosition;
        targetLocalPos.z = -distance;
        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetLocalPos, Time.deltaTime * 10f);

    }

    //Call this method from another script to tooglle recording
    public void SetRecording(bool recording)
    {
        isRecording = recording;

    }
}