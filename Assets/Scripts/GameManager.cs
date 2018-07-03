using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    const int WIDTH = 7;
    const int HEIGHT = 7;
    const float CREATE_Y_POS = 2f;
    const float SPACE = 0.8f;
    const float START_Y_POS = 2.4f;
    const float CREATE_DELAY = 0.3f;

    public GameObject[] animal;
    GameObject[,] board;

    bool bStart = true;

	// Use this for initialization
	void Start () {
        board = new GameObject[HEIGHT,WIDTH];

        if (bStart)
            StartCoroutine("Create");
    }
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator Create()
    {
        for (int i = 0; i < HEIGHT; ++i)
        {
            for (int j = 0; j < WIDTH; ++j)
            {
                int val = Random.Range(0, 7);
                Vector3 pos = new Vector3(j * SPACE - START_Y_POS, CREATE_Y_POS, 0.0f);
                board[i, j] = Instantiate(animal[val], pos, Quaternion.identity);
            }
            yield return new WaitForSeconds(CREATE_DELAY);
        }
        bStart = false;
    }
}
