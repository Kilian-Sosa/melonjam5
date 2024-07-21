using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour {
    [SerializeField] GameObject _container;
    Vector2Int startNode, goalNode;
    int[,] mazeMap;
    List<Vector2Int> path;

    public void SetMaze(int[,] maze, Vector2Int start = default, Vector2Int end = default) {
        mazeMap = maze;
        if (start != default) startNode = start;
        if (start != default) goalNode = end;

        // After setting the maze, try to find the path
        if (mazeMap != null) path = FindPath();
    }

    public void SendFolow(Material goalMaterial) => StartCoroutine(SendFollowCoroutine(goalMaterial));
 
    IEnumerator SendFollowCoroutine(Material goalMaterial) {
        if (path != null) {
            ShowPath(goalMaterial);
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < _container.transform.childCount; i++) {
                _container.transform.GetChild(i).gameObject.BroadcastMessage("StartPath", path);
                yield return new WaitForSeconds(Random.Range(0.4f, 0.6f));
            }
            LevelGenerator.mazeObj[goalNode.x, goalNode.y].GetComponent<CheckpointController>().enabled = true;
        }
        yield return null;
    }

    void ShowPath(Material goalMaterial) {
        foreach (var position in path)
            LevelGenerator.mazeObj[position.x, position.y].transform.GetChild(0).GetComponent<Renderer>().material = goalMaterial;
    }

    List<Vector2Int> FindPath() {
        Queue<Vector2Int> queue = new();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new();

        queue.Enqueue(startNode);
        cameFrom[startNode] = startNode;

        do {
            Vector2Int current = queue.Dequeue();

            if (current == goalNode) break;

            foreach (var neighbor in GetNeighbors(current)) {
                if (!cameFrom.ContainsKey(neighbor)) {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        } while (queue.Count > 0);

        List<Vector2Int> path = new();
        Vector2Int currentPos = goalNode;

        while (currentPos != startNode) {
            path.Add(currentPos);
            try { currentPos = cameFrom[currentPos]; } catch (KeyNotFoundException) { return null; }
        }

        // Add the end node to the path
        path.Add(startNode);
        path.Reverse();
        return path;
    }

    List<Vector2Int> GetNeighbors(Vector2Int position) {
        List<Vector2Int> neighbors = new();

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