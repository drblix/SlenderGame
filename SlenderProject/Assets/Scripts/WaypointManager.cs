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

    // TODO: 
    // revamp waypoint system by picking random locations on the map instead of set waypoints
    // use Physics.CheckBox() to prevent spawning inside objects 
    // for waypoints close to player, get a position within a circle created around the player
    // ^^ no idea how to do the above bit, but it's all about experimentation! :D
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

    public static Vector3 RandomPointInBox(Vector3 center, Vector3 size)
    {
        Vector3 randomVec = new Vector3(
            (Random.value - .5f) * size.x,
            (Random.value - .5f) * size.y,
            (Random.value - .5f) * size.z
        );

        return center + randomVec;
    }
}
