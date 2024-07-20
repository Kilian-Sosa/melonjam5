using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public GameObject wallPrefab, pathPrefab;
    public static int[,] maze;
    public static GameObject[,] mazeObj;

    void Start() {
        LoadLevel("level1");
    }

    void LoadLevel(string filename) {
        switch(filename) {
            case "level1":
                maze = new int[,] {
                    {1, 0, 1, 1, 1},
                    {1, 0, 0, 0, 1},
                    {1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 1},
                    {1, 1, 0, 1, 1}
                };
                break;
            case "level2":
                maze = new int[,] {
                    {1, 1, 1, 1, 1},
                    {0, 0, 0, 0, 1},
                    {1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0},
                    {1, 1, 1, 1, 1}
                };
                break;
            case "level3":
                maze = new int[,] {
                    {1, 1, 1, 0, 1},
                    {1, 1, 1, 0, 1},
                    {1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0},
                    {1, 0, 1, 1, 1}
                };
                break;
            case "level4":
                maze = new int[,] {
                    {1, 1, 1, 1, 1, 1},
                    {1, 0, 0, 0, 0, 0},
                    {1, 1, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0, 1},
                    {1, 1, 1, 1, 0, 1},
                    {1, 1, 1, 1, 0, 1}
                };
                break;

            case "level5":
                maze = new int[,] {
                    {1, 1, 1, 1, 0, 1},
                    {0, 0, 0, 1, 0, 1},
                    {0, 1, 0, 1, 0, 1},
                    {1, 0, 0, 0, 0, 1},
                    {1, 1, 1, 1, 1, 1},
                    {1, 1, 1, 1, 1, 1}
                };
                break;

            case "level6":
                maze = new int[,] {
                    {1, 0, 1, 1, 1, 1, 1},
                    {1, 0, 0, 0, 0, 0, 1},
                    {1, 1, 1, 0, 1, 0, 1},
                    {1, 0, 1, 0, 0, 0, 0},
                    {1, 0, 0, 1, 1, 0, 1},
                    {1, 1, 1, 1, 0, 1, 1},
                    {1, 1, 1, 1, 1, 1, 1}
                };
                break;

            case "level7":
                maze = new int[,] {
                    {1, 1, 1, 1, 1, 1, 1},
                    {0, 0, 0, 1, 0, 0, 1},
                    {1, 1, 0, 1, 1, 0, 1},
                    {1, 0, 0, 0, 0, 0, 1},
                    {1, 1, 0, 1, 0, 0, 1},
                    {1, 0, 0, 1, 1, 0, 1},
                    {1, 0, 1, 1, 1, 1, 1}
                };
                break;
        }
        mazeObj = new GameObject[maze.GetLength(0), maze.GetLength(1)];
        ShowMaze();
    }

    void ShowMaze() {
        for (int j = maze.GetLength(1) - 1; j >= 0; j--) {
            for (int i = 0; i < maze.GetLength(0); i++) {
                Vector3 position = new(i * 1f, 0, j * 1f);
                if (maze[i, j] == 1) mazeObj[i, j] = Instantiate(wallPrefab, position, Quaternion.identity);
                else {
                    GameObject pathObj = Instantiate(pathPrefab, position, Quaternion.identity);
                    pathObj.GetComponent<PathCube>().correctPosition = position;
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
            if (obj != null && obj.CompareTag("Path") && obj.transform.position == position 
                && obj.GetComponent<PathCube>().correctPosition == position) return true;
        return false;
    }

    void MoveObj(GameObject obj) {
        Vector3 randomPosition;
        do {
            randomPosition = new Vector3(
                Mathf.Round(obj.transform.position.x + Random.Range(-2, 2)),
                0,
                Mathf.Round(obj.transform.position.z + Random.Range(-2, 2))
            );
        } while (!IsValidPosition(randomPosition) || IsPositionOccupied(randomPosition));

        obj.transform.position = randomPosition;
    }

    void MoveRandomPathObjects() {
        List<GameObject> pathObjects = new();

        foreach (GameObject obj in mazeObj)
            if (obj != null && obj.CompareTag("Path")) pathObjects.Add(obj);

        int numberToMove = Mathf.CeilToInt(pathObjects.Count / 3f);
        ShuffleList(pathObjects);
        List<GameObject> objectsToMove = pathObjects.GetRange(0, numberToMove);

        foreach (GameObject obj in objectsToMove) MoveObj(obj);
    }

    void ShuffleList<T>(List<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    IEnumerator ShufflePath() {
        for (int i = 3; i > 0; i--) {
            yield return new WaitForSeconds(1);
            ShowCountdown(i);
        }
        Debug.Log("Start shuffling path objects");
        MoveRandomPathObjects();

        // Start hide animation
        yield return null;
    }

    void ShowCountdown(int number) => Debug.Log(number);
}