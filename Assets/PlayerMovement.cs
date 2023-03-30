using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float mouseSensitivity = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private Camera playerCamera;
    private float cameraRotation = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection += transform.forward * moveZ * speed * Time.deltaTime;
        moveDirection += transform.right * moveX * speed * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
        moveDirection = Vector3.zero;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        cameraRotation -= mouseY;
        cameraRotation = Mathf.Clamp(cameraRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
