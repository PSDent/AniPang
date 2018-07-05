using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour {

    bool bClicked = false;

    void CheckClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            bClicked = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            bClicked = false;
        }
    }

    //// Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    void Update()
    {
        CheckClick();
    }
}
