using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    public static GridManager Instance { get; private set; }
    
    [SerializeField] private Transform gridParent;

    private Grid<GridObject> grid;

    public Grid<GridObject> Grid
    {
        get
        {
            return grid;
        }
    }

    private void Awake()
    {
        // Singleton
        Instance = this;
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), gridParent, (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        
    }
    
    
}
