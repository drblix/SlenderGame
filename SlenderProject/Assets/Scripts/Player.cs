using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private CharacterController cc;
    private PlayerInput pInput;
    private Rigidbody rb;
    private Camera cam;
    private AudioSource stepSource;
    private AudioSource breathingSource;

    private Transform mainCam;
    private Transform checkObj;
    private Transform flashlight;
    private Transform head;

    private Vector3 idleVec;
    private Quaternion idleRot;
    private Vector3 runningVec;
    private Quaternion runningRot;

    private float xRot = 0f;

    private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } }
    private Vector3 lastPos;

    [Header("Movement Settings")]
    [SerializeField]
    private float movementSpeed = 3f;
    [SerializeField]
    private float fallSpeed = 9f;

    private const float MAX_STAMINA = 6f;
    private const float SPRINT_THRESHOLD = 3.5f;
    private float currentStamina = 6f;

    [SerializeField]
    private AudioClip[] stepClips;

    [Header("Headbob Settings")]
    [SerializeField]
    private float bobFrequency = 5f;
    [SerializeField]
    private float vertAmp = .1f;
    [SerializeField]
    private float horzAmp = .1f;
    [SerializeField][Range(0f, 1f)]
    private float bobSmoothing = .1f;

    private float walkTime;
    private Vector3 targetBobPos;

    private float[] layerDistances = new float[32];

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        pInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        head = transform.Find("Head");
        mainCam = head.Find("MainCamera");

        cam = mainCam.GetComponent<Camera>();
        stepSource = mainCam.GetComponent<AudioSource>();
        breathingSource = head.GetComponent<AudioSource>();
        checkObj = transform.Find("CheckObj");
        flashlight = mainCam.Find("Flashlight");

        Cursor.lockState = CursorLockMode.Locked;

        // creates manual clipping distances for specific objects
        // [8] would be map objects, which have a clipping distance of 25 units
        // numbers correlate to their layer number
        layerDistances[8] = PlayerSettings.mapObjDraw;
        cam.layerCullDistances = layerDistances;

        //transform.position = new Vector3(215f, 2f, 210f);

        // flashlight pos states
        idleVec = new(0f, -.25f, 0f);
        idleRot = Quaternion.Euler(Vector3.zero);
        runningVec = new(0.63f, -0.63f, 0f);
        runningRot = Quaternion.Euler(30f, 0f, 0f);

        currentStamina = MAX_STAMINA;

        Time.timeScale = 1f;
    }

    private void Update()
    {
        CameraMovement();
        BodyMovement();
        Sprinting();
        HandleFootsteps();
        if (PlayerSettings.headBobbing)
            HandleBobbing();

        // checks if player is on the ground, applying gravity if not
        if (!IsGrounded())
        {
            transform.position += fallSpeed * Time.deltaTime * Vector3.down;
        }

        // fail-safe if the player clips through the ground somehow
        if (transform.position.y < -5f) { transform.position = new Vector3(217f, 4f, 218f); }

        lastPos = transform.position;
    }

    private void BodyMovement()
    {
        float horizontal = pInput.actions["LeftRight"].ReadValue<float>();
        float vertical = pInput.actions["ForwardBack"].ReadValue<float>();

        Vector3 movementVect = movementSpeed * Time.deltaTime * new Vector3(horizontal, 0f, vertical).normalized;

        // increases move speed if running straight
        // decreases move speed if moving backwards
        if (isRunning && (movementVect.z > 0f || (movementVect.x != 0f && !(movementVect.z < 0f))) && currentStamina > 0f)
        {
            movementVect *= 1.5f;
        }
        else if (movementVect.z < 0f || currentStamina <= 0f)
        {
            isRunning = false;
            stepSource.pitch = 1f;
            movementVect *= 0.8f;
        }

        movementVect = transform.TransformDirection(movementVect);
        cc.Move(movementVect);
    }

    private void CameraMovement()
    {
        float horizontal = pInput.actions["Horizontal"].ReadValue<float>() * PlayerSettings.mouseSensitivity;
        float vertical = pInput.actions["Vertical"].ReadValue<float>() * PlayerSettings.mouseSensitivity;

        xRot = PlayerSettings.invertMouse ? xRot += vertical * Time.deltaTime : xRot -= vertical * Time.deltaTime;
        xRot = Mathf.Clamp(xRot, -85f, 85f);

        mainCam.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        Vector3 rotVect = horizontal * Time.deltaTime * Vector3.up;
        transform.Rotate(rotVect);
    }

    private void Sprinting()
    {
        if (pInput.actions["LeftShift"].WasPressedThisFrame() && IsGrounded() && IsMoving() && currentStamina >= SPRINT_THRESHOLD)
        {
            isRunning = true;
            stepSource.pitch = 1.3f;
            flashlight.parent = head;
            currentStamina -= Time.deltaTime;
        }
        else if (pInput.actions["LeftShift"].WasReleasedThisFrame() || !IsMoving())
        {
            isRunning = false;
            stepSource.pitch = 1f;
            flashlight.parent = mainCam;
            currentStamina += Time.deltaTime / 1.5f;
        }

        currentStamina = isRunning ? currentStamina -= Time.deltaTime : currentStamina += (Time.deltaTime / 1.5f);
        currentStamina = Mathf.Clamp(currentStamina, 0f, MAX_STAMINA);

        if (currentStamina <= 0f && !breathingSource.isPlaying)
        {
            breathingSource.Play();
        }
        else if (breathingSource.isPlaying && currentStamina >= SPRINT_THRESHOLD)
        {
            breathingSource.Stop();
        }

        float slerpTime = Time.deltaTime / .25f;
        if (isRunning)
        {
            flashlight.localRotation = Quaternion.Slerp(flashlight.localRotation, runningRot, slerpTime);
            flashlight.localPosition = Vector3.Slerp(flashlight.localPosition, runningVec, slerpTime);
        }
        else
        {
            flashlight.localRotation = Quaternion.Slerp(flashlight.localRotation, idleRot, slerpTime);
            flashlight.localPosition = Vector3.Slerp(flashlight.localPosition, idleVec, slerpTime);
        }
    }

    private void HandleFootsteps()
    {
        if (!IsMoving() || stepSource.isPlaying) { return; }

        stepSource.clip = stepClips[Random.Range(0, stepClips.Length)];
        stepSource.Play();
    }

    private void HandleBobbing()
    {
        walkTime = IsMoving() ? walkTime += Time.deltaTime : 0f;

        targetBobPos = head.position + CalcOffset(walkTime);

        mainCam.position = Vector3.Lerp(mainCam.position, targetBobPos, bobSmoothing);

        mainCam.position = (mainCam.position - targetBobPos).magnitude <= 0.001f ? targetBobPos : mainCam.position;
    }


    private Vector3 CalcOffset(float time)
    {
        Vector3 offset = Vector3.zero;

        if (time > 0f)
        {
            float horzOff;
            float vertOff;

            if (isRunning)
            {
                horzOff = Mathf.Cos(time * bobFrequency) * horzAmp * 1.25f;
                vertOff = Mathf.Sin(time * bobFrequency * 2) * vertAmp * 1.25f;
            }
            else
            {
                horzOff = Mathf.Cos(time * bobFrequency) * horzAmp;
                vertOff = Mathf.Sin(time * bobFrequency * 2) * vertAmp;
            }

            offset = transform.right * horzOff + transform.up * vertOff;
        }

        return offset;
    }

    public void ToggleAmbience(bool state)
    {
        // false = pause ; true = resume
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            if (state && !source.isPlaying)
            {
                source.UnPause();
            }
            else if (!state)
            {
                source.Pause();
            }
        }
    }

    public void SetObjDistance(float num)
    {
        PlayerSettings.mapObjDraw = (int)num;
        layerDistances[8] = PlayerSettings.mapObjDraw;
        cam.layerCullDistances = layerDistances;
    }

    private bool IsGrounded() => Physics.Raycast(checkObj.position, Vector3.down, .5f, LayerMask.GetMask("Terrain"));

    private bool IsMoving() => transform.position != lastPos;
}

public static class PlayerSettings
{
    public static float mouseSensitivity = 30f;
    public static int mapObjDraw = 50;
    public static bool headBobbing = true;
    public static bool invertMouse = false;
}
