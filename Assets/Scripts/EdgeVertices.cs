using UnityEngine;

public struct EdgeVertices
{
	public Vector3 v1, v2, v3, v4;

	public EdgeVertices (Vector3 corner1, Vector3 corner2)
    {
        v1 = corner1;
        v2 = Vector3.Lerp(corner1, corner2, 1f / 3f);
        v3 = Vector3.Lerp(corner1, corner2, 2f / 3f);
        v4 = corner2;
    }
}