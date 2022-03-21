using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper.Utils;

public class GridBuildingSystem2D : MonoBehaviour {

    public static GridBuildingSystem2D Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public static Action<PlacedObject_Done,List<Vector2Int>> OnObjectPlaced;
    public static Action<Vector3, bool> OnObjectRemoved;
    public static Action<Vector3,List<Vector2Int>> OnObjectMoved;
    public static Action<PlacedObjectTypeSO> OnObjectSelected;
    public static Action<PlacedObject_Done> OnPlacedObjectSelected;
    


    private Grid<GridObject> grid;
    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;

    private void Awake() {
        Instance = this;
        placedObjectTypeSO = null;
    }

    private void Start()
    {
        grid = GridManager.Instance.Grid;
    }

    private void OnEnable()
    {
        OnObjectPlaced += GridBuildingSystem2D_OnObjectPlaced;
        OnObjectRemoved += GridBuildingSystem2D_OnObjectRemoved;
        OnObjectSelected += GridBuildingSystem2D_OnObjectSelected;
        OnObjectMoved += GridBuildingSystem2D_OnObjectMoved;
    }

    

    private void OnDisable()
    {
        OnObjectPlaced -= GridBuildingSystem2D_OnObjectPlaced;
        OnObjectRemoved -= GridBuildingSystem2D_OnObjectRemoved;
        OnObjectSelected -= GridBuildingSystem2D_OnObjectSelected;
        OnObjectMoved -= GridBuildingSystem2D_OnObjectMoved;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);

            // Test Can Build
            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            bool canBuild = true;
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild) {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

                PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
                
                // Invoke OnObjectPlaced Event
                OnObjectPlaced?.Invoke(placedObject, gridPositionList);

                DeselectObjectType();
            } else {
                // Cannot build here
                UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }


        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
        
        if (Input.GetMouseButtonDown(1)) {
            //Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            //RemovePlacedObject(mousePosition);
            // Get placed object
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null) {
                // Invoke OnPlacedObjectSelected Event
                OnPlacedObjectSelected?.Invoke(placedObject);
            }
        }
    }

    private void RemovePlacedObject(Vector3 mousePosition, bool isUnit)
    {
        PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
        if (placedObject != null) {
            // Call action
            OnObjectRemoved?.Invoke(mousePosition, isUnit);
        }
    }
    
    private void DeselectObjectType() {
        placedObjectTypeSO = null; 
        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GridBuildingSystem2D_OnObjectSelected(PlacedObjectTypeSO obj)
    {
        placedObjectTypeSO = obj;
        RefreshSelectedObjectType();
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition() {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        grid.GetXY(mousePosition, out int x, out int y);

        if (placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } else {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectTypeSO != null) {
            return Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }
    
    private void GridBuildingSystem2D_OnObjectPlaced(PlacedObject_Done placedObject, List<Vector2Int> gridPositionList)
    {
        placedObject.transform.rotation = Quaternion.Euler(0, 0, 0);//-placedObjectTypeSO.GetRotationAngle(dir));

        foreach (Vector2Int gridPosition in gridPositionList) {
            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
        }
    }

    private void GridBuildingSystem2D_OnObjectRemoved(Vector3 mousePosition, bool isUnit)
    {
        PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
        if (placedObject != null) {
            // Demolish if not unit
            if(!isUnit) placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
        }
    }
    
    private void GridBuildingSystem2D_OnObjectMoved(Vector3 lastPos, List<Vector2Int> targetGridPositionList)
    {
        PlacedObject_Done placedObject = grid.GetGridObject(lastPos).GetPlacedObject();
        if (placedObject != null) {
            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                Debug.Log("Cleared placed object at " + gridPosition);
            }
            placedObject.transform.rotation = Quaternion.Euler(0, 0, 0);//-placedObjectTypeSO.GetRotationAngle(dir));

            foreach (Vector2Int gridPosition in targetGridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }
        }
    }
    

}
