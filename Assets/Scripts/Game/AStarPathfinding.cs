using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour {
    [SerializeField] GameObject _container;
    Vector2Int startNode, goalNode;
    int[,] mazeMap;

    public void SetMaze(int[,] maze, Vector2Int start = default, Vector2Int end = default) {
        mazeMap = maze;
        if (start != default) startNode = start;
        if (start != default) goalNode = end;

        // After setting the maze, try to find the path
        if (mazeMap != null) {
            List<Vector2Int> path = FindPath();
            if (path != null) _container.BroadcastMessage("StartPath", path);
        }
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