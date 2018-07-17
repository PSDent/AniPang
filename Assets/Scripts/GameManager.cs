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
    public const float TIMING = 0.2f;
    const int SCORE_INCREASE = 300;

    // 0-Monkey, 1-Gigaffe, 2-Panda, 3-Penguin, 4-Rabbit, 5-Snake, 6-Pig
    enum AnimalType { Monkey, Giraffe, Panda, Penguin, Rabbit, Snake, Pig };

    public GameObject[] animal;
    GameObject[,] board;
    User user;

    bool bReFilling = false;
    bool bDropping = false;
    bool bStart = true;

    // Use this for initialization
    void Start()
    {
        user = GetComponent<User>();
        // 동물 타일을 담을 2차원 배열을 할당
        board = new GameObject[HEIGHT, WIDTH];

        if (bStart)
            Create();
    }

    public bool IsDropping()
    {
        return bDropping;
    }

    // 동물 타일을 게임 시작 전 생성한다. 
    void Create()
    {
        for (int i = 0; i < HEIGHT; ++i)
        {
            for (int j = 0; j < WIDTH; ++j)
            {
                CreateAnimalTile(j, i, true);
            }
        }
        bStart = false;

        CheckOverlap();
        StartCoroutine("FadeIn");
    }

    void CreateAnimalTile(int x, int y, bool alphaOn)
    {
        int val = Random.Range(0, 7);
        Vector3 pos = new Vector3(x * SPACE - START_X_POS, CREATE_Y_POS * y + START_Y_POS, 0.0f);

        board[y, x] = Instantiate(animal[val], pos, Quaternion.identity);
        board[y, x].GetComponent<AnimalBox>().SetArrNumber(y, x);

        // 페이드 효과를 위해 알파값을 0으로 초기화 한다.
        if (alphaOn)
        {
            Color color = board[y, x].GetComponent<SpriteRenderer>().color;
            board[y, x].GetComponent<SpriteRenderer>().color = new Vector4(color.r, color.g, color.b, 0.0f);
        }
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
                else if ((i == 0 || i == HEIGHT - 1) && j > 0 && j < WIDTH - 1)
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
                    int val = CheckTagWithEnum(board[i, j].tag, j, i);
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
    int CheckTagWithEnum(string str, int x, int y)
    {
        int[] cntAnimal = new int[7];
        AnimalType animalType = (AnimalType)System.Enum.Parse(typeof(AnimalType), str);
        int randAnimal;

        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                int tx = x + j, ty = y + i;

                if (tx < 7 && tx >= 0 && ty < 7 && ty >= 0)
                {
                    int index = (int)System.Enum.Parse(typeof(AnimalType), board[ty, tx].tag);
                    ++cntAnimal[index];
                }
            }
        
        }

        do
        {
            randAnimal = Random.Range(0, 7);
        } while (cntAnimal[randAnimal] > 0);
        return randAnimal;
    }

    private void FixedUpdate()
    {
        if(!bReFilling)
            StartCoroutine("Refill");
    }

    public GameObject[,] GetAnimalTile()
    {
        return board;
    }

    // 동물타일을 떨어뜨린다
    public void DropAnimals()
    {
        bDropping = true;
        for (int i = 0; i < HEIGHT; ++i)
        {
            for (int j = 0; j < WIDTH; ++j)
            {
                if(board[i,j])
                    board[i, j].GetComponent<AnimalBox>().StartCoroutine("Drop");
            }
        }
        StartCoroutine("Refill");
    }

    // 피버모드를 적용한다.
    void FeverMode(Queue<GameObject> queue)
    {
        // 본래 짝이 맞는 동물타일이 들어있는 큐를 복사
        Queue<GameObject> tempQueue = new Queue<GameObject>(queue);

        // 복사된 큐를 dequeue 하면서 상하좌우의 타일을 모두 없앤다.
        while(tempQueue.Count > 0)
        {
            int row = 0, column = 0;
            tempQueue.Dequeue().GetComponent<AnimalBox>().GetArrNumber(ref row, ref column);

            if (row - 1 >= 0 && board[row - 1, column])
                board[row - 1, column].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            if (row + 1 < HEIGHT && board[row + 1, column])
                board[row + 1, column].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            if (column - 1 >= 0 && board[row, column - 1])
                board[row, column - 1].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            if (column + 1 < WIDTH && board[row, column + 1])
                board[row, column + 1].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
        }
    }

    //
    // 여기에 무언가 버그가 하나 있는 듯 하니 여유 있으면 고치기 바람. 
    //
    // 동물타일을 이동시킬 때마다 3개 이상의 짝이 존재하는지 검사한다.
    public bool CheckAnimal(int row, int column, int dirV, int dirH, string type)
    {
        Queue<GameObject> queueW = new Queue<GameObject>();
        Queue<GameObject> queueH = new Queue<GameObject>();
        bool L, R, U, D, result1, result2;
        result1 = result2 = L = R = U = D = false;

        if (row + (dirV * -1) >= 0 && row + (dirV * -1) < HEIGHT &&
            column + (dirH * -1) >= 0 && column + (dirH * -1) < WIDTH)
            if (board[row + (dirV * -1), column + (dirH * -1)] == null)
                return false;

        queueW.Enqueue(board[row + (dirV * -1), column + (dirH * -1)]);     
        queueH.Enqueue(board[row + (dirV * -1), column + (dirH * -1)]);

        int pivotRow = row + (dirV * -1);
        int pivotColumn = column + (dirH * -1);

        // 맞는 짝을 상,하 탐색한다
        for (int i = 1; i < 7; ++i)
        {
            if (column + i != pivotColumn)
                if (!R && column + i < 7 && board[row, column + i]
                    && board[row, column + i].tag == type)
                    queueW.Enqueue(board[row, column + i]);
                else
                    R = true;

            if (column - i != pivotColumn)
                if (!L && column - i >= 0 && board[row, column - i] && board[row, column - i].tag == type)
                    queueW.Enqueue(board[row, column - i]);
                else
                    L = true;

            if (row + i != pivotRow)
                if (!U && row + i < 7 && board[row + i, column] && board[row + i, column].tag == type)
                    queueH.Enqueue(board[row + i, column]);
                else
                    U = true;

            if (row - i != pivotRow)
                if (!D && row - i >= 0 && board[row - i, column] && board[row - i, column].tag == type)
                    queueH.Enqueue(board[row - i, column]);
                else
                    D = true;
        }
        
        if (queueW.Count >= 3)
        {
            user.AddBombGage(queueW.Count * User.BOMB_INCREASE);
            // 콤보를 추가한다.
            user.AddCombo();
            // 짝이 맞는 동물타일의 개수만큼 점수를 올려준다. 
            user.AddTargetScore(SCORE_INCREASE * queueW.Count);

            // 피버상태라면 피버를 적용한다.
            if(user.IsFeverMode())
                FeverMode(queueW);

            // 큐가 공백일 때 까지 계속 Dequeue.
            while (queueW.Count > 0)
            {
                queueW.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            
            result1 = true;
        }

        if (queueH.Count >= 3)
        {
            user.AddBombGage(queueH.Count * User.BOMB_INCREASE);
            // 콤보를 추가한다.
            user.AddCombo();
            // 짝이 맞는 동물타일의 개수만큼 점수를 올려준다.
            user.AddTargetScore(SCORE_INCREASE * queueH.Count);

            // 피버상태라면 피버를 적용한다. 
            if (user.IsFeverMode())
                FeverMode(queueH);

            // 만약 이미 앞에서 기준이 되는 동물이 없어졌다면 팝.
            if (result1)
                queueH.Dequeue();

            // 큐가 공백일 때 까지 계속 Dequeue.
            while (queueH.Count > 0)
            {
                queueH.Dequeue().GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            
            result2 = true;
        }
        return result1 || result2;
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
                    if(board[i, j])
                        board[i, j].GetComponent<SpriteRenderer>().color += new Color(0.0f, 0.0f, 0.0f, FADE_VALUE);
                }
                yield return new WaitForSeconds(FADE_IN_DELAY);
            }
        }
    }

    // 빈 공간이 생긴 세로줄에 동물타일을 생성한다. 
    IEnumerator Refill()
    {
        bReFilling = true;
        bool bEmpty = false;

        while(true)
        {
            yield return new WaitForSeconds(TIMING);
            for (int i = 0; i < 7; ++i)
            {
                // 각 열이 비어있는지 확인 후 
                if (board[6, i] == null)
                {
                    // 비어있으면 새로운 동물타일을 생성한다. 
                    bEmpty = true;
                    CreateAnimalTile(i, 6, false);
                }
            }

            if (!bEmpty)
            {
                bReFilling = false;
                bDropping = false;

                // 모두 꽉 찼다면 동물타일 짝을 검사한다. 
                for(int i = 0; i < WIDTH; ++i)
                {
                    CheckAnimal(6, i, 0, 0, board[6, i].tag);
                }
                break;
            }
            
            bEmpty = false;
        }

    }

}   