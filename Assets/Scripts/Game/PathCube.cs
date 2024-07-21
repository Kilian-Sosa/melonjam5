using UnityEngine;

public class PathCube : MonoBehaviour {
    Vector3 _correctPosition;
    bool _isCorrect = false;

    void OnMouseDown() {
        if (!_isCorrect) {
            Vector3 oldPosition = transform.position;
            Vector3 newPosition = _correctPosition;

            if (!LevelGenerator.IsPositionOccupied(newPosition)) {
                AudioManager.Instance.PlaySFX("pathMoved");
                // Update mazeStatus: set old position to wall (1) and new position to path (0)
                LevelGenerator.mazeStatus[(int)oldPosition.x, (int)oldPosition.z] = 1;
                LevelGenerator.mazeStatus[(int)newPosition.x, (int)newPosition.z] = 0;

                transform.position = newPosition;
                _isCorrect = true;

                LevelGenerator.UpdateMazeStatus(oldPosition, newPosition);
            }
        }
    }

    public void SetCorrectPosition(Vector3 position) => _correctPosition = position;

    public Vector3 GetCorrectPosition() => _correctPosition;
}
