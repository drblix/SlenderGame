using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private Transform[] waypoints;

    private static Transform playerTransform;
    private static readonly Vector2 boxSize = new(515, 468);
    private static readonly Vector3 boxCenter = new(361, 0f, 336.7f);

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        playerTransform = player.transform;
    }

    public static Vector3 GetRandomPoint()
    {
        bool validSpot = false;
        Vector3 groundPoint = Vector3.zero;

        do
        {
            Vector2 randPoint = new Vector2(
                (Random.value - .5f) * boxSize.x,
                (Random.value - .5f) * boxSize.y
            );

            if (Physics.Raycast(new Vector3(randPoint.x, 100f, randPoint.y) + boxCenter, Vector3.down, out RaycastHit info, 250f))
            {
                groundPoint = info.point;
                validSpot = !Physics.CheckBox(groundPoint + new Vector3(0f, 2f, 0f), new Vector3(.82f, 1.31f, .525f), Quaternion.identity);
            }
        } while (!validSpot);

        return groundPoint;
    }

    public static Vector3 GetClosestPoint(float radius = 20f, float minDistance = 10f)
    {
        bool validSpot = false;
        Vector3 groundPoint = Vector3.zero;

        do
        {
            Vector2 point2;

            point2 = Random.insideUnitCircle * radius + new Vector2(playerTransform.position.x, playerTransform.position.z);
            point2 += new Vector2(minDistance + 4f, 0f) * Mathf.Sign(point2.x);
            point2 += new Vector2(0f, minDistance + 4f) * Mathf.Sign(point2.y);
            groundPoint = new Vector3(point2.x, 50f, point2.y);

            if (Physics.Raycast(groundPoint, Vector3.down, out RaycastHit info, 100f))
                groundPoint = info.point;
            else
                continue;

            if (Physics.CheckBox(groundPoint + new Vector3(0f, 2f, 0f), new Vector3(.82f, 1.31f, .525f), Quaternion.identity))
                continue;

            validSpot = true;
            float distance = Vector3.Distance(playerTransform.position, groundPoint);
            if (distance < minDistance)
            {
                point2 += new Vector2(minDistance, 0f) * Mathf.Sign(point2.x);
                point2 += new Vector2(0f, minDistance) * Mathf.Sign(point2.y);
                groundPoint = new Vector3(point2.x, 50f, point2.y);

                if (Physics.Raycast(groundPoint, Vector3.down, out RaycastHit info2, 100f))
                    groundPoint = info2.point;
                else {
                    validSpot = false;
                    continue;
                }
            }
        } while (!validSpot);

        return groundPoint;
    }
}
