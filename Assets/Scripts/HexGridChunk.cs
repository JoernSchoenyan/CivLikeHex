using System;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    private HexCell[] cells;
    private HexMesh hexMesh;

    private void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    private void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        enabled = false;
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
    }

    public void Refresh()
    {
        enabled = true;
    }
}
