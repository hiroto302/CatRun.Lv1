using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    enum GAMEMODE  //ゲームの状態を表す方法「列挙体」enum  番号に適切な名前をつけて管理することができる
    {
        TITLE,
        PLAY,
        DEMO,
        END,
    };
    GAMEMODE nowMode;  //現在のゲームモード
    public Transform titleInfoGroup;  //他のオブジェクトをtransformの形として、GameDirectergerが参照することができる
    public PlayerController player;
    public Transform goalMarker;
    public Transform endInfoGroup;
    public Transform esaGroup;
    void Start()
    {
        nowMode = GAMEMODE.TITLE;
        titleInfoGroup.gameObject.SetActive(true);
        // player.Reset();
    }

    void Update()
    {
        //switch cace文で各モードの時に行う処理を条件分岐で行う
        switch (nowMode)
        {
            //タイトルモードの時にここのプログラムが処理される
            case GAMEMODE.TITLE:
                if(Input.GetButtonDown("Jump"))
                {
                    nowMode = GAMEMODE.PLAY;
                    //タイトルロゴなどを消す
                    titleInfoGroup.gameObject.SetActive(false);
                    //playerを動かす
                    player.IsStop = false;
                }
                break;

            case GAMEMODE.PLAY:
                if(player.transform.position.x > goalMarker.transform.position.x)
                {
                    player.IsStop = true;
                    if(player.transform.position.y < goalMarker.transform.position.y)
                    {
                    nowMode = GAMEMODE.DEMO;
                    endInfoGroup.gameObject.SetActive(true);
                    player.DemoStart();
                    }
                }
                break;

            case GAMEMODE.DEMO:
                nowMode = GAMEMODE.END;
                break;

            case GAMEMODE.END:
                //gameをリスタートするときは、初期化しなければならない
                if(Input.GetButtonDown("Jump"))
                {
                    nowMode = GAMEMODE.TITLE;
                    //タイトルロゴなどを表示し直す
                    titleInfoGroup.gameObject.SetActive(true);
                    endInfoGroup.gameObject.SetActive(false);
                    //childによりesaGroupの中身にアクセスすることができる
                    for(int i=0; i < esaGroup.childCount; ++i)  //child
                    {
                    esaGroup.GetChild(i).gameObject.SetActive(true);  //esaGroupの中に含まれているエサ一つずつアクセスしてtrueにしていく
                    }
                    player.Reset();
                }
                break;
        }
    }
}
