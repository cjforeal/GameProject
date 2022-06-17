using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("obstacle")]
    public Tilemap obstacleTilemap;
    [Header("ObjectOverGround")]
    public Tilemap tilemap;
    [Header("道具数量上限")]
    public int propLimit;
    [HideInInspector]
    public int propNow;
    [Header("道具刷新时间")]
    public float propTime;
    [Header("游戏时长")]
    public float gameLength;
    [Header("道具种类")]
    public List<GameObject> Props;
    [Header("数字图片")]
    public List<Sprite> Nums;
    [Header("个位数")]
    public Image onesDigit;
    [Header("十位数")]
    public Image tensDigit;
    [Header("是否开启疯狂模式")]
    public bool CrazyGameOn;
    [HideInInspector]
    public float x;
    [HideInInspector]
    public float y;
    public static Vector3 pianyi;

    // Start is called before the first frame update
    private void OnEnable()
    {
        InvokeRepeating("ShowProps", 7, propTime);
    }
    public virtual void Update()
    {
        gameLength -= Time.deltaTime;
        if (gameLength < 0 && PlayerManager.Instance.Ining > 1) StartCoroutine(Restart());
        else if (gameLength < 0 && PlayerManager.Instance.Ining <= 1) StartCoroutine(GameOver());
        ShowTime();
        GotoNextGame();
    }
    //防止道具生成在建筑物上
    public void ShowProps()
    {
        if (propNow < propLimit)
        {
            while (true)
            {
                x = Random.Range(-7, 9);
                y = Random.Range(-5, 4);
                x = x > 0 ? x - 0.5f : x + 0.5f;
                y = y > 0 ? y - 0.5f : y + 0.5f;
                if (!tilemap.GetTile(tilemap.WorldToCell(new Vector3(x, y, 0))) && !obstacleTilemap.GetTile(obstacleTilemap.WorldToCell(new Vector3(x, y, 0))))
                    break;
            }
            Instantiate(Props[Random.Range(0, 4)], new Vector3(x, y), Quaternion.identity);
            propNow++;
        }
    }
    //显示倒计时
    public void ShowTime()
    {
        if(gameLength>=0&&gameLength<100)
        {
            int i = (int)gameLength;
            onesDigit.sprite = Nums[(i%10)];
            i = i / 10;
            tensDigit.sprite = Nums[i];          
        }
    }
    public IEnumerator Restart()
    {
        Time.timeScale = 0;
        JudgeWin();
        PlayerManager.Instance.OneGameOver.SetActive(true);
        gameLength = 100000;
        yield return new WaitForSecondsRealtime(6f);
        PlayerManager.Instance.OneGameOver.SetActive(false);
        PlayerManager.Instance.GameOver.SetActive(true);
        InstantiateCrown();
    }

    public IEnumerator GameOver()
    {
        Time.timeScale = 0;
        JudgeWin();
        PlayerManager.Instance.GameOver.SetActive(true);
        InstantiateCrown();
        yield return new WaitForSecondsRealtime(5f);
        PlayerManager.Instance.GameOver.SetActive(false);
        Time.timeScale = 1;
        PlayerManager.Instance.Clear();
        SceneManager.LoadScene("Prepare");
    }
    private void InstantiateCrown()
    {
        int i = DataBase.Instance.Ining;
        if (DataBase.Instance.winner[i] == "GreenShit")
        {
            Instantiate(PlayerManager.Instance.winFlag, PlayerManager.Instance.winFlag.transform.position + pianyi, Quaternion.identity,PlayerManager.Instance.GameOver.transform);
        }
        else if (DataBase.Instance.winner[i] == "PinkShit")
        {
            pianyi += new Vector3(0, -85);
            Instantiate(PlayerManager.Instance.winFlag, PlayerManager.Instance.winFlag.transform.position + pianyi, Quaternion.identity, PlayerManager.Instance.GameOver.transform);
        }
        else if (DataBase.Instance.winner[i] == "Bird")
        {
            pianyi += new Vector3(0, -85 * 2);
            Instantiate(PlayerManager.Instance.winFlag, PlayerManager.Instance.winFlag.transform.position + pianyi, Quaternion.identity, PlayerManager.Instance.GameOver.transform);
        }
        else if (DataBase.Instance.winner[i] == "Ghost")
        {
            pianyi += new Vector3(0, -85 * 3);
            Instantiate(PlayerManager.Instance.winFlag, PlayerManager.Instance.winFlag.transform.position + pianyi, Quaternion.identity, PlayerManager.Instance.GameOver.transform);
        }
        pianyi += new Vector3(150, -pianyi.y);

        DataBase.Instance.Ining++;
        PlayerManager.Instance.Ining--; 
    }
    private void GotoNextGame()
    {
        if (PlayerManager.Instance.Ining>0&&PlayerManager.Instance.GameOver.activeSelf)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return;
            if (Gamepad.current.bButton.wasPressedThisFrame)
            {
                PlayerManager.Instance.GameOver.SetActive(false);
                PlayerManager.Instance.Clear();
                Time.timeScale = 1;
                SceneManager.LoadScene("Prepare");
            }
            else if (Gamepad.current.aButton.wasPressedThisFrame)
            {
                PlayerManager.Instance.GameOver.SetActive(false);
                Time.timeScale = 1;
                PlayerManager.Instance.restart = true;
            }
        }
    }
    //判断获胜玩家并将玩家对局信息存入数据库
    private void JudgeWin()
    {
        int score = PlayerManager.Instance.playingPlayers[0].GetComponent<BasicPlayer>().scores;
        PlayerManager.Instance.playingPlayers[0].GetComponent<BasicPlayer>().win = 1;
        for (int i = 1; i < PlayerManager.Instance.playingPlayers.Count; i++)
        {
            BasicPlayer p = PlayerManager.Instance.playingPlayers[i].GetComponent<BasicPlayer>();
            if (p.scores > score)
            {
                score = p.scores;
                p.win = 1;
                PlayerManager.Instance.playingPlayers[i - 1].GetComponent<BasicPlayer>().win = 0;
            }
        }
        string winnername = "";
        for (int i = 0; i < PlayerManager.Instance.playingPlayers.Count; i++)
        {
            BasicPlayer b = PlayerManager.Instance.playingPlayers[i].GetComponent<BasicPlayer>();
            string temp = b.name.Substring(0, b.name.Length - 7);
            DataBase.Instance.SaveData(temp, temp, DataBase.Instance.Ining,
                b.scores, b.win);
            PlayerManager.Instance.Texts[i].text = b.scores.ToString();
            if (b.win == 1) winnername = b.gameObject.name.Substring(0, b.name.Length - 7);
        }
        DataBase.Instance.winner.Add(winnername);
    }
}
