using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public static int[,] maze, mazeStatus;
    public static GameObject[,] mazeObj;
    public static AStarPathfinding _aStarPathfinding;

    [Header("Prefabs")]
    [SerializeField] GameObject _pathPrefab, _wallPrefab;

    [Header("Materials")]
    [SerializeField] Material pathMovedMaterial, goalMaterial;
    Vector2Int start = new(0, 0), end = new(0, 0);
    static Material pathMaterial, staticGoalMaterial;

    void Start() {
        staticGoalMaterial = goalMaterial;
        _aStarPathfinding = GetComponent<AStarPathfinding>();
        //PlayerPrefs.SetInt("level", 1);
        LoadLevel(PlayerPrefs.GetInt("level", 1));
    }

    void LoadLevel(int levelNum) {
        switch(levelNum) {
            case 1:
                maze = new int[,] {
                    {1, 0, 1, 1, 1},
                    {1, 0, 0, 0, 1},
                    {1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 1},
                    {1, 1, 0, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(4, 2);
                break;
            case 2:
                maze = new int[,] {
                    {1, 0, 1, 1, 1},
                    {1, 0, 0, 0, 1},
                    {1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0},
                    {1, 1, 1, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(3, 4);
                break;
            case 3:
                maze = new int[,] {
                    {1, 0, 1, 1, 1},
                    {1, 0, 1, 0, 1},
                    {1, 0, 1, 0, 1},
                    {1, 0, 0, 0, 0},
                    {1, 0, 1, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(3, 4);
                break;
            case 4:
                maze = new int[,] {
                    {1, 0, 1, 1, 1, 1},
                    {1, 0, 0, 0, 0, 0},
                    {1, 1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0, 1},
                    {1, 1, 1, 1, 0, 1},
                    {1, 1, 1, 1, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(1, 5);
                break;
            case 5:
                maze = new int[,] {
                    {1, 0, 1, 1, 0, 1},
                    {1, 0, 0, 1, 0, 1},
                    {0, 1, 0, 1, 0, 1},
                    {0, 0, 0, 0, 0, 1},
                    {1, 1, 0, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(0, 4);
                break;
            case 6:
                maze = new int[,] {
                    {1, 0, 1, 1, 1, 1, 1},
                    {1, 0, 0, 0, 0, 0, 1},
                    {1, 1, 1, 0, 1, 0, 1},
                    {1, 0, 1, 0, 0, 0, 0},
                    {1, 0, 0, 1, 1, 0, 1},
                    {1, 1, 1, 1, 0, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(3, 6);
                break;
            case 7:
                maze = new int[,] {
                    {1, 0, 1, 1, 1, 1, 1},
                    {0, 0, 0, 1, 0, 0, 1},
                    {1, 1, 0, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0, 0, 1},
                    {1, 1, 0, 1, 0, 0, 1},
                    {1, 0, 0, 1, 1, 0, 1},
                    {1, 0, 1, 1, 1, 1, 1}
                };
                start = new Vector2Int(0, 1);
                end = new Vector2Int(6, 1);
                break;
        }
        mazeStatus = DeepCopyMaze(maze);
        mazeObj = new GameObject[maze.GetLength(0), maze.GetLength(1)];
        ShowMaze();
    }

    void ShowMaze() {
        for (int j = maze.GetLength(1) - 1; j >= 0; j--) {
            for (int i = 0; i < maze.GetLength(0); i++) {
                Vector3 position = new(i * 1f, 0, j * 1f);
                if (maze[i, j] == 1) mazeObj[i, j] = Instantiate(_wallPrefab, position, Quaternion.identity);
                else {
                    GameObject pathObj = Instantiate(_pathPrefab, position, Quaternion.identity);
                    pathObj.GetComponent<PathCube>().SetCorrectPosition(position);
                    if (pathMaterial == null) pathMaterial = pathObj.transform.GetChild(0).GetComponent<Renderer>().material;

                    // Set start and end colors
                    if ((i == start.x && j == start.y) || (i == end.x && j == end.y)) {
                        pathObj.transform.GetChild(0).GetComponent<Renderer>().material = goalMaterial;
                        if (i == end.x && j == end.y) pathObj.transform.GetChild(0).gameObject.SetActive(true); // Show door
                    }
                    mazeObj[i, j] = pathObj;
                }
            }
        }

        StartCoroutine(ShufflePath());
    }

    public static bool IsValidPosition(Vector3 position) => position.x >= 0 && position.x < maze.GetLength(0)
            && position.z >= 0 && position.z < maze.GetLength(1);

    public static bool IsPositionOccupied(Vector3 position) {
        foreach (GameObject obj in mazeObj)
            if (obj != null && obj.CompareTag("Path") && obj.transform.position == position) return true;
        return false;
    }

    IEnumerator MoveObj(GameObject obj) {
        Vector3 oldPosition = obj.transform.position;
        Vector3 randomPosition;
        do {
            randomPosition = new Vector3(
                Mathf.Round(oldPosition.x + Random.Range(-2, 2)),
                0,
                Mathf.Round(oldPosition.z + Random.Range(-2, 2))
            );
        } while (!IsValidPosition(randomPosition) || IsPositionOccupied(randomPosition));

        // Update mazeStatus: set old position to wall (1) and new position to path (0)
        mazeStatus[(int)oldPosition.x, (int)oldPosition.z] = 1;
        mazeStatus[(int)randomPosition.x, (int)randomPosition.z] = 0;

        obj.transform.position = randomPosition;
        obj.transform.GetChild(0).GetComponent<Renderer>().material = pathMovedMaterial;
        yield return new WaitForEndOfFrame();
    }

    void MoveRandomPathObjects() {
        List<GameObject> pathObjects = new();
        _aStarPathfinding.SetMaze(mazeStatus, start, end); // Set maze for pathfinding

        foreach (GameObject obj in mazeObj) {
            if (obj != null && obj.CompareTag("Path")) {
                Vector3 position = obj.transform.position;
                Vector2Int pos = new((int)position.x, (int)position.z);

                // Exclude start and end positions
                if (pos != start && pos != end) pathObjects.Add(obj);
            }
        }

        int numberToMove = Mathf.CeilToInt(pathObjects.Count / 3f);
        ShuffleList(pathObjects);
        List<GameObject> objectsToMove = pathObjects.GetRange(0, numberToMove);

        StartCoroutine(MoveObjectsSequentially(objectsToMove));
    }

    IEnumerator MoveObjectsSequentially(List<GameObject> objects) {
        foreach (GameObject obj in objects) yield return MoveObj(obj);
    }

    void ShuffleList<T>(List<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    IEnumerator ShufflePath() {
        ActivateCurtains();
        for (int i = 3; i > 0; i--) {
            yield return new WaitForSeconds(1);
            //ShowCountdown(i);
        }

        MoveRandomPathObjects();
        GameManager.Instance.StartTimer();
    }

    void ShowCountdown(int number) => Debug.Log(number);

    void ActivateCurtains() => GameObject.Find("UI").transform.Find("Curtains").gameObject.SetActive(true);

    int[,] DeepCopyMaze(int[,] originalMaze) {
        int width = originalMaze.GetLength(0);
        int height = originalMaze.GetLength(1);
        int[,] newMaze = new int[width, height];

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                newMaze[i, j] = originalMaze[i, j];
        return newMaze;
    }

    public static void UpdateMazeStatus(Vector3 oldPosition, Vector3 newPosition) {
        mazeStatus[(int)oldPosition.x, (int)oldPosition.z] = 1;
        mazeStatus[(int)newPosition.x, (int)newPosition.z] = 0;
        mazeObj[(int)newPosition.x, (int)newPosition.z].transform.GetChild(0).GetComponent<Renderer>().material = pathMaterial;

        if (AreMazesEqual()) {
            _aStarPathfinding.SendFolow(staticGoalMaterial);
            GameManager.Instance.StopTimer();
        }
    }

    static bool AreMazesEqual() {
        for (int i = 0; i < maze.GetLength(0); i++)
            for (int j = 0; j < maze.GetLength(1); j++)
                if (maze[i, j] != mazeStatus[i, j]) return false;
        return true;
    }
}