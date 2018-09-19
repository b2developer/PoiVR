using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShittyMirrorToggle : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void Toggle()
    {
        obj.SetActive(!obj.activeInHierarchy);
    }
}
