using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTester : MonoBehaviour
{
    private void Update()
    {
        bool result = Physics.CheckBox(transform.position, transform.localScale / 2f, Quaternion.Euler(transform.eulerAngles));
        Debug.Log(result);
    }
}
