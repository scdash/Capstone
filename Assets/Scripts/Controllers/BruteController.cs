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
        if (Input.GetButtonDown("Jump")) {
            anim.SetTrigger("isJumping");
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            anim.SetTrigger("isFore");
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            anim.SetTrigger("isBack");
        }
        if(Input.GetKeyUp(KeyCode.RightArrow)) {
            anim.SetTrigger("stopFore");
        } else if (Input.GetKeyUp(KeyCode.LeftArrow)){
            anim.SetTrigger("stopBack");
        }
    }
}
