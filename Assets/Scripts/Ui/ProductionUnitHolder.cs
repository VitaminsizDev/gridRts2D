using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionUnitHolder : MonoBehaviour
{
    [SerializeField] private PlacedObjectTypeSO _type;

    public void SelectObject()
    {
        GridBuildingSystem2D.OnObjectSelected?.Invoke(_type);
    }
}
