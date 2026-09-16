using Clipper2Lib;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour {
    private PolygonCollider2D col;
    private PathsD paths = new PathsD();
    private Stack<int> freeStack = new Stack<int>();
    private readonly PathD emptyPolygon = new PathD();
    /// <summary> The minimum allowed distance from center of player to edge of dangerzone </summary>
    public float DangerZoneRadius = Mathf.Sqrt(2) / 2;


    private void Start() {
        col = GetComponent<PolygonCollider2D>();
    }

    private void UpdateCollider() {
        // inflate polygon so that if **center** of player is in the polygon it means player will try to dodge
        PathsD inflatedPaths = Clipper.InflatePaths(paths, DangerZoneRadius,
            JoinType.Miter, EndType.Polygon, DangerZoneRadius);

        // union any overlapping polygons then add the remainign polygons to the polygon collider
        List<Vector2> result = new List<Vector2>();
        PathsD dangerZone = Clipper.Union(inflatedPaths, FillRule.NonZero);
        col.pathCount = dangerZone.Count;
        for (int i = 0; i < dangerZone.Count; i++) {
            result.Clear();
            foreach (PointD point in dangerZone[i]) {
                result.Add(new Vector2((float)point.x, (float)point.y));
            }
            col.SetPath(i, result);
        }
    }

    /// <summary> creates a new path and returns the index of that path </summary>
    public int NewPath(Vector2[] points) {
        int index;
        if (freeStack.Count > 0) {
            index = freeStack.Pop();
        } else {
            index = paths.Count;
            paths.Add(emptyPolygon);
        }

        PathD path = new PathD();

        foreach (Vector2 point in points) {
            path.Add(new PointD(point.x, point.y));
        }

        paths[index] = path;

        UpdateCollider();

        return index;
    }

    /// <summary> frees(clears) the path at the index, allowing it to be used again later </summary>
    public void FreePath(int index) {
        if (index < paths.Count) {
            paths[index] = emptyPolygon;
            freeStack.Push(index);
            UpdateCollider();
        }
    }
}
