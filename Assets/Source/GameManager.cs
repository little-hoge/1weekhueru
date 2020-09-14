using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eenum;
using UnityEngine.UI;

namespace Eenum {

    enum gamestate {
        TITLE,
        MAIN,
        OVER,
    };

}

public class GameManager : MonoBehaviour {

    // 
    public GameObject Logo;
    public GameObject TitlePanel;
    public GameObject GameOverPanel;

    public Text LevelText;
    private List<GameObject> LogoList = new List<GameObject>();

    // X、Y、Z座標の最小/最大
    const short xMinPos = -30, xMaxPos = 30;
    const short yMinPos = -50, yMaxPos = 50;
    const short zMinPos = 0, zMaxPos = 360;

    // ゲーム状態
    gamestate gstate;

    // 
    byte alpha;
    int level;
    const short levelweight = 5;

    // Start is called before the first frame update
    void Start() {
        LogoCreate();

    }

    // Update is called once per frame
    void Update() {
        LevelText.text = "Level：" + level.ToString() + "　表示度：" + alpha * 100 / 255;
        switch (gstate) {
            case gamestate.TITLE:
                break;
            case gamestate.MAIN:
                GameMain();
                break;
            case gamestate.OVER:
                GameOver();
                break;
            default:
                break;
        }

    }

    // ランダム位置取得
    private Vector3 GetRandomPosition() {
        float x = Random.Range(xMinPos, xMaxPos);
        float y = Random.Range(yMinPos, yMaxPos);
        float z = Random.Range(zMinPos, zMaxPos);
        Debug.Log(z);
        return new Vector3(x, y, z);
    }

    // 生成最大数
    private int GetCreateLogoMax() {
        return level * levelweight;
    }

    // 生成
    void LogoCreate() {
        alpha = 0;
        for (int i = 0; i < GetCreateLogoMax(); i++) {

            //インスタンスを作成
            GameObject CopyObj = Instantiate(Logo);
            var Pos = GetRandomPosition();
            CopyObj.transform.position = (Vector2)Pos;
            CopyObj.transform.Rotate(new Vector3(0f, 0f, Pos.z));
            CopyObj.tag = "Untagged";
            LogoList.Add(CopyObj);

            if (i == GetCreateLogoMax() - 1) {
                LogoList[i].tag = "Target";
                LogoList[i].GetComponent<SpriteRenderer>().color = new Color32(1, 1, 1, 0);
            }
        }
    }

    // 削除
    void LogoDelete() {

        for (int i = 0; i < LogoList.Count; i++) {
            Destroy(LogoList[i]);
        }
        LogoList.Clear();

    }

    void GameMain() {

        // 徐々に表示
        if (alpha < 255) alpha++;
        LogoList[GetCreateLogoMax() - 1].GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, alpha);


        // クリック時
        if (Input.GetMouseButtonDown(0)) {
            Vector2 touchPosition = new Vector2(0, 0);

            // エディターでのクリック
            if (Input.GetMouseButtonDown(0)) {
                touchPosition = Input.mousePosition;
            }
            else {
                // 端末でのタップ
                Touch touch = Input.GetTouch(0);
                touchPosition = touch.position;
            }
            TouchAct(touchPosition);
        }

    }

    // タッチクリック時
    private void TouchAct(Vector2 pos) {

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos);

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(worldPoint, Vector2.zero)) {
            // ターゲットを見つけた時
            if (hit && hit.collider.tag == "Target") {
                LogoDelete();
                level++;
                LogoCreate();
                return;
            }

        }

        // 失敗
        gstate = gamestate.OVER;
        GameOverPanel.SetActive(true);
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(level);
    }

    void GameOver() {
        LogoDelete();
        gstate = gamestate.TITLE;
    }

    // 釦押下時
    public void OnClickGameMain() {
        level = 1;
        gstate = gamestate.MAIN;
        LogoCreate();
        TitlePanel.SetActive(false);
        GameOverPanel.SetActive(false);
    }

}
