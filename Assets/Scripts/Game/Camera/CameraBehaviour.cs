using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
    [SerializeField] Transform[] _rotationPoints;
    [SerializeField] float _transitionSpeed = 2f;
    int _currentRotationIndex = -1;

    public bool _isTransitioning = false;
    Transform _targetRotationPoint;

    void Start() {
        AudioManager.Instance.StopSFX();
        Rotate(true);
        GameManager.Instance.ResumeGame();
    }

    void Update() {
        if (!GameManager.Instance.isActive) return;

        if (_isTransitioning) {
            transform.position = Vector3.Lerp(transform.position, _targetRotationPoint.position, _transitionSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotationPoint.rotation, _transitionSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetRotationPoint.position) < 0.1f && Quaternion.Angle(transform.rotation, _targetRotationPoint.rotation) < 1f) {
                _isTransitioning = false;
            }
        }
    }

    public void Rotate(bool movingForward) {
        if (movingForward) _currentRotationIndex = (_currentRotationIndex + 1) % _rotationPoints.Length;
        else _currentRotationIndex = (_currentRotationIndex - 1 + _rotationPoints.Length) % _rotationPoints.Length;

        _targetRotationPoint = _rotationPoints[_currentRotationIndex];
        _isTransitioning = true;
    }
}
