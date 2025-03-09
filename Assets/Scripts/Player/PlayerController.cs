using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraTransform; 
    public Transform cameraPivot; 
    public float minXLook = -20f;
    public float maxXLook = 60f;
    private float camCurXRot;
    private float camCurYRot;
    public float lookSensitivity = 1f;
    public float cameraDistance = 4f; 
    public float cameraHeight = 2f; 
    public float cameraSmoothSpeed = 5f; 

    private Vector2 mouseDelta;
    private Rigidbody rigidbody;

    [HideInInspector]
    public bool canLook = true;
    public bool canToggleInventory = true;

    public Action inventory;

    public UIInventory uiInventory;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (uiInventory != null)
        {
            inventory = null;
            inventory = uiInventory.Toggle; 
            Debug.Log("UIInventory 연결됨"); 
        }
        else
        {
            Debug.LogError("UIInventory 연결 안됨");
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }


    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y;

        rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        if (cameraTransform == null || cameraPivot == null) return;

        
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);

        Vector3 targetPosition = cameraPivot.position - transform.forward * cameraDistance + Vector3.up * cameraHeight;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * cameraSmoothSpeed);

        cameraTransform.LookAt(cameraPivot);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayerMask);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Performed && canToggleInventory)
        {
            inventory?.Invoke();

            canToggleInventory = false;
            StartCoroutine(EnableInventoryToggle());
        }
    }

    private IEnumerator EnableInventoryToggle()
    {
        yield return new WaitForSeconds(0.2f); // 0.2초 후 다시 입력 가능
        canToggleInventory = true;
    }
}
