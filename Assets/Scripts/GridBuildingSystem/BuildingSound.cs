using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSound : MonoBehaviour {

    [SerializeField] private Transform pfBuildingSound = null;

    private void Start() {

        GridBuildingSystem2D.OnObjectPlaced += GridBuildingSystem2D_OnObjectPlaced;
    }

    private void GridBuildingSystem2D_OnObjectPlaced(PlacedObject_Done arg1, List<Vector2Int> arg2)
    {
        Transform buildingSoundTransform = Instantiate(pfBuildingSound, Vector3.zero, Quaternion.identity);
        Destroy(buildingSoundTransform.gameObject, 2f);
    }

}
