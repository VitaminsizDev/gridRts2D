using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectableObject : MonoBehaviour
{
    
    public bool isSelected { get; private set; } = false;
    
    // OnSelect action
    public static Action onSelected;

    private void OnEnable()
    {
        // Subscribe to events
        onSelected += AnotherObjectSelected;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        onSelected -= AnotherObjectSelected;
    }

    /// <summary>
    /// Called with onSelected event
    /// All selectable objects become unselected
    /// </summary>
    void AnotherObjectSelected()
    {
        isSelected = false;
    }

    /// <summary>
    /// Deselects all other selectables
    /// Selects this object
    /// </summary>
    private void OnMouseDown()
    {
        onSelected?.Invoke();
        isSelected = true;
    }
}
