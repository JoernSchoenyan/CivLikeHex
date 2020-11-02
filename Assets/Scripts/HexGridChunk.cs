using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    public HexFeatureManager features;
    public HexMesh terrain;

    private HexCell[] cells;
	private Canvas gridCanvas;
	private static Color color1 = new Color(1f, 0f, 0f);
	private static Color color2 = new Color(0f, 1f, 0f);
	private static Color color3 = new Color(0f, 0f, 1f);

	private void Awake()
    {
        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];

		gridCanvas = GetComponentInChildren<Canvas>();
		//ShowUI(false);
	}

	public void AddCell(int index, HexCell cell)
	{
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}

	public void Refresh()
	{
		enabled = true;
	}

	public void ShowUI(bool visible)
    {
		gridCanvas.gameObject.SetActive(visible);
    }

	public void Triangulate()
	{
		terrain.Clear();
		features.Clear();

		for (int i = 0; i < cells.Length; i++)
		{
			Triangulate(cells[i]);
		}

		terrain.Apply();
		features.Apply();
	}

	private void Triangulate(HexCell cell)
	{
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
		{
			Triangulate(d, cell);
		}
		if (!cell.HasRoads)
        {
			features.AddFeature(cell.Position, cell.TerrainType);
		}
	}

	private void Triangulate(HexDirection direction, HexCell cell)
	{
		Vector3 center = cell.Position;
		EdgeVertices e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(direction), center + HexMetrics.GetSecondSolidCorner(direction));

		TriangulateEdgeFan(center, e, (float)cell.TerrainType);

		if (direction <= HexDirection.SE)
		{
			TriangulateConnection(direction, cell, e);
		}

		if (!cell.HasRoadThroughEdge(direction))
        {
			features.AddFeature((center + e.v1 + e.v5) * (1f / 3f), cell.TerrainType);
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
		EdgeVertices e2 = new EdgeVertices(e1.v1 + bridge, e1.v5 + bridge);

		TriangulateEdgeStrip(e1, color1, (float)cell.TerrainType, e2, color2, (float)neighbor.TerrainType);

		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null)
		{
			Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
			TriangulateCorner(v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor);
		}
	}

	private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, float type)
	{
		Vector3 types;
		types.x = types.y = types.z = type;
		
		terrain.AddTriangle(center, edge.v1, edge.v2);
		terrain.AddTriangleColor(color1);
		terrain.AddTriangleTerrainTypes(types);
		terrain.AddTriangle(center, edge.v2, edge.v3);
		terrain.AddTriangleColor(color1);
		terrain.AddTriangleTerrainTypes(types);
		terrain.AddTriangle(center, edge.v3, edge.v4);
		terrain.AddTriangleColor(color1);
		terrain.AddTriangleTerrainTypes(types);
		terrain.AddTriangle(center, edge.v4, edge.v5);
		terrain.AddTriangleColor(color1);
		terrain.AddTriangleTerrainTypes(types);
	}

	private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, float type1, EdgeVertices e2, Color c2, float type2)
	{
		Vector3 types;
		types.x = types.z = type1;
		types.y = type2;

		terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuadTerrainTypes(types);
		terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuadTerrainTypes(types);
		terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuadTerrainTypes(types);
		terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuadTerrainTypes(types);
	}

	private void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
	{
		Vector3 types;
		types.x = (float)bottomCell.TerrainType;
		types.y = (float)leftCell.TerrainType;
		types.z = (float)rightCell.TerrainType;

		terrain.AddTriangle(bottom, left, right);
		terrain.AddTriangleColor(color1, color2, color3);
		terrain.AddTriangleTerrainTypes(types);
	}

	private void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }
}