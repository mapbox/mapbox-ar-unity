using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityARInterface;

public class PlaceMapOnARPlane : MonoBehaviour {

    [SerializeField]
    Transform _mapTransform;

	// Use this for initialization
	void Start () 
    {
        ARPlaneHandler.returnARPlane += PlaceMap;
        ARPlaneHandler.resetARPlane += ResetPlane;
	}

    void PlaceMap(BoundedPlane plane)
    {
        if(!_mapTransform.gameObject.activeSelf)
        {
            _mapTransform.gameObject.SetActive(true);
        }
        _mapTransform.position = plane.center;
    }

    void ResetPlane()
    {
        _mapTransform.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ARPlaneHandler.returnARPlane -= PlaceMap;
    }

}
