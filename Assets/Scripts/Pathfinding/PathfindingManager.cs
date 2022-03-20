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
    }
    
    private void OnDisable()
    {
        GridBuildingSystem2D.OnObjectPlaced -= Pathfinding_OnObjectPlaced;
        GridBuildingSystem2D.OnObjectRemoved -= Pathfinding_OnObjectRemoved;
    }

    private void Pathfinding_OnObjectPlaced(PlacedObject_Done placedObject, List<Vector2Int> gridPositionList)
    {
        foreach (Vector2Int gridPosition in gridPositionList) {
            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
        }
    }
    
    private void Pathfinding_OnObjectRemoved(Vector3 mousePosition)
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
