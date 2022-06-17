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
    /// �������豸������̬����Ϸ������ǵù���
    /// </summary>
    static int DeviceIdCount;
    public static int DeviceIDCount { get { return DeviceIdCount; } set{ DeviceIdCount = value; } }

    /// <summary>
    /// ��ǰ�����豸ID
    /// </summary>
    private int DeviceId;

    /// <summary>
    /// ui�¼�ϵͳ
    /// </summary>
    private MultiplayerEventSystem eventSystem;

    /// <summary>
    /// ��ҹ��Ԥ���壨��̬���أ�
    /// </summary>
    private GameObject flagPre;

    /// <summary>
    /// ʵ�������
    /// </summary>
    private GameObject flag;

    /// <summary>
    /// ����UI�ű�
    /// </summary>
    private Opening Opening;

    /// <summary>
    /// ѡ���ɫ����ʱ��Ӧ��ͷ���
    /// </summary>
    private Image Kuang;

    /// <summary>
    /// ѡ���ɫ����Ĺ��ƫ�����������ĸ���ҹ���ص�
    /// </summary>
    private int pianyi;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        #region ��ȡ�����豸�ļ�������
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
    /// ���ѡ���ɫ��
    /// �˴����ð�ť�ĵ���ص��¼���ԭ���������ʹ��
    /// ÿ����Ҷ���Ҫ���ĸ���ɫ��ť��̬�󶨵������
    /// ������ÿ�ε��һ�θð�ť�°󶨵��ĸ�����������ã����ο��¼��ڵĺ�����������޷�����
    /// ����ͨ����Ϊ���İ취
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
    /// �ƶ���ҹ��
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
    /// ѡ���������ʱ
    /// ��һ�����ѡ���ɫ����ѡ��ťInteractable�������ʱ���������ѡ����Ҳ�Ǹ����壬�������Ui������ʧЧ
    /// �˺�����Ϊ�˱�����һ������Զ�ѡ���밴ť����List��Ŀ�ѡ��ť
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
    //����Ui��Ч
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
