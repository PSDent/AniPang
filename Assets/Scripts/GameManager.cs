using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // 상수
    const int WIDTH = 7;
    const int HEIGHT = 7;

    const float CREATE_Y_POS = 0.8f;
    const float SPACE = 0.8f;
    const float START_X_POS = 2.4f;
    const float START_Y_POS = -2.9f;
    const float CREATE_DELAY = 0.3f;
    const float FADE_IN_DELAY = 0.001f;
    const float FADE_VALUE = 0.035f;

    // 0-Monkey, 1-Gigaffe, 2-Panda, 3-Penguin, 4-Rabbit, 5-Snake, 6-Pig
    enum AnimalType { Monkey, Giraffe, Panda, Penguin, Rabbit, Snake, Pig };

    public GameObject[] animal;
    GameObject[,] board;

    bool bStart = true;

    // Use this for initialization
    void Start()
    {
        // 동물 타일을 담을 2차원 배열을 할당
        board = new GameObject[HEIGHT, WIDTH];

        if (bStart)
            Create();
    }

    // 동물 타일을 게임 시작 전 생성한다. 
    void Create()
    {
        for (int i = 0; i < HEIGHT; ++i)
        {
            for (int j = 0; j < WIDTH; ++j)
            {
                int val = Random.Range(0, 7);
                Vector3 pos = new Vector3(j * SPACE - START_X_POS, CREATE_Y_POS * i + START_Y_POS, 0.0f);

                board[i, j] = Instantiate(animal[val], pos, Quaternion.identity);
                board[i, j].GetComponent<AnimalBox>().SetArrNumber(i, j);

                // 페이드 효과를 위해 알파값을 0으로 초기화 한다.
                Color color = board[i, j].GetComponent<SpriteRenderer>().color;

                board[i, j].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
            }
        }
        bStart = false;

        CheckOverlap();

        StartCoroutine("FadeIn");
    }

    // 중복된 동물타일을 없앤다.
    void CheckOverlap()
    {
        for (int i = 0; i < HEIGHT; ++i)
        {
            for (int j = 0; j < WIDTH; ++j)
            {
                bool bOverlap = false;

                if (i > 0 && i < HEIGHT - 1 && j > 0 && j < WIDTH - 1)
                {
                    if (board[i - 1, j].tag == board[i, j].tag && board[i + 1, j].tag == board[i, j].tag ||
                        board[i, j - 1].tag == board[i, j].tag && board[i, j + 1].tag == board[i, j].tag)
                    {
                        bOverlap = true;
                    }
                }
                else if ((i == 0 || i == HEIGHT - 1) && j > 0 && j > WIDTH - 1)
                {
                    if (board[i, j - 1].tag == board[i, j].tag && board[i, j + 1].tag == board[i, j].tag)
                    {
                        bOverlap = true;
                    }
                }
                else if (i > 0 && i < HEIGHT - 1 && (j == 0 || j == WIDTH - 1))
                {
                    if (board[i - 1, j].tag == board[i, j].tag && board[i + 1, j].tag == board[i, j].tag)
                    {
                        bOverlap = true;
                    }
                }

                // 해당 타일이 중복이 되었다면 
                if (bOverlap)
                {
                    // 해당 타일을 제거 후
                    int val = CheckTagWithEnum(board[i, j].tag);
                    DestroyObject(board[i, j]);

                    // 중복되지 않는 새로운 동물 타일로 교체한다. 
                    Vector3 pos = new Vector3(j * SPACE - START_X_POS, CREATE_Y_POS * i + START_Y_POS, 0.0f);
                    board[i, j] = Instantiate(animal[val], pos, Quaternion.identity);

                    // 새로 교체한 동물타일의 알파값을 다시 조정한다. 
                    Color color = board[i, j].GetComponent<SpriteRenderer>().color;
                    board[i, j].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
                    board[i, j].GetComponent<AnimalBox>().SetArrNumber(i, j);
                }

            }
        }
    }

    // 중복되는 타일의 태그를 Enum 을 이용하여 정수로 변환한다.
    int CheckTagWithEnum(string str)
    {
        AnimalType animalType = (AnimalType)System.Enum.Parse(typeof(AnimalType), str);
        int randAnimal;

        do
        {
            randAnimal = Random.Range(0, 7);
        } while ((int)animalType == randAnimal);
        Debug.Log("AnimalType : " + animalType);

        return randAnimal;
    }

    // 동물 타일에 페이드 효과를 준다.
    IEnumerator FadeIn()
    {
        while (board[6, 6].GetComponent<SpriteRenderer>().color.a < 1.0f)
        {
            for (int i = 0; i < HEIGHT; ++i)
            {
                for (int j = 0; j < WIDTH; ++j)
                {
                    board[i, j].GetComponent<SpriteRenderer>().color += new Color(0.0f, 0.0f, 0.0f, FADE_VALUE);
                }
                yield return new WaitForSeconds(FADE_IN_DELAY);
            }
        }
    }

    public GameObject[,] GetAnimalTile()
    {
        return board;
    }

    // 동물타일을 이동시킬 때마다 3개 이상의 짝이 존재하는지 검사한다.
    public bool CheckAnimal(int row, int column, int dirV, int dirH, string type)
    {
        Queue<GameObject> queueW = new Queue<GameObject>();
        Queue<GameObject> queueH = new Queue<GameObject>();
        bool L, R, U, D, result1, result2;
        result1 = result2 = L = R = U = D = false;

        queueW.Enqueue(board[row - dirV, column - dirH]);
        queueH.Enqueue(board[row - dirV, column - dirH]);

        // 맞는 짝을 상,하 탐색한다
        for (int i = 1; i < 7; ++i)
        {
            if (!R && column + i < 7 && board[row, column + i].tag == type)
                queueW.Enqueue(board[row, column + i]);
            else
                R = true;
            if (!L && column - i >= 0 && board[row, column - i].tag == type)
                queueW.Enqueue(board[row, column - i]);
            else
                L = true;
            if (!U && row + i < 7 && board[row + i, column].tag == type)
                queueH.Enqueue(board[row + i, column]);
            else
                U = true;
            if (!D && row - i >= 0 && board[row - i, column].tag == type)
                queueH.Enqueue(board[row - i, column]);
            else
                D = true;
        }

        Debug.Log("cnt1 : " + queueW.Count);
        Debug.Log("cnt2 : " + queueH.Count);

        if (queueW.Count >= 3)
        {
            while(queueW.Count > 0)
            {
                queueW.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            //    Destroy(queueW.Dequeue());
            result1 = true;
        }

        if (queueH.Count >= 3)
        {
            queueH.Dequeue();
            while (queueH.Count > 0)
            {
                queueH.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            result2 = true;
        }
        return result1 || result2;
    }

}
