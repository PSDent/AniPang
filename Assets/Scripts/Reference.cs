using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reference : MonoBehaviour {
    public struct POINT
    {
        public int x;
        public int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        int GetX()
        {
            return x;
        }

        int GetY()
        {
            return y;
        }
    }

    public const string CROSSHAIR = "sprite/UI/crosshair";
    public const string BOMB = "Sprite/UI/bomb_circle";
}
