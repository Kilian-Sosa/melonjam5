using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneBehaviour : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        GameManager.Instance.UpdateCounts();
        Destroy(other.gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        GameManager.Instance.UpdateCounts();
        Destroy(collision.gameObject);
    }
}
