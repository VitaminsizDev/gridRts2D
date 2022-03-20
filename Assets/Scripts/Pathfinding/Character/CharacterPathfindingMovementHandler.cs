/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using V_AnimationSystem;
using Helper.Utils;

public class CharacterPathfindingMovementHandler : SelectableObject {

    private const float speed = 40f;

    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private AnimatedWalker animatedWalker;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    private LineRenderer lineRenderer;
    

    private void Start() {
        pathVectorList = null;
        Transform bodyTransform = transform.Find("Body");
        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
        unitAnimation = new V_UnitAnimation(unitSkeleton);
        animatedWalker = new AnimatedWalker(unitAnimation, UnitAnimType.GetUnitAnimType("dMarine_Idle"), UnitAnimType.GetUnitAnimType("dMarine_Walk"), 1f, 1f);
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void Update() {
        HandleMovement();
        unitSkeleton.Update(Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && isSelected && !UtilsClass.IsPointerOverUI()) {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            SetTargetPosition(mouseWorldPosition);
        }
    }
    
    private void HandleMovement() {
        if (pathVectorList == null)
        {
            animatedWalker.SetMoveVector(Vector3.zero);
            return;
        }

        Vector3 targetPosition = pathVectorList[currentPathIndex];
        if (Vector3.Distance(transform.position, targetPosition) > 1f) {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            animatedWalker.SetMoveVector(moveDir);
            transform.position += moveDir * speed * Time.deltaTime;
        } else {
            currentPathIndex++;
            if (currentPathIndex >= pathVectorList.Count) {
                StopMoving();
                animatedWalker.SetMoveVector(Vector3.zero);
                lineRenderer.positionCount = 0;
            }
        }
    }

    private void StopMoving() {
        pathVectorList = null;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        // Find path, if path found then set the pathVectorList
        // If path not found, pathVectorList will be null
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1) {
            DrawWalkPath(pathVectorList);
            pathVectorList.RemoveAt(0);
        }
    }

    private void DrawWalkPath(List<Vector3> path)
    {
        lineRenderer.positionCount = path.Count;
        if (path != null) {
            for (int i=0; i<=path.Count - 1; i++) {
                var lineRenderTarget = path[i];
                lineRenderTarget.z = 0;
                lineRenderer.SetPosition(i, lineRenderTarget);
            }
        }
    }
}