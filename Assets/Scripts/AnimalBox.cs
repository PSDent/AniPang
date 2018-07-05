using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBox : MonoBehaviour {

    const float DRAG_SPEED = 0.2f;
    const float DRAG_RANGE = 0.8f;
    const float DRAG_RANGE_DIV = 1.6f;

    GameManager gameMgr;
    BoxCollider2D boxColl;
    Vector3 originPos;

    int row, column;
    bool bClicked = false;
    bool fix = false;
    float vertical = 0, horizontal = 0;

	// Use this for initialization
	void Start () {
        originPos = transform.position;
        Debug.Log(originPos);
        boxColl = GetComponent<BoxCollider2D>();
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnMouseUp()
    {
        // 상하 이동 중
        if(vertical != 0)
        {
            // ## 이곳의 row + (int)(vertical / vertical), column 값에 대해 보완을하여 버그를 없앨것.

            // 동물 타일이 절반 이상 넘어간 후 마우스를 놨다면
            if (originPos.y + DRAG_RANGE / DRAG_RANGE_DIV < transform.position.y || originPos.y - DRAG_RANGE / DRAG_RANGE_DIV > transform.position.y)
            {
                // 넘어간 타일과 위치를 교환한다.
                transform.position = gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column].GetComponent<AnimalBox>().GetOriginPos();
                gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column].transform.position = originPos;

                // 각 동물 타일의 originPos 값을 초기화한다.
                originPos = transform.position;
                gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column].GetComponent<AnimalBox>().SetOriginPos(
                    gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column].transform.position);

                // board 배열의 위치를 바꾼다.
                GameObject tempObj = gameMgr.GetAnimalTile()[row, column];
                gameMgr.GetAnimalTile()[row, column] = gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column];
                gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column] = tempObj;

                gameMgr.GetAnimalTile()[row, column].GetComponent<AnimalBox>().SetArrNumber(row - (int)(vertical / vertical), column);
                gameMgr.GetAnimalTile()[row + (int)(vertical / vertical), column].GetComponent<AnimalBox>().SetArrNumber(row + (int)(vertical / vertical), column);
            }
            // 절반을 넘지 못했다면 다시 원위치 시킨다.
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }
        // 좌우 이동 중
        else if(horizontal != 0)
        {
            // 동물 타일이 절반 이상 넘어간 후 마우스를 놨다면
            if (originPos.x + DRAG_RANGE / DRAG_RANGE_DIV < transform.position.x || originPos.x - DRAG_RANGE / DRAG_RANGE_DIV > transform.position.x)
            {
                // 넘어간 타일과 위치를 교환한다.

            }
            // 절반을 넘지 못했다면 다시 원위치 시킨다.
            else
            {
                transform.position = originPos;
                fix = false;
            }
        }

        bClicked = false;
        Debug.Log("MouseUp");
    }

    // 마우스가 드래그 된 방향에 따라 동물 타일을 교환시킨다.
    private void OnMouseDrag()
    {
        float mouseHorizon = Input.GetAxis("Mouse X");
        float mouseVertical = Input.GetAxis("Mouse Y");
        Vector3 pos = transform.position;
        int t_row = row, t_column = column;

        // 더 좋은 방향으로 수정할 것 -
        // 마우스 방향과 속도에 따라 동물 타일을 움직이되 
        // 동물타일이 일정 거리를 벗어나면 그 때 해당 방향의 타일과 위치를 교환하도록 할 것.

        // # 가장자리에 위치한 동물 타일들이 화면 밖으로 못 나가도록 할 것.

        // 상, 하
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

        Vector3 nowPos = transform.position;
        if(originPos.x + DRAG_RANGE > nowPos.x + mouseHorizon * horizontal && originPos.x - DRAG_RANGE < nowPos.x + mouseHorizon * horizontal &&
            originPos.y + DRAG_RANGE > nowPos.y + mouseVertical * vertical && originPos.y - DRAG_RANGE < nowPos.y + mouseVertical * vertical)
            transform.position += new Vector3(mouseHorizon * horizontal, mouseVertical * vertical, 0);
    }
    //Debug.Log("Up!");

    //if (Mathf.Abs(mouseVertical) > Mathf.Abs(mouseHorizon))
    //{

    //    transform.position += new Vector3(0, mouseVertical * DRAG_SPEED, 0);
    //    // Up
    //    if (mouseVertical > 0)
    //    {

    //        //GameObject tempObj = gameMgr.GetAnimalTile()[row + 1, column];

    //        //gameMgr.GetAnimalTile()[row + 1, column] = transform.gameObject;
    //        //gameMgr.GetAnimalTile()[row, column] = tempObj;

    //        //gameMgr.GetAnimalTile()[row + 1, column].transform.position = tempObj.transform.position;
    //        //gameMgr.GetAnimalTile()[row, column].transform.position = pos;

    //        //// 행, 렬 값 변경.
    //        //gameMgr.GetAnimalTile()[row, column].GetComponent<AnimalBox>().SetArrNumber(row - 1, column);
    //        //gameMgr.GetAnimalTile()[row + 1, column].GetComponent<AnimalBox>().SetArrNumber(row + 1, column);

    //        Debug.Log("Up");
    //    }
    //    // Down
    //    else
    //    {
    //        //GameObject tempObj = gameMgr.GetAnimalTile()[row - 1, column];
    //        //gameMgr.GetAnimalTile()[row - 1, column] = transform.gameObject;
    //        //gameMgr.GetAnimalTile()[row, column] = tempObj;

    //        //gameMgr.GetAnimalTile()[row - 1, column].transform.position = tempObj.transform.position;
    //        //gameMgr.GetAnimalTile()[row, column].transform.position = pos;

    //        //// 행, 렬 값 변경.
    //        //gameMgr.GetAnimalTile()[row, column].GetComponent<AnimalBox>().SetArrNumber(row + 1, column);
    //        //gameMgr.GetAnimalTile()[row - 1, column].GetComponent<AnimalBox>().SetArrNumber(row - 1, column);

    //        Debug.Log("Down");
    //    }
    //    //gameMgr.SendMessage("CheckAnimal");

    //}
    //// Left or Right
    //else
    //{
    //    transform.position += new Vector3(mouseHorizon * DRAG_SPEED, 0, 0);
    //    // Left
    //    if (mouseHorizon < 0)
    //    {
    //        //GameObject tempObj = gameMgr.GetAnimalTile()[row, column + 1];

    //        //gameMgr.GetAnimalTile()[row, column + 1] = transform.gameObject;
    //        //gameMgr.GetAnimalTile()[row, column] = tempObj;

    //        //gameMgr.GetAnimalTile()[row, column + 1].transform.position = tempObj.transform.position;
    //        //gameMgr.GetAnimalTile()[row, column].transform.position = pos;

    //        //// 행, 렬 값 변경.
    //        //gameMgr.GetAnimalTile()[row, column].GetComponent<AnimalBox>().SetArrNumber(row, column - 1);
    //        //gameMgr.GetAnimalTile()[row, column + 1].GetComponent<AnimalBox>().SetArrNumber(row, column + 1);

    //        Debug.Log("Left");
    //    }
    //    // Right
    //    else
    //    {
    //        //GameObject tempObj = gameMgr.GetAnimalTile()[row + 1, column];

    //        //gameMgr.GetAnimalTile()[row, column - 1] = transform.gameObject;
    //        //gameMgr.GetAnimalTile()[row, column] = tempObj;

    //        //gameMgr.GetAnimalTile()[row, column - 1].transform.position = tempObj.transform.position;
    //        //gameMgr.GetAnimalTile()[row, column].transform.position = pos;

    //        //// 행, 렬 값 변경.
    //        //gameMgr.GetAnimalTile()[row, column].GetComponent<AnimalBox>().SetArrNumber(row, column + 1);
    //        //gameMgr.GetAnimalTile()[row, column - 1].GetComponent<AnimalBox>().SetArrNumber(row, column - 1);

    //        Debug.Log("Right");
    //    }
    //}

    //bClicked = true;

    public void SetOriginPos(Vector3 originPos)
    {
        this.originPos = originPos;
    }

    public Vector3 GetOriginPos()
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

}
