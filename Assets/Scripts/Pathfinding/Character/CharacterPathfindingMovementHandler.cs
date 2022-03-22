using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper.Utils;

public class CharacterPathfindingMovementHandler : PlacedObject_Done {

    private const float speed = 40f;

    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private LineRenderer lineRenderer;
    
    private Vector3 lastPosition;
    

    private void Start() {
        pathVectorList = null;
        Transform bodyTransform = transform.Find("Body");
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lastPosition = transform.position;
    }

    private void Update() {
        HandleMovement();

        if (Input.GetMouseButtonDown(0) && isSelected && !UtilsClass.IsPointerOverUI()) {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            SetTargetPosition(mouseWorldPosition);
        }
    }
    
    /// <summary>
    /// Moves to the target node by node with given speed
    /// </summary>
    private void HandleMovement() {
        if (pathVectorList == null)
        {
            return;
        }

        Vector3 targetPosition = pathVectorList[currentPathIndex];
        if (Vector3.Distance(transform.position, targetPosition) > 1f) {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            transform.position += moveDir * speed * Time.deltaTime;
        } else {
            currentPathIndex++;
            if (currentPathIndex >= pathVectorList.Count) {
                StopMoving();
                lineRenderer.positionCount = 0;
            }
        }
    }
    
    /// <summary>
    /// Reset the move list when reached target
    /// </summary>
    private void StopMoving() {
        pathVectorList = null;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    /// <summary>
    /// Finds the path to follow
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        // Find path, if path found then set the pathVectorList
        // If path not found, pathVectorList will be null
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            
            // Call object placed event
            Vector2Int rotationOffset = PlacedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = targetPosition;//GridManager.Instance.Grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
            GridManager.Instance.Grid.GetXY(targetPosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);

            
            List<Vector2Int> gridPositionList = PlacedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
            
            GridBuildingSystem2D.OnObjectMoved?.Invoke(lastPosition, gridPositionList);
            this.SetOrigin(placedObjectOrigin);
            DrawWalkPath(pathVectorList);
            pathVectorList.RemoveAt(0);
            
            lastPosition = targetPosition;
        }
    }

    /// <summary>
    /// Draws the path to follow with line renderer
    /// </summary>
    /// <param name="path"></param>
    private void DrawWalkPath(List<Vector3> path)
    {
        lineRenderer.positionCount = path.Count;
        GridManager.Instance.Grid.GetXY(this.transform.position, out int currentX, out int currentY);
        if (path != null) {
            var firstPos = GridManager.Instance.Grid.GetWorldPosition(currentX, currentY) + Vector3.one * GridManager.Instance.Grid.GetCellSize() * .5f;
            firstPos.z = 0;
            lineRenderer.SetPosition(0, firstPos);
            for (int i=1; i<=path.Count - 1; i++)
            {
                var lineRenderTarget = path[i];
                lineRenderTarget.z = 0;
                lineRenderer.SetPosition(i, lineRenderTarget);
            }
        }
    }
}