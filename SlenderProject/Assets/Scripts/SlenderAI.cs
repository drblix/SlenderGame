using UnityEngine;
using UnityEngine.UI;

public class SlenderAI : MonoBehaviour
{
    private WaypointManager pointManager;
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

    private SlenderStates curState = SlenderStates.Searching;


    private void Awake()
    {
        pointManager = FindObjectOfType<WaypointManager>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>().transform;
        mainCam = player.Find("Head").Find("MainCamera");

        slenderMan.transform.position = Vector3.zero;
        //slenderMan.SetActive(false);
    }

    private void Update()
    {
        if (gameManager.GameOver) { return; }

        CheckVisiblity();

        if (!slenderMan.activeInHierarchy) { return; }

        if (moveTimer >= tpTimer)
        {
            moveTimer = 0f;

            if (curState == SlenderStates.Searching)
            {
                slenderMan.transform.position = pointManager.GetRandomWaypoint().position;
            }
            else
            {
                slenderMan.transform.position = pointManager.GetClosestWaypoint().position;
            }
        }

        if (sightTimer >= 6f)
        {
            sightTimer = 0f;
            seesPlayer = CanSeePlayer();

            curState = seesPlayer ? SlenderStates.Pursuit : SlenderStates.Searching;
        }

        slenderMan.transform.rotation = Quaternion.Euler(new(slenderMan.transform.eulerAngles.x, CalculatePAngle(), slenderMan.transform.eulerAngles.z));

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
            if (!audSources[0].isPlaying && jumpscareTimer <= 0f) {
                audSources[0].Play();
                jumpscareTimer = 12f;
            }

            lookAmount += Time.deltaTime * Mathf.Clamp(gameManager.CurrentPages / 4f, 1f, 2f);
        }
        else
        {
            lookAmount -= Time.deltaTime;
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
        tpTimer -= MAX_TP_TIMER - (20f * gameManager.CurrentPages);
        sightDist = MIN_SIGHT_DIST + (15.625f * gameManager.CurrentPages);

        if (gameManager.CurrentPages >= 1) { slenderMan.SetActive(true); }
    }

    public bool CanSeePlayer()
    {
        Ray ray = new(slenderMan.transform.position, player.position - slenderMan.transform.position);

        if (Physics.Raycast(ray, sightDist, LayerMask.GetMask("Player"))) { return true; }

        return false;
    }

    private float CalculatePAngle()
    {
        float s1 = player.transform.position.x - slenderMan.transform.position.x;
        float s2 = player.transform.position.z - slenderMan.transform.position.z;
        return Mathf.Atan2(s1, s2) * Mathf.Rad2Deg;
    }
}
