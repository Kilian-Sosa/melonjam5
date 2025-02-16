using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {
    [SerializeField] float speed = 2f;
    List<Vector2Int> _path;
    int _currentPathIndex;

    public void StartPath(List<Vector2Int> newPath) {
        _path = newPath;
        _currentPathIndex = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath() {
        while (_currentPathIndex < _path.Count) {
            Vector3 targetPosition = new(_path[_currentPathIndex].x, transform.position.y, _path[_currentPathIndex].y);
            while (transform.position != targetPosition) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                transform.LookAt(targetPosition);
                yield return null;
            }
            _currentPathIndex++;
        }
        AudioManager.Instance.PlaySFX("animalSaved");
        Destroy(gameObject);
    }
}
