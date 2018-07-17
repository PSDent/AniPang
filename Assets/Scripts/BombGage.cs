using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombGage : MonoBehaviour
{
    // 폭탄 사양 구현할 것.
    const float BOMBGAGE_MAX = 100.0f;
   
    Slider slider;
    User user;

    void Start()
    {
        user = GameObject.Find("GameManager").GetComponent<User>();

        slider = GameObject.Find("Canvas").transform.Find("ScoreBar")
            .Find("BombBar").Find("Slider").GetComponent<Slider>();
        slider.maxValue = BOMBGAGE_MAX;
    }

    private void FixedUpdate()
    {
        slider.value = user.bombGage;
    }

}
