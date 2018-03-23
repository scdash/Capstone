using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteP1 : MonoBehaviour {
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 5.0f;
        transform.Translate(0, 0, x);
    }
}
