using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour {
    private PolygonCollider2D col;
    private Stack<int> freeStack = new Stack<int>();
    private readonly Vector2[] emptyPolygon = {Vector2.zero, Vector2.zero, Vector2.zero};
    private void Start() {
        col = GetComponent<PolygonCollider2D>();
    }

    /// <summary> creates a new path and returns the index of that path </summary>
    public int NewPath(Vector2[] points) {
        int index;
        if (freeStack.Count > 0) { 
            index = freeStack.Pop(); 
        } else {
            index = col.pathCount++;
        }
        col.SetPath(index, points);
        return index;
    }

    /// <summary> frees(clears) the path at the index, allowing it to be used again later </summary>
    public void FreePath(int index) {
        if (index < col.pathCount) {
            col.SetPath(index, emptyPolygon);
            freeStack.Push(index);
        }
    }
}
