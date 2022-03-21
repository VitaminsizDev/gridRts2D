using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlacedObject_Done : MonoBehaviour {

    public static PlacedObject_Done Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO) {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));

        PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
        placedObject.Setup(placedObjectTypeSO, origin, dir);

        return placedObject;
    }

    public bool isSelected { get; private set; } = false;

    private void OnEnable()
    {
        // Subscribe to events
        GridBuildingSystem2D.OnPlacedObjectSelected += SelectableObject_OnPlacedObjectSelected;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        GridBuildingSystem2D.OnPlacedObjectSelected -= SelectableObject_OnPlacedObjectSelected;
    }

    /// <summary>
    /// Called with onSelected event
    /// All selectable objects become unselected
    /// </summary>
    void SelectableObject_OnPlacedObjectSelected([CanBeNull] PlacedObject_Done selectedObj)
    {
        if (selectedObj == this)
        {
            isSelected = true; 
            return;
        }
        isSelected = false;
    }


    private PlacedObjectTypeSO placedObjectTypeSO;
    public PlacedObjectTypeSO PlacedObjectTypeSO { get { return placedObjectTypeSO; } }
    private Vector2Int origin;
    public PlacedObjectTypeSO.Dir dir;

    private void Setup(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int origin, PlacedObjectTypeSO.Dir dir) {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.origin = origin;
        this.dir = dir;
    }
    
    public void SetOrigin(Vector2Int origin) {
        this.origin = origin;
    }

    public List<Vector2Int> GetGridPositionList() {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public override string ToString() {
        return placedObjectTypeSO.nameString;
    }

}
