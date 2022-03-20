using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectableObject : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSO placedObjectType;
    
    public PlacedObjectTypeSO PlacedObjectType
    {
        get => placedObjectType;
    }
    public bool isSelected { get; private set; } = false;
    
    // OnSelect action
    public static Action<GameObject> OnSelectableSelected;

    private void OnEnable()
    {
        // Subscribe to events
        OnSelectableSelected += SelectableObject_OnSelected;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        OnSelectableSelected -= SelectableObject_OnSelected;
    }

    /// <summary>
    /// Called with onSelected event
    /// All selectable objects become unselected
    /// </summary>
    void SelectableObject_OnSelected(GameObject selectedObj)
    {
        if(selectedObj == this.gameObject)
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }
    }

    /// <summary>
    /// Deselects all other selectables
    /// Selects this object
    /// </summary>
    private void OnMouseDown()
    {
        OnSelectableSelected?.Invoke(this.gameObject);
    }
}
