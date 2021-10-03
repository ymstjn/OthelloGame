using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMove : MonoBehaviour
{
    int[] SelectPos = new int[2];

    void Start()
    {
        //z座標
        SelectPos[0] = 2;
        //x座標
        SelectPos[1] = 4;

        this.transform.position = PiecePos(SelectPos);
    }

    void Update()
    {
        if (Input.GetKeyDown("right"))
        {
            if (SelectPos[1] < 7)
            {
                SelectPos[1] += 1;
                this.transform.position = PiecePos(SelectPos);
            }
        }

        if (Input.GetKeyDown("left"))
        {
            if (0 < SelectPos[1])
            {
                SelectPos[1] -= 1;
                this.transform.position = PiecePos(SelectPos);
            }
        }

        if (Input.GetKeyDown("up"))
        {
            if (0 < SelectPos[0])
            {
                SelectPos[0] -= 1;
                this.transform.position = PiecePos(SelectPos);
            }
        }

        if (Input.GetKeyDown("down"))
        {
            if (SelectPos[0] < 7)
            {
                SelectPos[0] += 1;
                this.transform.position = PiecePos(SelectPos);
            }
        }
    }

    Vector3 PiecePos(int[] sp)
    {
        float z = 3.5f - (float)sp[0] * 1.0f;
        float x = -3.5f + (float)sp[1] * 1.0f;

        return new Vector3(x, 0.35f, z);
    }

    public int[] Out_SelectPos()
    {
        return SelectPos;
    }

}
