using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteController : MonoBehaviour {

    static Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Jump")) {
            anim.SetTrigger("isJumping");
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            anim.SetTrigger("isFore");
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            anim.SetTrigger("isBack");
        }
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 5.0f;
        transform.Translate(0, 0, x);
    }
}
