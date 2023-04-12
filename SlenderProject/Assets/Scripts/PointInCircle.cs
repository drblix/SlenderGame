using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInCircle : MonoBehaviour
{
    private void Awake()
    {
        /*
        Vector3 center = transform.position;
        
        float radius = transform.localScale.x / 2f;

        Vector2 point = Random.insideUnitCircle * radius + new Vector2(transform.position.x, transform.position.z);
        Vector3 point3 = new Vector3(point.x, transform.position.y, point.y);
        Debug.Log(point3);
        new GameObject("CirclePoint").transform.position = point3;
        */

        StartCoroutine(PointRoutine());
    }

    private IEnumerator PointRoutine()
    {
        while (true)
        {
            float radius = transform.localScale.x / 2f;

            Vector2 randPoint = Random.insideUnitCircle;
            Debug.Log(randPoint);
            Debug.Log(randPoint.magnitude);
            Vector2 circlePoint = new Vector2(Mathf.Clamp(randPoint.x, -1f, 1f), Mathf.Clamp(randPoint.y, -1f, 1f));
            Vector2 point2 = circlePoint * radius + new Vector2(transform.position.x, transform.position.z);
            Vector3 point3 = new Vector3(point2.x, transform.position.y, point2.y);
            new GameObject("CirclePoint").transform.position = point3;

            yield return new WaitForSeconds(1f);
        }
    }
}
