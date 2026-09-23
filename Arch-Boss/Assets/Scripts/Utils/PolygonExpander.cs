using System.Collections.Generic;
using UnityEngine;

public static class PolygonExpander {
    /// <summary> adds a rectangular Minkowski offset to the supplied polygon and returns the result </summary>
    public static List<Vector2> AddRectOffset(Vector2[] points, float xOffset, float yOffset) {
        List<Vector2> result = new();

        if (points.Length < 3) {
            Debug.LogWarning("A polygon must have at least 3 points");
            return result;
        }
        if (xOffset <= 0f || yOffset <= 0f) {
            Debug.LogWarning("Rect offset must be more than 0 in every dimension");
            return result;
        }

        float area = GetSignedArea(points);
        // -1f means clockwise, +1f means anti-clockwise
        int winding;

        if (Mathf.Abs(area) < 0.000001f) {
            Debug.LogWarning("Supplied polygon must have some area");
            return result;
        } else if (area > 0f) {
            winding = 1;
        } else {
            winding = -1;
        }

        Vector2[] rectOffsets = new Vector2[4] {
            new(-xOffset, -yOffset), // bottom left
            new(-xOffset, yOffset), // top left
            new(xOffset, yOffset), // top right
            new(xOffset, -yOffset) // bottom right
        };

        // small optimizations possible here by caching the calculations for next and using it for prev later
        for (int i = 0; i < points.Length; i++) {
            Vector2 prev = points[Mod(i - 1, points.Length)];
            Vector2 cur = points[i];
            Vector2 next = points[Mod(i + 1, points.Length)];

            Vector2 prevToCur = cur - prev;
            Vector2 curToNext = next - cur;

            bool isConvex = (prevToCur.x * curToNext.y - prevToCur.y * curToNext.x) * winding >= 0;
            // get the appropriate offsets needed to expand the polygon
            int curIndex = Mod(VectorToQuadrant(prevToCur) + winding, rectOffsets.Length);
            int endIndex = Mod(VectorToQuadrant(curToNext) + winding, rectOffsets.Length);

            if (isConvex) {
                // add the offsets from cur to end inclusive
                for (int j = 0; j < rectOffsets.Length; j++) {
                    result.Add(cur + rectOffsets[curIndex]);
                    curIndex = Mod(curIndex - winding, rectOffsets.Length);
                    if (curIndex == endIndex) {
                        result.Add(cur + rectOffsets[curIndex]);
                        break;
                    }
                }
            } else { // resolve intersecting offset lines.
                Vector2 intersection = GetIntersection(
                    cur + rectOffsets[curIndex], prevToCur,
                    cur + rectOffsets[endIndex], curToNext
                    );
                result.Add(intersection);
            }
        }
        return result;
    }

    private static int Mod(int value, int modulus) {
        return (value % modulus + modulus) % modulus;
    }

    private static float GetSignedArea(Vector2[] points) {
        float area = 0;
        // shoelace method 
        for (int i = 0; i < points.Length; i++) {
            Vector2 current = points[i];
            Vector2 next = points[Mod(i + 1, points.Length)];

            area += current.x * next.y - next.x * current.y;
        }

        return 0.5f * area;
    }

    private static Vector2 GetIntersection(Vector2 pointA, Vector2 directionA, Vector2 pointB, Vector2 directionB) {
        float cross = directionA.x * directionB.y - directionA.y * directionB.x;
        float lambda = ((pointB - pointA).x * directionB.y - (pointB - pointA).y * directionB.x) / cross;

        return pointA + lambda * directionA;
    }

    /// <summary> gets the quadrant the vector is in, clockwise is increasing index<br/>
    /// minus this with winding direction to get index of closest offset corner outside of polygon </summary>
    private static int VectorToQuadrant(Vector2 vector) {
        // direction of vector MATTERS. negative will flip the quadrant

        // 1 | 2
        // --+--
        // 0 | 3

        if (vector.x < 0 && vector.y < 0) {
            return 0;
        }

        if (vector.x < 0 && vector.y >= 0) {
            return 1;
        }

        if (vector.x >= 0 && vector.y >= 0) {
            return 2;
        }

        return 3;
    }
}

