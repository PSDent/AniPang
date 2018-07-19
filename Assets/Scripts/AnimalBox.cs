using System.Collections;
using UnityEngine;

public class AnimalBox : MonoBehaviour
{

    const float DRAG_SPEED = 0.2f;
    const float DRAG_RANGE = 0.8f;
    const float DRAG_RANGE_DIV = 1.6f;
    const float DROP_SPEED = 0.08f;
    const float DROP_TIME = 0.01f;

    SpriteRenderer spriteRenderer;
    User user;
    GameManager gameMgr;
    BoxCollider2D boxColl;
    Vector3 originPos;

    int dirV = 0, dirH = 0;
    int row, column;
    bool fix = false;
    float vertical = 0, horizontal = 0;

    // 플래그 
    bool bGhost = false;
    bool bLight = false;
    bool bStart = false;
    bool bDropping = false;
    bool bBomb = false;

    // Use this for initialization
    void Start()
    {
        user = GameObject.Find("GameManager").GetComponent<User>();
        originPos = transform.position;
        boxColl = GetComponent<BoxCollider2D>();
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!bDropping && row > 0 && gameMgr.GetAnimalTile()[row - 1, column] == null)
        {
            StartCoroutine("Drop");
        }

       // if(bBomb)
    }

    void Switching()
    {
        GameObject ChangeTile = gameMgr.GetAnimalTile()[row + dirV, column + dirH];

        // 넘어간 타일과 위치를 교환한다.
        transform.position = ChangeTile.GetComponent<AnimalBox>().GetOriginPos();
        ChangeTile.transform.position = originPos;

        // 각 동물 타일의 originPos 값을 초기화한다.
        originPos = transform.position;
        ChangeTile.GetComponent<AnimalBox>().SetOriginPos(ChangeTile.transform.position);

        // 이부분이 참 이상한 것 같습니다 
        // board 배열의 위치를 바꾼다.
        GameObject tempObj = gameMgr.GetAnimalTile()[row, column];
        gameMgr.GetAnimalTile()[row, column] = ChangeTile;
        gameMgr.GetAnimalTile()[row + dirV, column + dirH] = tempObj;
        //Debug.Log("Before Row : " + row + " Column : " + column);

        gameMgr.GetAnimalTile()[row, column].GetComponent<AnimalBox>().SetArrNumber(row, column);
        gameMgr.GetAnimalTile()[row + dirV, column + dirH].GetComponent<AnimalBox>().SetArrNumber(row + dirV, column + dirH);
        //Debug.Log("After Row : " + row + " Column : " + column);
    }

    private void OnMouseDown()
    {
        if (bBomb)
            Bomb();
        else if (bGhost)
            GhostBlock();
    }

    // 타일 이동시 짝이 3 이상이라면 그 때 움직이도록 수정할 것.
    // 이 부분이 제일 맛갔음 고치셈.
    private void OnMouseUp()
    {
        bool bOne = false, bTwo = false;
        // 상하 이동 중
        if (vertical != 0)
        {
            // 동물 타일이 절반 이상 넘어간 후 마우스를 놨다면
            // 여기 뭔가 문제가 있음. 
            if (originPos.y + DRAG_RANGE / DRAG_RANGE_DIV < transform.position.y || originPos.y - DRAG_RANGE / DRAG_RANGE_DIV > transform.position.y)
            {
                if (row + dirV < 0 || row + dirV > 6)
                {
                    transform.position = originPos;
                    return;
                }

                GameObject ChangeTile = gameMgr.GetAnimalTile()[row + dirV, column];
                bOne = gameMgr.CheckAnimal(row + dirV, column, dirV, dirH, transform.tag);
                // 아래의 구문에서 심각한 인덱스 문제가 발생함. 고치기 바람. 
                bTwo = gameMgr.CheckAnimal(row, column, -dirV, dirH, ChangeTile.tag);

                if ((bOne || bTwo) == false)
                {
                    transform.position = originPos;
                    fix = false;
                    return;
                }
                Switching();
            }
            // 절반을 넘지 못했다면 다시 원위치 시킨다. 
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }
        // 좌우 이동 중
        else if (horizontal != 0)
        {
            // 동물 타일이 절반 이상 넘어간 후 마우스를 놨다면
            if (originPos.x + DRAG_RANGE / DRAG_RANGE_DIV < transform.position.x || originPos.x - DRAG_RANGE / DRAG_RANGE_DIV > transform.position.x)
            {
                if (column + dirH < 0 || column + dirH > 6)
                {
                    transform.position = originPos;
                    return;
                }

                // 넘어간 타일 위치
                GameObject ChangeTile = gameMgr.GetAnimalTile()[row, column + dirH];
                bTwo = gameMgr.CheckAnimal(row, column + dirH, dirV, dirH, transform.tag);
                bOne = gameMgr.CheckAnimal(row, column, dirV, -dirH, ChangeTile.tag);

                if ((bOne || bTwo) == false)
                {
                    transform.position = originPos;
                    fix = false;
                    return;
                }

                Switching();
            }

            // 절반을 넘지 못했다면 다시 원위치 시킨다.
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }
        //if (originPos != transform.position)
           transform.position = originPos;
    }

    // 마우스가 드래그 된 방향에 따라 동물 타일을 교환시킨다.
    private void OnMouseDrag()
    {
        float mouseHorizon = Input.GetAxis("Mouse X");
        float mouseVertical = Input.GetAxis("Mouse Y");
        Vector3 pos = transform.position;
        int t_row = row, t_column = column;

        // # 가장자리에 위치한 동물 타일들이 화면 밖으로 못 나가도록 할 것.

        // 상, 하
        // fix 변수는 수직 혹은 수평으로만 움직일 수 있도록 방향을 고정해주는 flag 변수
        if (Mathf.Abs(mouseVertical) > Mathf.Abs(mouseHorizon) && !fix)
        {
            vertical = DRAG_SPEED;
            horizontal = 0;
            fix = true;
        }
        // 좌, 우
        else if (Mathf.Abs(mouseVertical) < Mathf.Abs(mouseHorizon) && !fix)
        {
            horizontal = DRAG_SPEED;
            vertical = 0;
            fix = true;
        }

        DecideDirection(mouseVertical, mouseHorizon);

        // 드래그한 만큼 타일을 이동시킨다. 
        Vector3 nowPos = transform.position;
        if (originPos.x + DRAG_RANGE > nowPos.x + mouseHorizon * horizontal && originPos.x - DRAG_RANGE < nowPos.x + mouseHorizon * horizontal &&
            originPos.y + DRAG_RANGE > nowPos.y + mouseVertical * vertical && originPos.y - DRAG_RANGE < nowPos.y + mouseVertical * vertical)
            transform.position += new Vector3(mouseHorizon * horizontal, mouseVertical * vertical, 0);
    }

    // 마우스 드래그 방향에 따라 동물 타일의 방향을 결정함.
    void DecideDirection(float mouseV, float mouseH)
    {
        if (vertical > 0)
        {
            if (mouseV > 0)
                dirV = 1;
            else if (mouseV < 0)
                dirV = -1;
            dirH = 0;
        }
        else
        {
            if (mouseH > 0)
                dirH = 1;
            else if (mouseH < 0)
                dirH = -1;
            dirV = 0;
        }
    }

    void SetOriginPos(Vector3 originPos)
    {
        this.originPos = originPos;
    }

    Vector3 GetOriginPos()
    {
        return originPos;
    }

    public void SetArrNumber(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void GetArrNumber(ref int row, ref int column)
    {
        row = this.row;
        column = this.column;
    }

    void Bomb()
    {
        int cnt = 0;

        for (int i = 0; i < GameManager.WIDTH; ++i)
            if (column != i)
            {
                ++cnt;
                gameMgr.GetAnimalTile()[0, i].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }

        for (int i = 1; i < GameManager.WIDTH; ++i)
        {
            if (column + i < GameManager.WIDTH && gameMgr.GetAnimalTile()[row, column + i])
            {
                ++cnt;
                gameMgr.GetAnimalTile()[row, column + i].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            if (column - i >= 0 && gameMgr.GetAnimalTile()[row, column - i])
            {
                ++cnt;
                gameMgr.GetAnimalTile()[row, column - i].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            if (row + i < GameManager.HEIGHT && gameMgr.GetAnimalTile()[row + i, column])
            {
                ++cnt;
                gameMgr.GetAnimalTile()[row + i, column].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
            if (row - i >= 0 && gameMgr.GetAnimalTile()[row - i, column])
            {
                ++cnt;
                gameMgr.GetAnimalTile()[row - i, column].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
            }
        }

        user.Addition(cnt);
        gameMgr.GetAnimalTile()[row, column].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
    }

    public void LightBlock()
    {
        int cnt = 0;
        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                if (row + i >= 0 && row + i < GameManager.HEIGHT && column + j >= 0 && column + j < GameManager.WIDTH)
                    if (gameMgr.GetAnimalTile()[row + i, column + j] && (row + i != row && column + j != column) 
                        && gameMgr.GetAnimalTile()[row + i, column + j].tag != "Bomb")
                    {
                        ++cnt;
                        gameMgr.GetAnimalTile()[row + i, column + j].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
                    }
            }
        }
        user.Addition(cnt); 
    }

    // 유령블럭 혹은 폭탄 함수에 무한루프가 걸린다. 잘 찾아서 해결하자.
    void GhostBlock()
    {
        string str;
        int val = Random.Range(0, 7);
        GameManager.AnimalType animalType = (GameManager.AnimalType)val;
        str = animalType.ToString();

        int cnt = 0;
        for (int i = 0; i < GameManager.HEIGHT; ++i)
        {
            for (int j = 0; j < GameManager.WIDTH; ++j)
            {
                if (gameMgr.GetAnimalTile()[i, j] && gameMgr.GetAnimalTile()[i, j].tag == str)
                {
                    ++cnt;
                    gameMgr.GetAnimalTile()[i, j].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
                }
            }
        }

        user.Addition(cnt);
        gameMgr.GetAnimalTile()[row, column].GetComponentInChildren<DestroyTile>().StartCoroutine("FlareDestroy");
    }

    public void ActiveBombFlag()
    {
        bBomb = true;
        Debug.Log("ASD");
        SetSpriteBomb();
    }

    public void ActiveGhostFlag()
    {
        bGhost = true;
    }

    public void ActiveLightFlag()
    {
        bLight = true;
    }

    public bool GetLightFlag()
    {
        return bLight;
    }

    public void SetSpriteBomb()
    {
        Debug.Log("SD");
        spriteRenderer.sprite = Resources.Load<Sprite>(Reference.BOMB);
    }

    public void SetCrossHair()
    {
        transform.Find("EffectSprite").GetComponent<SpriteRenderer>().
            sprite = Resources.Load<Sprite>(Reference.CROSSHAIR);
        transform.Find("EffectSprite").GetComponent<SpriteRenderer>().color = new Color(1.0f, 0, 0, 1.0f);
        transform.Find("EffectSprite").position += new Vector3(0, 0, -2);
        transform.Find("EffectSprite").GetComponent<SpriteRenderer>().enabled = true;

    }

    public void SetColor(float r, float g, float b, float a)
    {
        Color color = new Color(r, g, b, a);

        GetComponent<SpriteRenderer>().color = color;
    }

    public bool IsSpecial()
    {
        return bBomb; 
    }

    IEnumerator Drop()
    {
        bDropping = true;

        // 맨 처음의 경우 사라지는 동물과 타이밍을 맞추기 위해 조금 지연시킨다. 
        if (!bStart)
        {
            yield return new WaitForSeconds(GameManager.TIMING-0.2f);
            bStart = true;
        }

        // 이후 아래가 비었는지 계속 확인하며 떨어진다.
        while (row > 0 && gameMgr.GetAnimalTile()[row - 1, column] == null)
        {
            gameMgr.GetAnimalTile()[row - 1, column] = gameMgr.GetAnimalTile()[row, column];
            gameMgr.GetAnimalTile()[row, column] = null;
            --row;

            while (originPos.y - transform.position.y < 0.8f)
            {
                Vector3 movement = new Vector3(0, DROP_SPEED, 0);
                transform.position -= movement;

                yield return new WaitForSeconds(DROP_TIME);
            }

            Vector3 tempVector = new Vector3(originPos.x, originPos.y - 0.8f, originPos.z);
            originPos = tempVector;
        }

        transform.position = originPos;
        bStart = false;

        gameMgr.CheckAnimal(row, column, 0, 0, transform.tag);
        bDropping = false;
    }
}