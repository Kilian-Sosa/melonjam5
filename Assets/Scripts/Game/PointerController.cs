using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PointerController : MonoBehaviour {
    [SerializeField] GameObject target;
    [SerializeField] GameObject goal;
    [SerializeField] TMP_Text distance;


    void Update() {
        if (!GameManager.Instance.isActive) return;
        float x = target.transform.position.x;
        float z = target.transform.position.z;
        float y = target.transform.position.y;
        gameObject.transform.position = new Vector3(x, y + 3f, z);

        gameObject.transform.LookAt(target.transform);
    }
}
