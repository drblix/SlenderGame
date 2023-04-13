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

        // StartCoroutine(PointRoutine());
    }

    private IEnumerator PointRoutine()
    {
        while (true)
        {
            const float MIN_DISTANCE = 10f;
            const float RADIUS = 20f;
            Vector2 point2;
            Vector3 point3;

            point2 = Random.insideUnitCircle * RADIUS + new Vector2(transform.position.x, transform.position.z);
            point2 += new Vector2(MIN_DISTANCE + 4f, 0f) * Mathf.Sign(point2.x);
            point2 += new Vector2(0f, MIN_DISTANCE + 4f) * Mathf.Sign(point2.y);
            point3 = new Vector3(point2.x, transform.position.y, point2.y);

            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(obj.GetComponent<BoxCollider>());
            obj.transform.position = point3;
            obj.transform.localScale = Vector3.one * .1f;

            if (Physics.CheckBox(point3, obj.transform.localScale / 2f, Quaternion.identity))
                Destroy(obj);

            float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), point2);
            if (distance < MIN_DISTANCE)
            {
                point2 += new Vector2(MIN_DISTANCE, 0f) * Mathf.Sign(point2.x);
                point2 += new Vector2(0f, MIN_DISTANCE) * Mathf.Sign(point2.y);
                point3 = new Vector3(point2.x, transform.position.y, point2.y);
                obj.transform.position = point3;
            }

            yield return new WaitForSeconds(.1f);
        }
    }
}
