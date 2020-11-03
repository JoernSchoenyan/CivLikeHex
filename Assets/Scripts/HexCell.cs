using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public HexGridChunk chunk = null;
    public RectTransform uiRect = null;

    [SerializeField]
    private HexCell[] neighbors;
    private TerrainType terrainType;

    private void Awake()
    {
        terrainType = (TerrainType)Random.Range(1, 5);
    }

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public bool HasRoads
    {
        get
        {
            return false;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public TerrainType TerrainType
    {
        get
        {
            return terrainType;
        }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return false;
    }

    private void Refresh()
    {
        if (chunk != null)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    private void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
}