using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject {

    private Grid<GridObject> grid;
    public int x { get; private set; }
    public int y { get; private set; }
    public PlacedObject_Done placedObject;

    public GridObject(Grid<GridObject> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        placedObject = null;
    }
    
    public GridObject(int x, int y) {
        this.x = x;
        this.y = y;
        placedObject = null;
    }

    public override string ToString() {
        return x + ", " + y + "\n" + placedObject;
    }

    public void SetPlacedObject(PlacedObject_Done placedObject) {
        this.placedObject = placedObject;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void ClearPlacedObject() {
        placedObject = null;
        grid.TriggerGridObjectChanged(x, y);
    }

    public PlacedObject_Done GetPlacedObject() {
        return placedObject;
    }

    public bool CanBuild() {
        return placedObject == null;
    }

}