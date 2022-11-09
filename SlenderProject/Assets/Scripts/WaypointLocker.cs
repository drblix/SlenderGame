using UnityEngine;

public class WaypointLocker : MonoBehaviour
{
    // sets all waypoint transforms to the ground
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Ray ray = new(child.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Terrain")))
            {
                child.position = hit.point;
            }
        }
    }
}
