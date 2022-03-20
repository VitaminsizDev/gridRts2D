using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    public static Action<SelectableObject> OnSelectableSelected;

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
    void SelectableObject_OnSelected([CanBeNull] SelectableObject selectedObj)
    {
        if (selectedObj == null)
        {
            isSelected = false; 
            return;
        }
        
        if(selectedObj.gameObject == this.gameObject)
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
        OnSelectableSelected?.Invoke(this);
    }
}
