/* 
    --------------------------------------------------
    This code was built on top of CodeMonkey's Pathfinding tutorial.
    Check his version at: unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using Helper.Utils;
using TMPro;
using UnityEngine;

public class PathfindingVisual : MonoBehaviour {

    private Grid<PathNode> grid;
    private Mesh mesh;
    private bool updateMesh;
    
    // Visual
    private Transform[,] visualNodeArray; 
    [SerializeField] private Transform pfPathfindingDebugStepVisualNode;
    private List<Transform> visualNodeList;
    [SerializeField] private bool showPath = true;
    private float autoShowSnapshotsTimer;
    [SerializeField]private Transform gridVisualsParent;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        visualNodeList = new List<Transform>();
    }

    public void SetGrid(Grid<PathNode> grid) {
        this.grid = grid;
        UpdateVisual();

        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, Grid<PathNode>.OnGridObjectChangedEventArgs e) {
        updateMesh = true;
    }

    private void LateUpdate() {
        if (updateMesh) {
            updateMesh = false;
            UpdateVisual();
        }
    }

    public void DrawGridVisual(Grid<PathNode> grid) {
        visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                Vector3 gridPosition = new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                Transform visualNode = CreateVisualNode(gridPosition);
                visualNodeArray[x, y] = visualNode;
                visualNodeList.Add(visualNode);
                SetupVisualNode(visualNode, new Vector2Int(x, y), 0, grid);
            }
        }
    }
    
    private void UpdateVisual() {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                PathNode pathNode = grid.GetGridObject(x, y);

                if (pathNode.GetPlacedObject() == null) {
                    quadSize = Vector3.zero;
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, Vector2.zero, Vector2.zero);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    
    private Transform CreateVisualNode(Vector3 position) {
        Transform visualNodeTransform = Instantiate(pfPathfindingDebugStepVisualNode, position, Quaternion.identity);
        return visualNodeTransform;
    }

    private void SetupVisualNode(Transform visualNodeTransform, Vector2Int gridPosition, int val, Grid<PathNode> grid) {
        // Setup parent
        visualNodeTransform.parent = gridVisualsParent;
        // Write the coordinates of the node to the text
        visualNodeTransform.Find("nodeCoord").GetComponent<TextMeshPro>().SetText("[" + gridPosition.x.ToString() + ", " + gridPosition.y.ToString() + "]");
        // Setup node scale
        visualNodeTransform.localScale = Vector3.one * grid.GetCellSize() * .1f;
    }
}

