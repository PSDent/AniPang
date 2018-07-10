using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReFiller : MonoBehaviour {

    GameObject[,] board;
   GameManager gameMgr;

	// Use this for initialization
	void Start () {
        gameMgr = GetComponent<GameManager>();
        board = gameMgr.GetAnimalTile();
    }
	
    void ReFill()
    {

    }
}
