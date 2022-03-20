using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper.Utils;

public class GridBuildingSystem2D : MonoBehaviour {

    public static GridBuildingSystem2D Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public static Action<PlacedObject_Done,List<Vector2Int>> OnObjectPlaced;
    public static Action<Vector3> OnObjectRemoved;


    private Grid<GridObject> grid;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
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
    }
    
    private void OnDisable()
    {
        OnObjectPlaced -= GridBuildingSystem2D_OnObjectPlaced;
        OnObjectRemoved -= GridBuildingSystem2D_OnObjectRemoved;
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
        
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            RemovePlacedObject(mousePosition);
        }
    }

    private void RemovePlacedObject(Vector3 mousePosition)
    {
        PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
        if (placedObject != null) {
            // Call action
            OnObjectRemoved?.Invoke(mousePosition);
        }
    }
    
    private void DeselectObjectType() {
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
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

    private void GridBuildingSystem2D_OnObjectRemoved(Vector3 mousePosition)
    {
        PlacedObject_Done placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
        if (placedObject != null) {
            // Demolish
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
        }
    }
    

}
