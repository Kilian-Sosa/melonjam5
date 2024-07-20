using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {
    public float speed = 1f;
    private List<Vector2Int> path;
    private int currentPathIndex;

    public void StartPath(List<Vector2Int> newPath) {
        path = newPath;
        currentPathIndex = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath() {
        while (currentPathIndex < path.Count) {
            Vector3 targetPosition = new(path[currentPathIndex].x, transform.position.y, path[currentPathIndex].y);
            while (transform.position != targetPosition) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
            currentPathIndex++;
        }
    }
}
