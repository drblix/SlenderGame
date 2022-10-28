using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput pInput;
    private CharacterController cc;
    private Rigidbody rb;

    private Transform mainCam;
    private Transform checkObj;

    private float xRot = 0f;

    private const float MOUSE_SENSITIVITY = 30f;
    private const float MOVEMENT_SPEED = 3f;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        pInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        mainCam = transform.Find("MainCamera");
        checkObj = transform.Find("CheckObj");

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraMovement();
        BodyMovement();

        if (!Physics.CheckSphere(checkObj.position, .2f, ~LayerMask.GetMask("Terrain")))
        {
            transform.position -= new Vector3(0f, 0.05f, 0f);
        }

        if (transform.position.y < -5f) { transform.position = new Vector3(217f, 4f, 218f); }
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame) { Application.Quit(); }
    }

    private void BodyMovement()
    {
        float horizontal = pInput.actions["LeftRight"].ReadValue<float>();
        float vertical = pInput.actions["ForwardBack"].ReadValue<float>();

        Vector3 movementVect = MOVEMENT_SPEED * Time.deltaTime * new Vector3(horizontal, 0f, vertical).normalized;
        cc.Move(transform.TransformDirection(movementVect));
    }

    private void CameraMovement()
    {
        float horizontal = pInput.actions["Horizontal"].ReadValue<float>() * MOUSE_SENSITIVITY;
        float vertical = pInput.actions["Vertical"].ReadValue<float>() * MOUSE_SENSITIVITY;

        xRot -= vertical * Time.deltaTime;
        xRot = Mathf.Clamp(xRot, -85f, 85f);

        mainCam.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        Vector3 rotVect = horizontal * Time.deltaTime * Vector3.up;
        transform.Rotate(rotVect);
    }
}
