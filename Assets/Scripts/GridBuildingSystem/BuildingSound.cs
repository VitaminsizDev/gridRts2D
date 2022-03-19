using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSound : MonoBehaviour {

    [SerializeField] private GridBuildingSystem2D gridBuildingSystem2D = null;
    [SerializeField] private Transform pfBuildingSound = null;

    private void Start() {

        if (gridBuildingSystem2D != null) {
            gridBuildingSystem2D.OnObjectPlaced += GridBuildingSystem2D_OnObjectPlaced;
        }
    }

    private void GridBuildingSystem2D_OnObjectPlaced(object sender, System.EventArgs e) {
        Transform buildingSoundTransform = Instantiate(pfBuildingSound, Vector3.zero, Quaternion.identity);
        Destroy(buildingSoundTransform.gameObject, 2f);
    }

}
