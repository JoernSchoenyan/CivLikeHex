using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
	private Mesh hexMesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Color> colors;

	private MeshCollider meshCollider;

	private void Awake()
	{
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

    public void Triangulate(HexCell[] cells)
    {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();

		for (int i = 0; i < cells.Length; i++)
		{
			Triangulate(cells[i]);
		}

		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.RecalculateNormals();
		meshCollider.sharedMesh = hexMesh;
	}

	private void Triangulate(HexCell cell)
	{
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
			Triangulate(d, cell);
        }
	}

    private void Triangulate(HexDirection direction, HexCell cell)
    {
		Vector3 center = cell.Position;
		EdgeVertices e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(direction), center + HexMetrics.GetSecondSolidCorner(direction));

		TriangulateEdgeFan(center, e, cell.color);

		if (direction <= HexDirection.SE)
        {
			TriangulateConnection(direction, cell, e);
		}
	}

	private void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1)
    {
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null)
        {
			return;
        }

		Vector3 bridge = HexMetrics.GetBridge(direction);
		EdgeVertices e2 = new EdgeVertices(e1.v1 + bridge, e1.v4 + bridge);

		TriangulateEdgeStrip(e1, cell.color, e2, neighbor.color);

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null)
        {
			Vector3 v5 = e1.v4 + HexMetrics.GetBridge(direction.Next());
			TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
        }
	}

	private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
	{
		AddTriangle(center, edge.v1, edge.v2);
		AddTriangleColor(color);
		AddTriangle(center, edge.v2, edge.v3);
		AddTriangleColor(color);
		AddTriangle(center, edge.v3, edge.v4);
		AddTriangleColor(color);
	}

	private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
	{
		AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		AddQuadColor(c1, c2);
		AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		AddQuadColor(c1, c2);
		AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		AddQuadColor(c1, c2);
	}

	private void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
		AddTriangle(bottom, left, right);
		AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
	}

	private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	private void AddTriangleColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	private void AddTriangleColor(Color c1, Color c2, Color c3)
    {
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		vertices.Add(Perturb(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}

	private void AddQuadColor(Color c1, Color c2)
    {
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}

	private Vector3 Perturb(Vector3 position)
    {
		Vector4 sample = HexMetrics.SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;

		return position;
	}
}