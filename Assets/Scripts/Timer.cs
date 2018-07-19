using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    float time = 60.0f;
    const float DECREASE_TIME = 0.1f;

    Slider slider;
    GameObject denyInput;
    GameObject ResultPanel;
    Text resultScore;
    User user;

	// Use this for initialization
	void Start () {
        slider = GameObject.Find("Canvas").transform.Find("Slider").GetComponent<Slider>();
        slider.maxValue = time;

        denyInput = GameObject.Find("DenyInput");
        denyInput.SetActive(false);

        ResultPanel = GameObject.Find("Canvas").transform.Find("Result").gameObject;
        ResultPanel.SetActive(false);

        resultScore = GameObject.Find("Canvas").transform.Find("Result").Find("Score").GetComponent<Text>();

        user = GameObject.Find("GameManager").GetComponent<User>();

        StartCoroutine("RunOutTime");
    }

    IEnumerator RunOutTime()
    {
        Color imageColor = slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color;

        // 시간에 따른 TimeBar의 색깔을 달리하여 플레이어에게 알림
        while (time > 0)
        {
            time -= DECREASE_TIME;
            slider.value = time;

            switch((int)time)
            {
                case 10:
                    slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color = new Color(1.0f, 0, 0);
                    break;

                case 20:
                    slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color = new Color(1.0f, 0.35f, 0);
                    break;

                case 30:
                    slider.transform.Find("Fill Area").Find("Fill").
                    GetComponent<Image>().color = new Color(1.0f, 0.5f, 0);
                    break;
            }

            yield return new WaitForSeconds(DECREASE_TIME);
        }
        denyInput.SetActive(true);
        ResultPanel.SetActive(true);
        resultScore.text = user.GetScore().ToString();
    }

}
