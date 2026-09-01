using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class CustomSimplePlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 1.15f;
    public float runSpeed = 4.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;
    public float gravity = 1500.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    private bool canMove = true;

    private bool isCrouching = false;
    private float crouchHeight = 1.0f;
    private Vector3 originalPosition;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCrouching)
            {
                // Debug.Log(isCrouching);
                isCrouching = false;
                StandUp();
            }
            else
            {
                // Debug.Log(isCrouching);
                isCrouching = true;
                Crouch();
            }
        }

        if (isCrouching)
        {
            isRunning = false;
        }

        // Debug.Log(this.transform.position);

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void Crouch()
    {
        Vector3 newPosition = originalPosition - new Vector3(0, crouchHeight, 0);
        playerCamera.transform.localPosition = newPosition;
    }

    void StandUp()
    {
        playerCamera.transform.localPosition = originalPosition;
    }

    public void SwitchCanMove(bool state)
    {
        canMove = state;
    }
    //애니메이션 시작하면 끄고, 애니메이션 시작하면 키기

    public IEnumerator EnablePlayerMovementAfterDelay(float delay)
    {
        SwitchCanMove(false);
        yield return new WaitForSeconds(delay);
        SwitchCanMove(true); // Player 움직임 활성화
    }

    public void SetPlayerViewRocation(float sideRocation, float upDownRocation)
    {
        transform.rotation = Quaternion.Euler(0f, sideRocation, 0f);
        playerCamera.transform.rotation = Quaternion.Euler(upDownRocation, sideRocation, 0f);
    }
}