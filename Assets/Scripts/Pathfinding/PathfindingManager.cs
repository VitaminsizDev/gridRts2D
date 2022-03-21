using System;
using System.Collections;
using System.Collections.Generic;
using Helper.Utils;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    // Singleton
    public static PathfindingManager Instance { get; private set; }
    
    [SerializeField] private Transform pathfindingParent;
    
    public Pathfinding pathfinding { get; private set; }
    
    private Grid<PathNode> grid;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        // Setup grid with pathfinding
        pathfinding = new Pathfinding(GridManager.Instance.Grid, pathfindingParent);
        grid = pathfinding.GetGrid();
    }

    private void OnEnable()
    {
        GridBuildingSystem2D.OnObjectPlaced += Pathfinding_OnObjectPlaced;
        GridBuildingSystem2D.OnObjectRemoved += Pathfinding_OnObjectRemoved;
        GridBuildingSystem2D.OnObjectMoved += Pathfinding_OnObjectMoved;
    }
    
    private void OnDisable()
    {
        GridBuildingSystem2D.OnObjectPlaced -= Pathfinding_OnObjectPlaced;
        GridBuildingSystem2D.OnObjectRemoved -= Pathfinding_OnObjectRemoved;
        GridBuildingSystem2D.OnObjectMoved -= Pathfinding_OnObjectMoved;
    }

    private void Pathfinding_OnObjectPlaced(PlacedObject_Done placedObject, List<Vector2Int> gridPositionList)
    {
        foreach (Vector2Int gridPosition in gridPositionList) {
            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
        }
    }
    
    private void Pathfinding_OnObjectRemoved(Vector3 mousePosition, bool isUnit)
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
    
    private void Pathfinding_OnObjectMoved(Vector3 lastPos, List<Vector2Int> targetGridPositionList)
    {
        PlacedObject_Done placedObject = grid.GetGridObject(lastPos).GetPlacedObject();
        if (placedObject != null) {
            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
            placedObject.transform.rotation = Quaternion.Euler(0, 0, 0);//-placedObjectTypeSO.GetRotationAngle(dir));

            foreach (Vector2Int gridPosition in targetGridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }
        }
    }
}
