using UnityEngine;
using UnityEngine.UI;

public class SlenderAI : MonoBehaviour
{
    private GameManager gameManager;

    private Transform player;
    private Transform mainCam;

    [SerializeField]
    private GameObject slenderMan;
    [SerializeField]
    private MeshRenderer slenderMesh;
    [SerializeField]
    private AudioSource[] audSources;
    [SerializeField]
    private RawImage staticVid;

    private enum SlenderStates
    {
        Searching,
        Pursuit
    }

    private const float MAX_TP_TIMER = 180f;
    private const float MIN_SIGHT_DIST = 75f;
    private const float LOS_TIMER = 4f;
    private float lookAmount = 0f;

    private float moveTimer = 0f;
    private float sightTimer = 0f;
    private float jumpscareTimer = 0f;

    // time it takes for slenderman to tp (in seconds)
    // starting = 180
    private float tpTimer = 10f;
    // how far slender can "see"
    // starting = 75
    private float sightDist = 75f;

    private bool seesPlayer = false;
    private bool hasBeenSeen = false;

    private SlenderStates curState = SlenderStates.Searching;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>().transform;
        mainCam = player.Find("Head").Find("MainCamera");

        slenderMan.transform.position = Vector3.zero;
        slenderMan.SetActive(false);

        gameManager.pageCollected.AddListener(UpdateDifficulty);
    }

    private void Update()
    {
        if (gameManager.gameOver) { return; }

        CheckVisiblity();

        if (!slenderMan.activeInHierarchy) { return; }

        if (moveTimer >= tpTimer)
        {
            moveTimer = 0f;

            if (curState == SlenderStates.Searching)
            {
                slenderMan.transform.position = WaypointManager.GetRandomPoint();
            }
            else
            {
                slenderMan.transform.position = WaypointManager.GetClosestPoint();
            }

            hasBeenSeen = false;
        }

        if (sightTimer >= LOS_TIMER)
        {
            sightTimer = 0f;
            seesPlayer = CanSeePlayer();

            curState = seesPlayer ? SlenderStates.Pursuit : SlenderStates.Searching;
        }

        slenderMan.transform.rotation = Quaternion.Euler(new(slenderMan.transform.eulerAngles.x, CalculatePAngle(), slenderMan.transform.eulerAngles.z));

        if (hasBeenSeen)
            moveTimer += Time.deltaTime * 2f;
        else
            moveTimer += Time.deltaTime;
        
        sightTimer += Time.deltaTime;
        jumpscareTimer -= Time.deltaTime;
    }

    private void CheckVisiblity()
    {
        Ray ray = new(mainCam.position, mainCam.TransformDirection(Vector3.forward));

        // basically checking if slenderman is visible in the player's camera using a spherecast
        // look amount maxed at 5
        if (Physics.SphereCast(ray, 11f, 38f, LayerMask.GetMask("Slender")) || Vector3.Distance(player.position, slenderMan.transform.position) < 10f)
        {
            hasBeenSeen = true;

            if (!audSources[0].isPlaying && jumpscareTimer <= 0f) {
                audSources[0].Play();
                jumpscareTimer = 12f;
            }

            lookAmount += Time.deltaTime * Mathf.Clamp(gameManager.currentPages / 4f, 1f, 2f);
        }
        else
        {
            lookAmount -= Time.deltaTime / 3f;
        }

        lookAmount = Mathf.Clamp(lookAmount, 0f, 5f);

        float calcAmnt = lookAmount / 5f;
        audSources[1].volume = calcAmnt;
        audSources[2].volume = calcAmnt;
        staticVid.color = new(255, 255, 255, calcAmnt);

        if (lookAmount >= 4f)
        {
            StartCoroutine(gameManager.PlayerDeath());
        }
    }

    public void UpdateDifficulty()
    {
        tpTimer = MAX_TP_TIMER - (20f * gameManager.currentPages);
        sightDist = MIN_SIGHT_DIST + (15.625f * gameManager.currentPages);

        slenderMan.SetActive(gameManager.currentPages >= 1);
    }

    public bool CanSeePlayer()
    {
        Ray ray = new(slenderMan.transform.position, player.position - slenderMan.transform.position);
        return Physics.Raycast(ray, sightDist, LayerMask.GetMask("Player"));
    }

    private float CalculatePAngle()
    {
        float s1 = player.transform.position.x - slenderMan.transform.position.x;
        float s2 = player.transform.position.z - slenderMan.transform.position.z;
        return Mathf.Atan2(s1, s2) * Mathf.Rad2Deg;
    }
}
