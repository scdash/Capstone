using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MariaController : MonoBehaviour {

    static Animator anim;

    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Jump")) {
            anim.SetTrigger("isJump");
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            anim.SetTrigger("isFore");
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            anim.SetTrigger("isBack");
        }
        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            anim.SetTrigger("stopFore");
        } else if (Input.GetKeyUp(KeyCode.DownArrow)) {
            anim.SetTrigger("stopBack");
        }
    }
}
