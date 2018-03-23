using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MariaP2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;
        transform.Translate(0, 0, y);
    }
}
