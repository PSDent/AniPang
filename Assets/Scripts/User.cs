using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class User : MonoBehaviour
{
    // 상수 
    const int INCREASE_SCORE = 100;
    const float TEXT_ALPHA_TIME = 0.01f;
    const float INCREASE_ALPHA = 0.05f;
    const float WAIT_TIME = 3.0f;
    const float FEVER_TIME = 3.0f;
    const float FLASH_DURATION = 0.1f;
    const int SCORE_INCREASE = 300;
    const float FEvER_INCREASE = 0.2f;
    public const float BOMB_INCREASE = 2.0f;

    GameManager gameMgr;
    Text scoreText;
    Text comboText;
    Image feverImage;
    Color flashColor;
    Color originFlashColor;

    bool bBombMode = false;
    bool bFeverMode = false;
    bool bFadeText = false;
    bool bStartedFever = false;

    float accumulate_FeverTime = 0;
    int score = 0;
    int targetScore = 0;
    int combo = 0;
    int nextCombo = 5;

    public float bombGage = 0;

    private void Awake()
    {
        scoreText = GameObject.Find("Canvas").transform.Find("ScoreBar")
    .Find("ScoreNum").GetComponent<Text>();

        comboText = GameObject.Find("Canvas").transform.Find("ScoreBar").Find("ComboText")
            .GetComponent<Text>();

        feverImage = GameObject.Find("Canvas").transform.Find("FeverImage")
            .GetComponent<Image>();

        gameMgr = GetComponent<GameManager>();

        flashColor = new Color(255, 255, 0, 255);
        originFlashColor = new Color(feverImage.color.r, feverImage.color.g, feverImage.color.b);

    }

    private void FixedUpdate()
    {
        if (score < targetScore)
            score += INCREASE_SCORE;
        else
            score = targetScore;

        if (combo >= nextCombo)
        {
            nextCombo += 5;
            accumulate_FeverTime += (combo * FEvER_INCREASE);
            if (!bFeverMode)
                StartCoroutine("FeverTimer");
        }

        ComboText();
        scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return targetScore;
    }

    public void Addition(int cnt)
    {
        AddBombGage(cnt * BOMB_INCREASE);
        // 콤보를 추가한다.
        AddCombo();
        // 짝이 맞는 동물타일의 개수만큼 점수를 올려준다.
        AddTargetScore(SCORE_INCREASE * cnt);
    }

    void AddTargetScore(int val)
    {
        score = targetScore;
        // 획득한 기본 점수에 콤보를 적용한다. 
        targetScore += val * combo;
    }

    void AddCombo()
    {
        if (!bFadeText)
            StartCoroutine("FadeInCombo");
        ++combo;
    }

    void AddBombGage(float val)
    {
        bombGage += val;
        if (bombGage >= 100)
        {
            bombGage = 0;
            gameMgr.DecideBomb();
        }
    }

    void ComboText()
    {
        comboText.text = combo + "\nCOMBO";
    }

    public bool IsFeverMode()
    {
        return bFeverMode;
    }

    void Hint(int x, int y)
    {
        Debug.Log("Hint : " + x + " " + y);
        if (gameMgr.GetAnimalTile()[y, x])
            gameMgr.GetAnimalTile()[y, x].GetComponent<AnimalBox>().SetCrossHair();
    }

    IEnumerator FeverTimer()
    {
        bFeverMode = true;
        StartCoroutine("FlashFever");

        while (accumulate_FeverTime > 0)
        {
            yield return new WaitForSeconds(FEVER_TIME);
            accumulate_FeverTime -= FEVER_TIME;
        }

        nextCombo = 5;
        bFeverMode = false;
    }

    // 콤보 텍스트를 Fade In 효과
    IEnumerator FadeInCombo()
    {
        bFadeText = true;
        while (comboText.color.a < 1.0f)
        {
            comboText.color += new Color(0, 0, 0, INCREASE_ALPHA);
            yield return new WaitForSeconds(TEXT_ALPHA_TIME);
        }

        Debug.Log("Fade In");
        StartCoroutine("IdleTimer");
    }

    // 콤보 텍스트를 Fade Out 효과
    IEnumerator FadeOutCombo()
    {
        while (comboText.color.a > 0.0f)
        {
            comboText.color -= new Color(0, 0, 0, INCREASE_ALPHA);
            yield return new WaitForSeconds(TEXT_ALPHA_TIME);
        }
        Debug.Log("Fade Out");
        combo = 0;

        Reference.POINT hintPoint = gameMgr.CheckThereIsAnswer();
        if(hintPoint.x > -1)
            Hint(hintPoint.x, hintPoint.y);
    }

    // 일정 시간이 지나도 콤보가 변동이 없는지 확인하기 위한 타이머 
    IEnumerator IdleTimer()
    {
        // 시간을 재기 전 콤보와 잰 후의 콤보가 상이한지 비교하여 
        // 콤보의 변동을 확인 함.

        int before = combo;
        yield return new WaitForSeconds(WAIT_TIME);
        int after = combo;

        if (before == after)
            StartCoroutine("FadeOutCombo");
        else
            StartCoroutine("IdleTimer");

        bFadeText = false;
    }

    // 피버모드일 시 화면 아래의 바가 점멸함.
    IEnumerator FlashFever()
    {
        Color color = new Color(feverImage.color.r, feverImage.color.g, feverImage.color.b);

        while (accumulate_FeverTime > 0)
        {
            feverImage.color = flashColor;
            yield return new WaitForSeconds(FLASH_DURATION);
            feverImage.color = originFlashColor;
            yield return new WaitForSeconds(FLASH_DURATION);
        }
    }
}
