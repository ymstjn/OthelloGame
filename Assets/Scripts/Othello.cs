using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Othello : MonoBehaviour
{
    //置いてない場所：0、白：1、黒：-1
    //左上[0, 0]、右上[0, 7]、左下[7, 0]、右下[7, 7]
    int[,] board = new int[8, 8];

    public GameObject Pf_Piece;
    GameObject instance;

    //置いてない場所の記憶
    List<int> EmptyPos = new List<int>();
    public GameObject Pf_Pointer;
    GameObject instance_p;

    //プレイヤーのターンの確認
    //白ターン:1、黒ターン:-1
    int tp_white = 1;

    //プレイヤーの数字と名前の辞書型
    Dictionary<int, string> Player = new Dictionary<int, string>();

    //置けるか確認時に使う8方向(左から時計回り)
    readonly int[] dz = { 0, -1, -1, -1, 0, 1, 1, 1 };
    readonly int[] dx = { -1, -1, 0, 1, 1, 1, 0, -1 };

    //両プレイヤーのポイント表示
    Dictionary<int, int> PlayerPoint = new Dictionary<int, int>();

    SelectMove selectMove;
    Text PlayerText;
    Text PointText;

    void Start()
    {
        //初期配置
        board[3, 3] = 1;
        SetPiece(3, 3, 1);
        board[4, 4] = 1;
        SetPiece(4, 4, 1);
        board[3, 4] = -1;
        SetPiece(3, 4, -1);
        board[4, 3] = -1;
        SetPiece(4, 3, -1);

        //置いてない場所の初期化
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                EmptyPos.Add(i * 10 + j);
            }
        }
        EmptyPos.Remove(33);
        EmptyPos.Remove(44);
        EmptyPos.Remove(34);
        EmptyPos.Remove(43);

        selectMove = GameObject.Find("SelectCube").GetComponent<SelectMove>();
        PlayerText = GameObject.Find("PlayerText").GetComponent<Text>();
        PlayerText.text = "white";

        PointText = GameObject.Find("PointText").GetComponent<Text>();

        PointerDisplay();

        //数字とプレイヤーの紐づけ
        Player[1] = "White";
        Player[-1] = "Black";

        //ポイントの初期化
        PlayerPoint[1] = 2;
        PlayerPoint[-1] = 2;
        PointText.text = PointDisplay(PlayerPoint[1], PlayerPoint[-1]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var selectPos = selectMove.Out_SelectPos();
            List<int> ReversedPos;
            ReversedPos = SetCheck(selectPos[0], selectPos[1], tp_white);
            
            //駒を置くことが可能な場合
            if (ReversedPos.Count > 0)
            {
                //新たに置く駒のポイント追加
                PlayerPoint[tp_white] += 1;

                board[selectPos[0], selectPos[1]] = tp_white;
                SetPiece(selectPos[0], selectPos[1], tp_white);

                EmptyPos.Remove(selectPos[0] * 10 + selectPos[1]);

                for (int i = 0; i < ReversedPos.Count; i++)
                {
                    int rz = ReversedPos[i] / 10;
                    int rx = ReversedPos[i] % 10;

                    board[rz, rx] = tp_white;
                    ReversedPiece(rz, rx);

                    //ポイントの追加
                    PlayerPoint[tp_white] += 1;
                    PlayerPoint[-1 * tp_white] -= 1;
                }
                tp_white *= -1;
            }
            PointText.text = PointDisplay(PlayerPoint[1], PlayerPoint[-1]);

            string player = "";

            DeletePointer();
            bool flag1 = PointerDisplay();
            if (flag1 == false)
            {
                //プレイヤ―が置くことが出来ませんという処理
                player += Player[tp_white];
                player += "\ncan't place\n";

                tp_white *= -1;
                bool flag2 = PointerDisplay();

                player += Player[tp_white];

                if (flag2 == false)
                {
                    //ゲーム終了の処理
                    player = "ゲーム終了";
                }
            }
            else
            {
                //プレイヤーの表示
                player += Player[tp_white];
            }

            PlayerText.text = player;
        }

        if (Input.GetKeyDown("r"))
        {
            Scene loadScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadScene.name);
        }
    }

    //駒オブジェクトを設置
    void SetPiece(float z, float x, int wb)
    {
        Vector3 v3 = new Vector3(-3.5f + x * 1.0f, 0.35f, 3.5f - z * 1.0f);
        Quaternion rot = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        if (wb == -1)
        { 
            rot.z = 180.0f;
        }

        instance = Instantiate(Pf_Piece, v3, rot);
        instance.name = "Piece[" + z.ToString() + "," + x.ToString() + "]";
    }

    //石を置けるかどうかの確認
    List<int> SetCheck(int z, int x, int white)
    {
        //変更する座標を記憶する
        List<int> reversedPos = new List<int>();

        if (board[z, x] == 0)
        {
            int enemy = 1;
            int mine = -1;

            if (white == 1)
            {
                enemy = -1;
                mine = 1;
            }

            for (int i = 0; i < 8; i++)
            {
                int set_z = z;
                int set_x = x;
                bool check_enemy = false;
                List<int> stockPos = new List<int>();

                while (true) 
                {
                    set_z += dz[i];
                    set_x += dx[i];

                    if (0 <= set_z && set_z < 8 && 0 <= set_x && set_x < 8)
                    {
                        if (board[set_z, set_x] == enemy)
                        {
                            stockPos.Add(set_z * 10 + set_x);
                            check_enemy = true;
                        }
                        else if (check_enemy == true && board[set_z, set_x] == mine)
                        {
                            break;
                        }
                        else
                        {
                            stockPos = new List<int>();
                            break;
                        }
                    }
                    else
                    {
                        stockPos = new List<int>();
                        break;
                    }
                }

                if (stockPos.Count > 0)
                {
                    for (int j = 0; j < stockPos.Count; j++)
                    {
                        reversedPos.Add(stockPos[j]);                        
                    }
                }
            }
        }
        return reversedPos;
    }

    void ReversedPiece(int z, int x)
    {
        GameObject piece = GameObject.Find("Piece[" + z.ToString() + "," + x.ToString() + "]");
        Vector3 rot = piece.transform.localEulerAngles;
        rot.z += 180.0f;

        piece.transform.localEulerAngles = rot;
    }


    //ポインターオブジェクトを設置
    void SetPointer(float z, float x)
    {
        Vector3 v3 = new Vector3(-3.5f + x * 1.0f, 0.275f, 3.5f - z * 1.0f);
        Quaternion rot = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        instance_p = Instantiate(Pf_Pointer, v3, rot);
    }

    //駒を置ける場所のポインター表示
    bool PointerDisplay()
    {
        bool pointerFlag = false;

        for (int i = 0; i < EmptyPos.Count; i++)
        {
            int pz = EmptyPos[i] / 10;
            int px = EmptyPos[i] % 10;
            List<int> forCheck = SetCheck(pz, px, tp_white);

            if (forCheck.Count > 0)
            {
                SetPointer(pz, px);
                pointerFlag = true;
            }
        }

        return pointerFlag;
    }

    void DeletePointer()
    {
        var clones = GameObject.FindGameObjectsWithTag("Pointer");
        foreach (var clone in clones)
        {
            Destroy(clone);
        }
    }

    //ポイントの表示の為の処理
    string PointDisplay(int p1, int p2)
    {
        string s = "White : " + p1 + "\nBlack : " + p2;
        return s;
    }


    //主にエラー処理を行う。
    //エラー処理は分かっているだけで2つ
    //1.自分の駒が置けなくなった時
    //2.駒を全部置き終わる前に、ゲームが終了する時

    void CheckError()
    { 
    
    }

    void CheckArray()
    {
        string s = "";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                s += board[i, j].ToString() + "";
            }
            s += "\n";
        }
        Debug.Log(s);
    }
}
