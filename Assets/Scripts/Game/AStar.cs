using System.Collections.Generic;
using UnityEngine;
public class AStar : MonoBehaviour {
    public Material pathMaterial, startMaterial, goalMaterial, solutionMaterial;
    private Vector2Int startNode, goalNode;

    private int[,] mazeMap;
    private GameObject[,] mazeObjMap;
    private bool isDone;

    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        if (Input.GetMouseButtonDown(0)) {
            if (isDone) ClearPath();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Path")) {
                    int x = Mathf.RoundToInt(hitObject.transform.position.x);
                    int y = Mathf.RoundToInt(hitObject.transform.position.z);

                    if (startNode == default(Vector2Int)) {
                        startNode = new Vector2Int(x, y);
                        hitObject.GetComponent<Renderer>().material = startMaterial;
                    } else if (goalNode == default(Vector2Int)) {
                        goalNode = new Vector2Int(x, y);

                        mazeMap = MazeGenerator.maze;
                        mazeObjMap = MazeGenerator.mazeObj;
                        List<Vector2Int> path = AStarSearch(startNode, goalNode);

                        foreach (Vector2Int node in path)
                            mazeObjMap[node.x, node.y].GetComponent<Renderer>().material = solutionMaterial;
                        hitObject.GetComponent<Renderer>().material = goalMaterial;
                        startNode = default(Vector2Int);
                        goalNode = default(Vector2Int);
                        isDone = true;
                    }
                }
            }
        }
    }

    void ClearPath() {
        for (int i = 0; i < mazeMap.GetLength(0); i++)
            for (int j = 0; j < mazeMap.GetLength(1); j++)
                if (mazeObjMap[i, j].CompareTag("Path"))
                    mazeObjMap[i, j].GetComponent<Renderer>().material = pathMaterial;
        isDone = false;
    }

    List<Vector2Int> AStarSearch(Vector2Int start, Vector2Int end) {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0) {
            Vector2Int current = queue.Dequeue();

            if (current == end) break;

            foreach (var neighbor in GetNeighbors(current)) {
                if (!cameFrom.ContainsKey(neighbor)) {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int currentPos = end;

        while (currentPos != start) {
            path.Add(currentPos);
            currentPos = cameFrom[currentPos];
        }

        path.Reverse();
        path.RemoveAt(path.Count - 1);
        return path;
    }

    List<Vector2Int> GetNeighbors(Vector2Int position) {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] possibleNeighbors = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        foreach (var offset in possibleNeighbors) {
            Vector2Int neighbor = position + offset;

            if (neighbor.x >= 0 && neighbor.x < mazeMap.GetLength(0) && neighbor.y >= 0 && neighbor.y < mazeMap.GetLength(1) &&
                mazeMap[neighbor.x, neighbor.y] == 0) {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }
}