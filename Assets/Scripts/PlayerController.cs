using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    int score;
    public UnityEngine.UI.Text scoreValue;  //他オブジェクトであるUI text を操作する方法は色々あるのね。
    public UnityEngine.UI.Image nekokanImage;
    public SpriteRenderer nekobako;
    public Sprite[] nekobakoImages;
    float downSpeed; //落下速度
    Rigidbody2D rb;  //物理演算コンポーネントを使用してplayerを動かす
    Animator animCtrl;  //Animatorコンポーネントを通じて、AnimatorControllerを制御するために変数を取得する
    AudioSource audioSource;
    public AudioClip[] sounds;  //[]配列の形で効果音を収納する、番号管理する

    public bool IsStop;  //停止モード
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animCtrl = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        Reset();  //初期化をResetメソッドにまとめた,上記のanimctrlより下に記述すること。animctrlが参照することができないerrorになる
    }

    // Update is called once per frame
    void Update()
    {
        float forwardSpeed;  //ゴール後、落下するようする。前方方向の力

        // if(IsStop) return;  //停止状態であればreturnによりUpdateされないようにする
        if(IsStop) forwardSpeed = 0;
        else forwardSpeed =1;


        //なんどもjumpができないよにする
        RaycastHit2D hit;  //当たったかどうか返してくれるオブジェクト
        //下方向チェック
        hit = Physics2D.Raycast(transform.position + new Vector3(-0.32f, -0.32f), Vector2.right, 0.64f);  //ここの解説、ジャンプ処理１４分頃の位置
        //仮想の線を飛ばして当たっているか判定する、Playerの座標(中心位置)から、Playerの座標から見た相対的なRayの発射位置を指定、基準位置から(-0.32f, -0.32f)の位置に移動した所から, Vector2.right右方向に, 0.64f伸ばした位置をRayCastである仮想の線をひきますよという意味)
        if(hit.transform != null)  //センサーのhitに、何も当たっていないとNullを返す。当たった相手がNullではない時、つまり何か当たった時以下の処理をする
        {
            downSpeed = 0;  //nowPosのベクトルが右斜め下になるためここで初期化してめり込むのを防ぐ、自動で右方向に動いていくような処理にしたい
            animCtrl.SetBool("IsGround",true);  //アニメーションJumpDownからRunへの移行
            if(Input.GetButtonDown("Jump") && !IsStop)  //MouseButtonじゃない操作入力もあるよ！  edit→Input→Jumpから対応したボタンを確認できるよ！
            {
                downSpeed = 6.5f;  //落下とは逆方向にspeedをつけてあげる
                transform.Translate(Vector3.up * 0.01f);  //jump後にRaycastに引っかからない位置まで無理やり動かしておく
                audioSource.PlayOneShot(sounds[0]);
            }
        }
        else
        {
            animCtrl.SetBool("IsGround",false);
            downSpeed += -0.3f;  //落下速度をどんどん早くする
        }
        //上方向チェック
        hit = Physics2D.Raycast(transform.position + new Vector3(-0.32f, 0.32f), Vector2.right, 0.64f);
        if(hit.transform != null)
        {
            downSpeed = -0.1f;
            transform.position += new Vector3(0, -0.1f, 0);
        }

        //停止状態のアニメーション機能
        hit = Physics2D.Raycast(transform.position + new Vector3(0.34f, 0.26f), Vector2.down, 0.52f);  //前方方向のあたり判定
        if(hit.transform != null || IsStop)
        {
            animCtrl.SetBool("IsIdle", true);
        }
        else
        {
            animCtrl.SetBool("IsIdle", false);
        }

        //Playerの動作
        Vector2 nowPos = rb.position;  //plyerの現在位置の取得
        nowPos += new Vector2(forwardSpeed, downSpeed) * Time.deltaTime;  //位置をdownSpeed毎に変更していく
        rb.MovePosition(nowPos);  //Positionの更新を代入
        animCtrl.SetFloat("DownSpeed", downSpeed);  //DownJumpのアニメーション  downSpeedが０より大きいか小さいかで判別
    }

    //スコア機能
    private void OnTriggerEnter2D(Collider2D collision)  //引数に当たった相手の情報が入る
    {
        collision.gameObject.SetActive(false);  //対象物を消す方法はdestroyだけではない
        if(collision.name =="nekokan")
        {
            audioSource.PlayOneShot(sounds[2]);
            nekokanImage.gameObject.SetActive(true);
        }
        else
        {
            score +=1;
            scoreValue.text = score.ToString();  //スコアの表示
            audioSource.PlayOneShot(sounds[1]);  //音の再生
        }
    }

    //リセット機能
    public void Reset()
    {
        score = 0;
        scoreValue.text = score.ToString();  //スコアの表示
        nekokanImage.gameObject.SetActive(false);
        downSpeed = 0;
        IsStop = true;  //停止状態
        // animCtrl.SetBool("IsIdle", true);
        transform.position = new Vector3(0, -1.62f, 0);  //位置を戻し忘れないこと！
        animCtrl.SetTrigger("DemoEnd");
        nekobako.sprite = nekobakoImages[0];
    }

    //DEMO機能
    public void DemoStart()
    {
        animCtrl.SetTrigger("DemoStart");
    }
    void PlayJumpSound()
    {
        audioSource.PlayOneShot(sounds[0]);  //DemoAnimationにeventを追加し、効果音を加える
    }
    void ChangeNekobako()
    {
        nekobako.sprite = nekobakoImages[1];
    }
}
