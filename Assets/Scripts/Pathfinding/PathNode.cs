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

public class PathNode : GridObject{

    private Grid<PathNode> grid;
    
    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y) : base(x, y)
    {
        this.grid = grid;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetPlacedObject(PlacedObject_Done placedObject) {
        this.placedObject = placedObject;
        grid.TriggerGridObjectChanged(x, y);
    }
    
    public void ClearPlacedObject() {
        placedObject = null;
        grid.TriggerGridObjectChanged(x, y);
    }
}
