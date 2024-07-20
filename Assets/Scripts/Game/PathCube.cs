using UnityEngine;

public class PathCube : MonoBehaviour {
    public Vector3 correctPosition;
    private bool isCorrect = false;

    void OnMouseDown() {
        if (!isCorrect) {
            Vector3 newPosition = correctPosition;

            if (!LevelGenerator.IsPositionOccupied(newPosition)) {
                transform.position = newPosition;
                isCorrect = true;
            }
        }
    }
}
