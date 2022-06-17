using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class PlayerManager : MonoBehaviour
{   
    public static PlayerManager Instance { get; private set; }
    /// <summary>
    /// �����豸
    /// </summary>
    public InputDevice[] inputDevices;
    /// <summary>
    ///��ʼ��Ϸ�¼�
    /// </summary>
    public event UnityAction BeginPlaying;

    private bool judgeFirstPlayer;

    [Header("��Ϸ����")]
    public int Ining;

    [Header("�Ƿ�ʼ��Ϸ")]
    public bool preready;

    [Header("�Ƿ�����һ��")]
    public bool restart;


    [Header("�غϽ�������ʤ����־")]
    public GameObject winFlag;

    [Header("���ֽ������㻭��")]
    public GameObject OneGameOver;

    [Header("��Ϸ��������")]
    public GameObject GameOver;

    [Header("��ͼ����")]
    public Image MapIntroduction;

    [Header("��ѡ��Ľ�ɫ")]
    public List<GameObject> players;

    [Header("���ѡ����������豸ƥ��Ľ�ɫ����")]
    public List<GameObject> joinPlayers;

    [Header("��ʽ��Ϸʱʵ���������")]
    public List<GameObject> playingPlayers;

    [Header("������ҵ�ui������")]
    public List<MultiplayerEventSystem> multiplayerEventSystems;

    [Header("׼������׼��")]
    public List<Button> Buttons;

    [Header("��������Ƿ�׼��")]
    public bool[] isReady;

    [Header("��������Ƿ�ѡ�����ɫ")]
    public bool[] existChar;

    [Header("���ֽ�������ʾ�÷ֵķ����ı�")]
    public List<Text> Texts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    void Start()
    {   
        judgeFirstPlayer = true;
        inputDevices = new InputDevice[4];
        //PlayerInputManager.instance.onPlayerJoined += Instance_onPlayerJoined;          
    }
    void Update()
    {   
        PlayersReady();
        Restart();
        if (judgeFirstPlayer)
        {
            if (PlayerInputManager.instance.playerCount == 1)
            {
                PlayerInputManager.instance.DisableJoining();
                judgeFirstPlayer = false;
            }
        }
    }

    #region �����ѡ���ͼ��ΪBeginPlay�¼���ӵĳ����첽����ί��
    public void LoadPigEvent()
    {
        AsyncOperation aa = SceneManager.LoadSceneAsync("PigMap");
        aa.completed += EventAfterSceneLoaded;
    }
    public void LoadIceEvent()
    {
        AsyncOperation aa = SceneManager.LoadSceneAsync("IcyLand");
        aa.completed += EventAfterSceneLoaded;
    }
    public void LoadCandyEvent()
    {
        AsyncOperation aa = SceneManager.LoadSceneAsync("CandyLand");
        aa.completed += EventAfterSceneLoaded;
    }
    public void LoadBlankEvent()
    {
        AsyncOperation aa = SceneManager.LoadSceneAsync("Blank");
        aa.completed += EventAfterSceneLoaded;
    }
    private void EventAfterSceneLoaded(AsyncOperation a)
    {
        if (BasicPlayer.playTile != null)
        {   
            BasicPlayer.playTile.Clear();
        }
        for (int j = 0; j < transform.GetChild(0).childCount; j++)
        {
            transform.GetChild(0).GetChild(j).gameObject.SetActive(false);
        }
        for (int i = 0; i < isReady.Length; i++)
        {
            isReady[i] = false;
            Buttons[i].GetComponent<Image>().sprite = Buttons[i].spriteState.highlightedSprite;
            if (joinPlayers[i] != null)
            {
                PlayerInput.Instantiate(joinPlayers[i], i, null, -1, inputDevices[i]);
            }
        }
        Bkmuisic.Instance.audioSources[0].clip = Bkmuisic.Instance.playingAudioClip;
        Bkmuisic.Instance.audioSources[0].Play();
        preready = false;
    }
    #endregion


    #region ���׼����ť�Ļص�����
    public void IsReadyPlayer1()
    {
        isReady[0] = true;
        Buttons[0].image.sprite = Buttons[0].spriteState.pressedSprite;
    }
    public void IsReadyPlayer2()
    {
        isReady[1] = true;
        Buttons[1].image.sprite = Buttons[1].spriteState.pressedSprite;
    }
    public void IsReadyPlayer3()
    {
        isReady[2] = true;
        Buttons[2].image.sprite = Buttons[2].spriteState.pressedSprite;
    }
    public void IsReadyPlayer4()
    {
        isReady[3] = true;
        Buttons[3].image.sprite = Buttons[3].spriteState.pressedSprite;
    }
    #endregion
    /// <summary>
    /// ׼������ʹ���ui����ѡ����Ե�׼����ť
    /// </summary>
    public void SetSelectInReadyMenu()
    {
        for (int i = 0; i <joinPlayers.Count; i++)
        {
            multiplayerEventSystems[i].SetSelectedGameObject(Buttons[i].gameObject);
        }
    }


    /// <summary>
    /// ��Ҿ�����ʼ��Ϸ
    /// </summary>
    private void PlayersReady()
    {
        preready = true;
        for(int i=0;i<isReady.Length;i++)
        {
            if (isReady[i] == false)
            {
                preready = false;
                break;
            }    
        }
        if (preready)
        {
            BeginPlaying();
        }
            
    }

    /// <summary>
    /// ���¿�ʼһ��
    /// </summary>
    private void Restart()
    {
        if (restart)
        {
            SceneManager.LoadScene("Empty");
            BeginPlaying -= LoadPigEvent;
            BeginPlaying -= LoadIceEvent;
            BeginPlaying -= LoadCandyEvent;
            BeginPlaying -= LoadBlankEvent;
            //��ʾReady����
            transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            SetSelectInReadyMenu();
            BasicPlayer.playTile.Clear();
            int x = Random.Range(0, 4);
            switch (x)
            {
                case 0:
                    BeginPlaying += LoadPigEvent;
                    MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Pigmap");
                    SceneManager.LoadScene("PigMapTest", LoadSceneMode.Additive);
                    break;
                case 1:
                    BeginPlaying += LoadIceEvent;
                    MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Icymap");
                    SceneManager.LoadScene("IcyLandTest", LoadSceneMode.Additive);
                    break;
                case 2:
                    BeginPlaying += LoadCandyEvent;
                    MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Candymap");
                    SceneManager.LoadScene("CandyLandTest", LoadSceneMode.Additive);
                    break;
                case 3:
                    BeginPlaying += LoadBlankEvent;
                    MapIntroduction.sprite = Resources.Load<Sprite>("MapIntroduction/Blank");
                    SceneManager.LoadScene("BlankTest", LoadSceneMode.Additive);
                    break;
                    
            }
            restart = false;
        }
    }

    /// <summary>
    /// ��Ϸ�����������
    /// </summary>
    public void Clear()
    {
        Ining = 5;
        for(int i=0;i<4;i++)
        {
            joinPlayers[i] = null;
            playingPlayers[i] = null;
            if(multiplayerEventSystems[i]!=null)
            {
                Destroy(multiplayerEventSystems[i].gameObject);
                multiplayerEventSystems[i] = null;
            }
            existChar[i] = false;
        }
        if(BasicPlayer.playTile!=null)     BasicPlayer.playTile.Clear();
        if(DataBase.Instance.winner!=null) DataBase.Instance.winner.Clear();
        UIController.DeviceIDCount = 0;
        Ining = 5;
        DataBase.Instance.Ining = 0;
        Bkmuisic.Instance.audioSources[0].clip = Bkmuisic.Instance.prepareAudioClip;
        MapManager.pianyi = Vector3.zero;
    }



    private void Instance_onPlayerJoined(PlayerInput obj)
    {
        switch (PlayerInputManager.instance.playerPrefab.name)
        {
            case "GreenShit": existChar[0] = true; break;
            case "PinkShit": existChar[1] = true; break;
            case "Bird": existChar[2] = true; break;
            case "Ghost": existChar[3] = true; break;
        }
        for (int i = 0; i < PlayerInputManager.instance.maxPlayerCount; i++)
        {
            if (!existChar[i])
            {
                PlayerInputManager.instance.playerPrefab = players[i];
                break;
            }
        }
    }
}
