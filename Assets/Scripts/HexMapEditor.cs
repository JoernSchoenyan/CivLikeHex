using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
	public HexGrid hexGrid;

	private HexCell currentCell, searchFromCell;
	private bool editMode = false;

	private void Update()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
		}
	}

	public void SetEditMode(bool toggle)
    {
		editMode = toggle;
		hexGrid.ShowUI(!toggle);
    }

	private void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			currentCell = hexGrid.GetCell(hit.point);



		}

		if (Input.GetKey(KeyCode.LeftShift))
        {
			if (searchFromCell)
            {
				searchFromCell.DisableHighlight();
            }
			searchFromCell = currentCell;
			searchFromCell.EnableHighlight(Color.blue);
        }
	}

    private void EditCell(HexCell cell)
    {
		//cell.Color = activeColor;
    }
}