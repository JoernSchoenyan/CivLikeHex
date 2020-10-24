﻿using UnityEngine;

public class HexGrid : MonoBehaviour
{
	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;

	public Color defaultColor = Color.blue;
	public Color touchedColor = Color.magenta;

	HexCell[] cells;
	HexMesh hexMesh;

	private void Awake()
	{
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

    private void Start()
    {
		hexMesh.Triangulate(cells);
    }

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			HandleInput();
		}
	}

	private void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			TouchCell(hit.point);
		}
	}

	private void TouchCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = touchedColor;
		hexMesh.Triangulate(cells);
	}

	private void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;

		if (x > 0)
        {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
		if (z > 0)
        {
			if ((z & 1) == 0)
            {
				cell.SetNeighbor(HexDirection.SE, cells[i - width]);
				if (x > 0)
                {
					cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
			else
            {
				cell.SetNeighbor(HexDirection.SW, cells[i - width]);
				if (x < width - 1)
                {
					cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }
	}
}