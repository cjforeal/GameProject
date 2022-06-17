using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{   
    private PlayerInput playerInput;
    /// <summary>
    /// 总输入设备数，静态，游戏结束后记得归零
    /// </summary>
    static int DeviceIdCount;
    public static int DeviceIDCount { get { return DeviceIdCount; } set{ DeviceIdCount = value; } }

    /// <summary>
    /// 当前输入设备ID
    /// </summary>
    private int DeviceId;

    /// <summary>
    /// ui事件系统
    /// </summary>
    private MultiplayerEventSystem eventSystem;

    /// <summary>
    /// 玩家光标预制体（动态加载）
    /// </summary>
    private GameObject flagPre;

    /// <summary>
    /// 实例化光标
    /// </summary>
    private GameObject flag;

    /// <summary>
    /// 开场UI脚本
    /// </summary>
    private Opening Opening;

    /// <summary>
    /// 选择角色界面时对应的头像框
    /// </summary>
    private Image Kuang;

    /// <summary>
    /// 选择角色界面的光标偏移量，避免四个玩家光标重叠
    /// </summary>
    private int pianyi;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        #region 获取连接设备的几个方法
        //Debug.Log(playerInput.user.pairedDevices[0]);
        // Debug.Log(playerInput.devices[0] );
        //Debug.Log(playerInput.GetDevice<Gamepad>());
        #endregion
        DeviceIdCount++;
        DeviceId = DeviceIdCount - 1;
        playerInput = GetComponent<PlayerInput>();
        PlayerManager.Instance.inputDevices[DeviceId] = playerInput.user.pairedDevices[0];
        Opening = GameObject.FindWithTag("Canvas").GetComponent<Opening>();
        eventSystem = GetComponent<MultiplayerEventSystem>();
        PlayerManager.Instance.multiplayerEventSystems[DeviceId] = eventSystem;
        eventSystem.SetSelectedGameObject(Opening.StackPeek.GetChild(DeviceId).gameObject);
        Kuang = Opening.Kuangs[DeviceId];
        flagPre = Resources.Load<GameObject>("UI/P" + (DeviceId + 1));
        pianyi = 50 * (DeviceId);
        if(DeviceId!=0) flag=Instantiate(flagPre, Opening.PlayerMenu.transform);

    }
    private void OnEnable()=> playerInput.currentActionMap.actionTriggered += BindCharacter;

    /// <summary>
    /// 玩家选择角色绑定
    /// 此处不用按钮的点击回调事件，原因在于如果使用
    /// 每名玩家都需要给四个角色按钮动态绑定点击函数
    /// 以至于每次点击一次该按钮下绑定的四个函数都会调用，更何况事件内的函数调用与否无法控制
    /// 所以通过人为检测的办法
    /// </summary>
    /// <param name="ctx"></param>
    private void BindCharacter(InputAction.CallbackContext ctx)
    {   
        if(ctx.action.name=="Submit")
        {
            if (eventSystem.currentSelectedGameObject == null) return;
            else if (Opening.StackPeek.name== "ChooseChar" && ctx.started)
            {
                switch (eventSystem.currentSelectedGameObject.name[6])
                {
                    case '1': PlayerManager.Instance.joinPlayers[DeviceId] = PlayerManager.Instance.players[0]; break;
                    case '2': PlayerManager.Instance.joinPlayers[DeviceId] = PlayerManager.Instance.players[1]; break;
                    case '3': PlayerManager.Instance.joinPlayers[DeviceId] = PlayerManager.Instance.players[2]; break;
                    case '4': PlayerManager.Instance.joinPlayers[DeviceId] = PlayerManager.Instance.players[3]; break;
                }
                eventSystem.currentSelectedGameObject.GetComponent<Button>().interactable = false;
                eventSystem.sendNavigationEvents = false;
                PlayerManager.Instance.existChar[DeviceId] = true;
            }
        }
    }

    void Update()
    {
     
        if (Opening.StackPeek.name == "ChooseChar"&& eventSystem.sendNavigationEvents)
        {
            if (eventSystem.currentSelectedGameObject == null|| !eventSystem.currentSelectedGameObject.GetComponent<Button>().interactable)
            {
                SetSelectinChooseChar(Opening.Buttons[0], Opening.Buttons.Count);
                MoveFlag();
            }                        
            else
            {   
                if (flag == null)
                {
                    flag = Instantiate(flagPre, Opening.StackPeek);
                }
                MoveFlag();


            }
        }
        PlayYinXiao();
    }

    /// <summary>
    /// 移动玩家光标
    /// </summary>
    private void MoveFlag()
    {
        flag.transform.position = eventSystem.currentSelectedGameObject.transform.position + new Vector3(-50 + pianyi, -300, 0);
        int i = eventSystem.currentSelectedGameObject.name[6] - '0';
        switch (i)
        {
            case 1: Kuang.sprite = Opening.spritesforKuang[0]; break;
            case 2: Kuang.sprite = Opening.spritesforKuang[1]; break;
            case 3: Kuang.sprite = Opening.spritesforKuang[2]; break;
            case 4: Kuang.sprite = Opening.spritesforKuang[3]; break;
        }
    }

    /// <summary>
    /// 选择人物界面时
    /// 当一名玩家选择角色后所选按钮Interactable，如果此时其他玩家所选物体也是该物体，该名玩家Ui导航会失效
    /// 此函数是为了避免这一情况，自动选择传入按钮所属List里的可选按钮
    /// </summary>
    /// <param name="b"></param>
    /// <param name="length"></param>
    private void SetSelectinChooseChar(Button button,int length)
    {
        int i = button.gameObject.name[6]-'0';
        if (!button.interactable)
            SetSelectinChooseChar(Opening.Buttons[i%length],length);    
        else
            eventSystem.SetSelectedGameObject(button.gameObject);
    }
    //播放Ui音效
    private void PlayYinXiao()
    {   
        if(!PlayerManager.Instance.transform.GetChild(0).GetChild(0).gameObject.activeSelf&&SceneManager.GetActiveScene().name=="Prepare")
        {
            if (playerInput.currentActionMap.actions[0].ReadValue<Vector2>() != Vector2.zero && !Bkmuisic.Instance.audioSources[DeviceId + 1].isPlaying)
            {
                Bkmuisic.Instance.audioSources[DeviceId + 1].PlayOneShot(Bkmuisic.Instance.uiAudio[0]);
            }
        } 
        if (playerInput.currentActionMap.actions[4].ReadValue<float>() != 0&& !Bkmuisic.Instance.audioSources[DeviceId+1].isPlaying)
        {
            Bkmuisic.Instance.audioSources[DeviceId+1].PlayOneShot(Bkmuisic.Instance.uiAudio[1]);
        }
    }

    void OnDestroy()
    {
        Destroy(flag);       
    }



}
