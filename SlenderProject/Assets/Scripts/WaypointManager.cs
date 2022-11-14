using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private Transform[] waypoints;

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        // locks all waypoint transforms to the ground
        for (int i = 0; i < waypoints.Length; i++)
        {
            Ray ray = new(waypoints[i].position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Terrain")))
            {
                waypoints[i].position = hit.point;
            }
        }
    }

    public Transform GetClosestWaypoint()
    {
        Vector3 plrPos = player.transform.position;
        Transform closestPoint = null;
        float minDist = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.Distance(plrPos, waypoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = waypoints[i];
            }
        }

        //if (Vector3.Distance(closestPoint.position, plrPos) < 5f) { }

        return closestPoint;
    }

    public Transform GetRandomWaypoint(Transform curPoint = null)
    {
        Transform newPoint = waypoints[Random.Range(0, waypoints.Length)];

        // prevents the new point from being the current point if one is provided
        while (newPoint == curPoint) { newPoint = waypoints[Random.Range(0, waypoints.Length)]; }

        return newPoint;
    }
}
