using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    [SerializeField, Range(0, 10)] float _duration = 1f;
    [SerializeField, Range(0, 2)] float _strengthMultiplier = 0.2f;
    [SerializeField] AnimationCurve _curve;

    bool _isShaking;

    void OnValidate() => CurveSetup();

    public void Shake(float duration, float strengthMultiplier) {
        if (!_isShaking) StartCoroutine(ShakeCoroutine(duration, strengthMultiplier));
    }

    IEnumerator ShakeCoroutine(float duration, float strengthMultiplier) {
        // Wait until the camera stops transitioning
        var cameraBehaviour = GetComponent<CameraBehaviour>();
        if (cameraBehaviour._isTransitioning) yield break;

        _isShaking = true;
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration) {
            float strengthOverTime = _curve.Evaluate(elapsed / duration);
            transform.localPosition = originalPos + Random.insideUnitSphere * strengthOverTime * strengthMultiplier;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        _isShaking = false;
    }

    [ContextMenu("Curve Setup")]
    private void CurveSetup() {
        Keyframe[] keyframes = new Keyframe[3];
        keyframes[0] = new Keyframe(0f, 0f);
        keyframes[1] = new Keyframe(0.2f, 0.25f, -0.01f, -0.3f);
        keyframes[2] = new Keyframe(1f, 0f);
        _curve.keys = keyframes;
    }

#if UNITY_EDITOR
    [ContextMenu("Shake")]
    public void TestShake() => Shake(_duration, _strengthMultiplier);
#endif
}
