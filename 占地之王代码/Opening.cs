using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class Opening : MonoBehaviour
{  
    [Header("选择地图界面")]
    public GameObject MapMenu;
    [Header("选择角色界面")]
    public GameObject PlayerMenu;
    [Header("设置界面")]
    public GameObject SettingMenu;
    [Header("准备界面")]
    public GameObject Ready;
    [Header("排行榜")]
    public GameObject Ranking;
    [Header("初始选择ui")]
    public Button StartSelect;

    /// <summary>
    /// Ui栈
    /// </summary>
    private Stack<GameObject> uiStack;
    //返回UI栈顶物体
    public Transform StackPeek => uiStack.Peek().transform; 
    public List<Image> Kuangs;
    public List<Sprite> spritesforKuang;
    public List<Button> Buttons;
    public List<Text> Texts;

    private void Awake()
    {
        Ready = PlayerManager.Instance.transform.GetChild(0).GetChild(0). gameObject;
        Kuangs[0] = PlayerManager.Instance.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetComponent<Image>();
        Kuangs[1] = PlayerManager.Instance.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>();
        Kuangs[2] = PlayerManager.Instance.transform.GetChild(0).GetChild(1).GetChild(2).GetChild(1).GetComponent<Image>();
        Kuangs[3] = PlayerManager.Instance.transform.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetComponent<Image>();
    }
    private void Start()
    {
        uiStack = new Stack<GameObject>();
        StartSelect.Select();
        uiStack.Push(transform.GetChild(0).gameObject);
    }
    private void Update()
    {
        WithDraw();
        ShowMapMenu();
    }
    /// <summary>
    /// ui返回上一层
    /// </summary>
    private void WithDraw()
    {
        var gamepad = Gamepad.current;
        if (gamepad==null)
        {
            return;
        }
        else
        {
           if(gamepad.bButton.wasPressedThisFrame)
            {
                if (uiStack.Count <= 1) return;
                else if (StackPeek.name == "Ready") return;
                else
                {
                    GameObject temp1 = uiStack.Pop();
                    StackPeek.gameObject.SetActive(true);
                    if (temp1.name == "ChooseChar")
                    {   
                        for(int i=0;i<Buttons.Count;i++)
                        {
                            Buttons[i].interactable = true;
                            Kuangs[i].sprite = Resources.Load<Sprite>("R-C_0");
                        }
                        Kuangs[0].transform.parent.parent.gameObject.SetActive(false);
                        PlayerManager.Instance.Clear();
                    }
                    if (temp1.name == "ChooseMap")
                    {
                        PlayerManager.Instance.BeginPlaying -= PlayerManager.Instance.LoadPigEvent;
                        PlayerManager.Instance.BeginPlaying -= PlayerManager.Instance.LoadIceEvent;
                        PlayerManager.Instance.BeginPlaying -= PlayerManager.Instance.LoadCandyEvent;
                    }
                    temp1.SetActive(false);
                    SetSelect(StackPeek.transform.GetChild(0));
                }
            }
        }
    }

    /// <summary>
    /// ui改变后设置选择
    /// </summary>
    /// <param name="temp2"></param>
    private void SetSelect(Transform temp)
    {
        if (PlayerManager.Instance.multiplayerEventSystems[0] == null) return;
        if (temp.TryGetComponent(out Button b)) 
        {
            if (b.interactable)
            {
                PlayerManager.Instance.multiplayerEventSystems[0].SetSelectedGameObject(b.gameObject);
            }
        }
        else if (temp.TryGetComponent(out Slider s)) PlayerManager.Instance.multiplayerEventSystems[0].SetSelectedGameObject(s.gameObject);
    }

    /// <summary>
    /// 开始游戏选玩家
    /// </summary>
    public void StartGame()
    {
        StackPeek.gameObject.SetActive(false);
        PlayerMenu.SetActive(true);
        SetSelect(PlayerMenu.transform.GetChild(0));
        Kuangs[0].gameObject.transform.parent.parent.gameObject.SetActive(true);
        uiStack.Push(PlayerMenu);
        PlayerInputManager.instance.EnableJoining();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 选择地图
    /// </summary>
    private void ShowMapMenu()
    {
        if (StackPeek.name == "ChooseChar")
        {
            bool playeready = true;
            for (int i = 0; i < 4; i++)
            {
                if (!PlayerManager.Instance.existChar[i])
                {
                    playeready = false;
                    break;
                }
            }
            if(playeready)
            {
                StackPeek.gameObject.SetActive(false);
                MapMenu.SetActive(true);
                uiStack.Push(MapMenu);
                for(int i=0;i< PlayerManager.Instance.multiplayerEventSystems.Count;i++)
                    PlayerManager.Instance.multiplayerEventSystems[i].sendNavigationEvents = true;
                SetSelect(MapMenu.transform.GetChild(0));
                Kuangs[0].gameObject.transform.parent.parent.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 选择猪图
    /// </summary>
    public void ChoosePig()
    {
        PlayerManager.Instance.BeginPlaying += PlayerManager.Instance.LoadPigEvent;
        StackPeek.gameObject.SetActive(false);
        uiStack.Push(Ready);
        Ready.SetActive(true);
        Kuangs[0].gameObject.transform.parent.parent.gameObject.SetActive(true);
        PlayerManager.Instance.MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Pigmap");
        SceneManager.LoadScene("PigMapTest", LoadSceneMode.Additive);
        PlayerManager.Instance.SetSelectInReadyMenu();
    }

    /// <summary>
    /// 选择冰图
    /// </summary>
    public void ChooseIce()
    {
        PlayerManager.Instance.BeginPlaying += PlayerManager.Instance.LoadIceEvent;
        StackPeek.gameObject.SetActive(false);
        uiStack.Push(Ready);
        Ready.SetActive(true);
        Kuangs[0].gameObject.transform.parent.parent.gameObject.SetActive(true);
        PlayerManager.Instance.MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Icymap");
        SceneManager.LoadScene("IcyLandTest", LoadSceneMode.Additive);
        PlayerManager.Instance.SetSelectInReadyMenu();
    }

    /// <summary>
    /// 选择糖图
    /// </summary>
    public void ChooseCandy()
    {
        PlayerManager.Instance.BeginPlaying += PlayerManager.Instance.LoadCandyEvent;
        StackPeek.gameObject.SetActive(false);
        uiStack.Push(Ready);
        Ready.SetActive(true);
        Kuangs[0].gameObject.transform.parent.parent.gameObject.SetActive(true);
        PlayerManager.Instance.MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Candymap");
        SceneManager.LoadScene("CandyLandTest", LoadSceneMode.Additive);
        PlayerManager.Instance.SetSelectInReadyMenu();
    }

    /// <summary>
    ///选择空白图
    /// </summary>
    public void ChooseBlank()
    {
        PlayerManager.Instance.BeginPlaying += PlayerManager.Instance.LoadBlankEvent;
        StackPeek.gameObject.SetActive(false);
        uiStack.Push(Ready);
        Ready.SetActive(true);
        Kuangs[0].gameObject.transform.parent.parent.gameObject.SetActive(true);
        PlayerManager.Instance.MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Blank");
        SceneManager.LoadScene("Blank", LoadSceneMode.Additive);
        PlayerManager.Instance.SetSelectInReadyMenu();
    }

    /// <summary>
    /// 排行榜
    /// </summary>
    public void RankingBoard()
    {
        DataBase.Instance.ShowRanking(Texts);
        Ranking.SetActive(true);
        uiStack.Push(Ranking);
    }

    /// <summary>
    /// 设置
    /// </summary>
    public void ShowSettingMenu()
    {
        SettingMenu.SetActive(true);
        SetSelect(SettingMenu.transform.GetChild(0));
        uiStack.Push(SettingMenu);
    }

    public void ShowControl()
    {
        Transform t= uiStack.Peek().transform.GetChild(2).GetChild(0);
        t.gameObject.SetActive(true);
        uiStack.Push(t.gameObject);
    }


}
              