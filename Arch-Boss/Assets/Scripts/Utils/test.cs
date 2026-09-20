using UnityEngine;

public class test : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector2[] Input = new Vector2[0];
    public DangerZone Zone;

    [ContextMenu("calc")]
    void Run() {
        string output = "polygon(";
        foreach (var point in PolygonExpander.AddRectOffset(Input, 1f, 1f)) {
            output += $"({point.x},{point.y}),";
        }
        Debug.Log(output.Substring(0, output.Length - 1) + ")");
        Zone.NewPath(Input);
    }
}
