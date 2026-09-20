using Clipper2Lib;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour {
    private PolygonCollider2D col;
    private readonly List<Vector2[]> paths = new();
    private readonly Stack<int> freeStack = new Stack<int>();
    private readonly Vector2[] emptyPolygon = new Vector2[0];
    /// <summary> The minimum allowed distance from center of player to edge of dangerzone </summary>
    public float DangerZoneOffsetX = 0.5f;
    public float DangerZoneOffsetY = 0.5f;


    private void Awake() {
        col = GetComponent<PolygonCollider2D>();
    }

    private List<Vector2> PathDToList(PathD path) {
        List<Vector2> result = new();
        foreach (PointD point in path) {
            result.Add(new Vector2((float)point.x, (float)point.y));
        }
        return result;
    }

    private PathD ListToPathD(List<Vector2> path) {
        PathD result = new();
        foreach (Vector2 point in path) {
            result.Add(new PointD(point.x, point.y));
        }
        return result;
    }

    private void UpdateCollider() {
        // inflate polygon so that if **center** of player is in the polygon it means player will try to dodge
        PathsD inflatedPaths = new();
        foreach (Vector2[] path in paths) {
            if (path != emptyPolygon) {
                var inflatedPath = PolygonExpander.AddRectOffset(path, DangerZoneOffsetX, DangerZoneOffsetY);
                inflatedPaths.Add(ListToPathD(inflatedPath));
            }
        }

        // union any overlapping polygons then add the remainign polygons to the polygon collider
        PathsD dangerZone = Clipper.Union(inflatedPaths, FillRule.NonZero);
        col.pathCount = dangerZone.Count;
        for (int i = 0; i < dangerZone.Count; i++) {
            col.SetPath(i, PathDToList(dangerZone[i]));
        }
    }

    /// <summary> creates a new path and returns the index of that path </summary>
    public int NewPath(Vector2[] points) {
        if (points == null) {
            Debug.LogWarning("Cannot create new path with null array");
            return -1;
        }
        int index;
        if (freeStack.Count > 0) {
            index = freeStack.Pop();
        } else {
            index = paths.Count;
            paths.Add(emptyPolygon);
        }

        paths[index] = points;

        UpdateCollider();

        return index;
    }

    /// <summary> frees(clears) the path at the index, allowing it to be used again later </summary>
    public void FreePath(int index) {
        if (index >= 0 && index < paths.Count && paths[index] != emptyPolygon) {
            paths[index] = emptyPolygon;
            freeStack.Push(index);
            UpdateCollider();
        }
    }
}
