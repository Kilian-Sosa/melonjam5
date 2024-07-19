using System;
using UnityEngine;

public class CameraRotationTrigger : MonoBehaviour {
    CameraBehaviour _cameraController;
    bool _direction = true;

    void Start() {
        _cameraController = Camera.main.GetComponent<CameraBehaviour>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.name == "PlayerObj") {
            _cameraController.Rotate(_direction);
            _direction = !_direction;
            gameObject.SetActive(false);
        }
    }
}
