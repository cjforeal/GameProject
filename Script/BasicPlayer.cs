using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
/// <summary>
/// �����ǩö��
/// </summary>
public enum PlayerFlag
{
    NoHost,
    Player1,
    Player2,
    Player3,
    Player4
}

/// <summary>
/// ��ɫ��������ƶ�������״̬����ɫ���
/// </summary>
public class BasicPlayer : MonoBehaviour
{   
    protected Animator animPlayer;
    protected Rigidbody2D rigidBody;
    private Transform Canvas;
    protected Vector2 inputMove;
    protected Tilemap tilemap;

    protected Vector3[] vector3;

    /// <summary>
    /// ��ͣ����
    /// </summary>
    private GameObject stop;

    /// <summary>
    /// �Ƿ��ʤ
    /// </summary>
    [HideInInspector]
    public int win;

    /// <summary>
    /// ��ɫ���������
    /// </summary>
    protected PlayerFlag oldTileType;

    /// <summary>
    /// ��ұ�ǩ
    /// </summary>
    private PlayerFlag playerflag;
    public PlayerFlag playerFlag { get { return playerflag; } set { playerflag = value; } }

    /// <summary>
    /// ��̬�ֵ�洢��ɫ�Ͷ�Ӧ��ɫ������
    /// </summary>
    public static Dictionary<PlayerFlag, TileBase[][]> playTile;

    /// <summary>
    /// �������鴢���ɫɫ��
    /// </summary>
    public TileBase[][] Tiles;

    [HideInInspector]
    public int moveMode;

    [HideInInspector]
    public int scores;

    /// <summary>
    /// ���������
    /// </summary>
    [HideInInspector]
    public Vector2 Forward;


    [Header("�ٶ�")]
    public int speed;

    [Header("��ѣʱ��")]
    public float dizzyTime;

    [Header("��Ь�ڳ���ʱ��")]
    public float boostTime;

    [Header("ҩ����ʱ��")]
    public float yaoTime;

    [Header("ը����Ϳ��Ƭ����")]
    public int tileNumBomb;

    [Header("����������")]
    public Image bagBackGround;
    [Header("������")]
    public List<Image> PropsBag;
    [Header("�õ������Ƿ���ڵ���")]
    public bool[] propExist;

    [Header("���߷���")]
    public GameObject biao;

    [HideInInspector][Header("���ڻ���")]
    public bool AttackedByBiao;

    [HideInInspector][Header("����������Ƭ")]
    public TileBase[][] AttackerTile;

    [HideInInspector][Header("͸��ҩˮ��Ч")]
    public bool PolluteByYao;

    [Header("�޵�֡ʱ��")]
    public float unstop;
    protected float _unstop;
    
    //�޵�֡
    [HideInInspector]
    public bool UnStopable;


    private void OnEnable()
    {       
        vector3 =new Vector3[]{ new Vector3(0,0.5f,0), new Vector3(0.5f, 0, 0), new Vector3(0, -0.5f, 0),
                 new Vector3(-0.5f, 0, 0), new Vector3(0.5f, 0.5f, 0),new Vector3(0.5f, -0.5f, 0), 
                 new Vector3(-0.5f, -0.5f, 0),new Vector3(-0.5f, 0.5f, 0),};
        Canvas = GameObject.FindWithTag("Canvas").transform;
        bagBackGround = Instantiate(bagBackGround, Canvas);
        PropsBag[0] = bagBackGround.transform.GetChild(0).GetComponent<Image>();
        PropsBag[1] = bagBackGround.transform.GetChild(1).GetComponent<Image>();
        if (playTile==null)
        {    
            playTile=new Dictionary<PlayerFlag,TileBase[][]> ();
            stop= Resources.Load<GameObject>("UI/SettingMenu");
            stop = Instantiate(stop, Canvas);
            stop.SetActive(false);
        }
        _unstop = unstop;
    }
    public void Update()
    {
        ShowScores();
        if(UnStopable)
        {
            unstop -= Time.deltaTime;
            if (unstop < 0)
            {
                UnStopable = false;
                unstop = _unstop;
            }           
        }
    }

    /// <summary>
    /// �ƶ��������ֹͣ�ƶ�ʱ ����ҵ���ǰ��������Ӧ������������
    /// </summary>
    /// <param name="ctx"></param>
    public void Movement(InputAction.CallbackContext ctx)
    {       
        if(ctx.performed)
        {
            inputMove = ctx.ReadValue<Vector2>();
            animPlayer.SetFloat("Vertical", inputMove.y);
            animPlayer.SetFloat("Horizonal", inputMove.x);
            Forward = inputMove;

        }
        if(ctx.canceled)
        {
            float angle = Vector2.SignedAngle(Vector2.right, inputMove);
            if (angle > -45 && angle <= 45) Forward = Vector2.right;
            else if (angle > 45 && angle <= 135) Forward = Vector2.up;
            else if (angle > 135 || angle <= -135) Forward = Vector2.left ;
            else if(angle > -135 && angle <= -45) Forward = Vector2.down;
            animPlayer.SetFloat("Vertical", Forward.y);
            animPlayer.SetFloat("Horizonal", Forward.x);
            inputMove = new Vector2(0, 0);
        }
    }

    /// <summary>
    ///ʹ�õ���1
    /// </summary>
    /// <param name="ctx"></param>
    public void Prop1(InputAction.CallbackContext ctx)
    {
        if (propExist[0]&&ctx.started)
        {
            UseProp(PropsBag[0].transform.GetChild(0).gameObject);
            Destroy(PropsBag[0].transform.GetChild(0).gameObject);
            propExist[0] = false;
        }
    }

    /// <summary>
    /// ʹ�õ���2
    /// </summary>
    /// <param name="ctx"></param>
    public void Prop2(InputAction.CallbackContext ctx)
    {
        if(propExist[1] && ctx.started)
        {
            UseProp(PropsBag[1].transform.GetChild(0).gameObject);
            Destroy(PropsBag[1].transform.GetChild(0).gameObject);
            propExist[1] = false;
        }
    }

    /// <summary>
    /// ��ͣ��δʵװ��
    /// </summary>
    /// <param name="ctx"></param>
    public void Stop(InputAction.CallbackContext ctx)
    {   
        if(ctx.started)
        {  
        //    if(!isStop)
        //    {
        //        Time.timeScale = 0;
        //        stop.SetActive(true);
        //    }
        //    else
        //    {
        //        Time.timeScale = 1;
        //        stop.SetActive(false);
        //    }
        }
        
    }
    /// <summary>
    /// �ж����������Ƭ
    /// </summary>
    public void JudegNowPos(TileBase[][] tiles,Vector3 pos,ref PlayerFlag oldFlag)
    {
        TileBase tile = tilemap.GetTile(tilemap.WorldToCell(pos));
        if (!ContainsTile(tiles, tile))
        {
            scores++;
            foreach (PlayerFlag p in playTile.Keys)
            {
                if (p == this.playerFlag) continue;
                if (ContainsTile(playTile[p],tile))
                {
                    PlayerManager.Instance.playingPlayers[(int)p-1].GetComponent<BasicPlayer>().scores--;
                    oldFlag = p;
                }
            }
        }
    }

    /// <summary>
    /// �ķ�λ�ж�
    /// </summary>
    /// <param name="pos"></param>
    public  void TestFloor(TileBase[][] tiles,Vector3 pos)
    {
        int[] dir = { 0, 0, 0, 0 };
        int num = 0;
        for(int i=0;i<4; i++)
        {
            if (ContainsTile(tiles, tilemap.GetTile(tilemap.WorldToCell(pos+vector3[i]))))
            {
                dir[i]++;
                num++;
            }
        }
        switch (num)
        {
            case 0: tilemap.SetTile(tilemap.WorldToCell(pos), tiles[num][0]); break;
            case 1:
                int i;
                for (i = 0; i < 4; i++)
                {
                    if (dir[i] != 0) break;
                }
                tilemap.SetTile(tilemap.WorldToCell(pos), tiles[num][i]);
                break;
            case 2:
                if (dir[0] == dir[2]&&dir[0]==1)      tilemap.SetTile(tilemap.WorldToCell(pos), tiles[num][0]); 
                else if (dir[1] == dir[3]&&dir[1]==1) tilemap.SetTile(tilemap.WorldToCell(pos), tiles[num][1]); 
                else 
                {
                    int k ;
                    bool exist = false;
                    for ( k = 0; k < 4; k++)
                    {
                        if (dir[k%4] == dir[(k + 1)%4]&&dir[k]==1)
                        {
                            exist = ContainsTile(tiles, tilemap.GetTile(tilemap.WorldToCell(pos + vector3[k+4])));
                            break;
                        }  
                    }               
                    if(!exist) tilemap.SetTile(tilemap.WorldToCell(pos), tiles[5][k]);
                    else tilemap.SetTile(tilemap.WorldToCell(pos), tiles[6][k]);
                }
                break;
            case 3:
                int j ;
                for (j = 0; j < 4; j++)
                {
                    if (dir[j] == 0) break;
                }
                tilemap.SetTile(tilemap.WorldToCell(pos), tiles[num][j]);
                break;
            case 4: tilemap.SetTile(tilemap.WorldToCell(pos), tiles[num][0]); break;
        }
    }
    /// <summary>
    /// �жϸ���Ƭ�Ƿ������Լ�
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public  bool ContainsTile(TileBase[][] tiles,TileBase tile)
    {
        for (int i = 0; i < 7;i++)
        {
            for(int j=0;j<tiles[i].Length;j++)
            {   
                //Debug.Log(tiles[i][j]);
                if (tiles[i][j] == tile) return true;
            }
        }
        return false;
    }
    /// <summary>
    /// ����Χ8���������Ƭ�����Լ�����Ƭ������ͬ����Ƭ����������Ϳ
    /// </summary>
    public void TestAroundFloor(TileBase[][] tiles,Vector3 pos ,PlayerFlag oldFlag)
    {
        for(int i=0;i<vector3.Length;i++)
        {
            Vector3 vector = pos + vector3[i];
            TileBase tile = tilemap.GetTile(tilemap.WorldToCell(vector));
            if (ContainsTile(tiles, tile))
            {
                TestFloor(tiles, vector);
            }
            else if(oldFlag != PlayerFlag.NoHost&& ContainsTile(playTile[oldFlag],tile))
            {
                TestFloor(playTile[oldFlag],vector);
            }
        }
    }
    /// <summary>
    /// ������Ƭ�����ڽ���������
    /// </summary>
    /// <param name="path"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public  void LoadTile(string path,int i,int j)
    {
        TileBase tile = Resources.Load<TileBase>(path);
        Tiles[i][j] = tile;
        //Debug.Log(tile);
    }
    /// <summary>
    /// ѣ������
    /// </summary>
    public void WakeUp()
    {   
        dizzyTime -= Time.fixedDeltaTime; 
        if(dizzyTime<0)
        {
            animPlayer.SetBool("Dizzy", false);
            animPlayer.SetBool("Stuck", false);
            dizzyTime = 1;
        }
        UnStopable = true;
    }
    /// <summary>
    /// �ƶ�ģʽ
    /// </summary>
    /// <param name="m"></param>
    public void MoveByMoveMode(int m)
    {
        if(m==0) rigidBody.MovePosition(rigidBody.position + inputMove * speed * Time.fixedDeltaTime);
        else rigidBody.AddForce(inputMove * speed );
    }
    /// <summary>
    /// ʹ�õ���
    /// </summary>
    /// <param name="image">������</param>
    public void UseProp(GameObject image)
    {
        if(image.CompareTag("Biao"))
        {   
            float angle= Vector2.SignedAngle(transform.right,Forward);
            GameObject temp= Instantiate(biao, transform.position, Quaternion.Euler(0, 0, angle));
            temp.GetComponent<Biao>().biaoParent = this;
            if (!Bkmuisic.Instance.audioSources[(int)playerFlag].isPlaying)
            {
                Bkmuisic.Instance.audioSources[(int)playerFlag].PlayOneShot(Bkmuisic.Instance.playersAuido[1]);
            }
        }
        if (image.CompareTag("Bomb"))
        {
            int i=0;
            if (!Bkmuisic.Instance.audioSources[(int)playerFlag].isPlaying)
            {
                Bkmuisic.Instance.audioSources[(int)playerFlag].PlayOneShot(Bkmuisic.Instance.playersAuido[3]);
            }
            while ( i < tileNumBomb)
            {
                int x = Random.Range(-7, 9);
                int y = Random.Range(-3, 4);
                Vector3 temp = new Vector3(x, y);
                JudegNowPos(Tiles, temp, ref oldTileType);
                TestFloor(Tiles, temp);
                TestAroundFloor(Tiles, temp, oldTileType);
                i++;
            }
        }
        if (image.CompareTag("Yao"))
        {
            StartCoroutine(Yao());
            if (!Bkmuisic.Instance.audioSources[(int)playerFlag].isPlaying)
            {
                Bkmuisic.Instance.audioSources[(int)playerFlag].PlayOneShot(Bkmuisic.Instance.playersAuido[4]);
            }
        }
        if(image.CompareTag("Boost"))
        {
            StartCoroutine(Boost());
            if (!Bkmuisic.Instance.audioSources[(int)playerFlag].isPlaying)
            {
                Bkmuisic.Instance.audioSources[(int)playerFlag].PlayOneShot(Bkmuisic.Instance.playersAuido[2]);
            }
        }
    }
    public void ShowScores()
    {
        bagBackGround.transform.GetChild(2).GetComponent<Text>().text = scores.ToString();
    }
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Pig"))
        {   
            if(!UnStopable)
            {
                animPlayer.SetBool("Dizzy", true);
                if (!Bkmuisic.Instance.audioSources[(int)playerFlag].isPlaying)
                {
                    Bkmuisic.Instance.audioSources[(int)playerFlag].PlayOneShot(Bkmuisic.Instance.playersAuido[5]);
                }
                UnStopable = true;
            }
        }
        if(other.CompareTag("Icy"))
        {
            moveMode = 1;
        }
    }
    public virtual void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Icy"))
        {
            moveMode = 0;
            rigidBody.velocity = Vector2.zero;
        }
    }

   public IEnumerator Boost()
    {
        speed = 10;
        GameObject BoostEffect = Resources.Load<GameObject>("Effect/BoostEffect");
        Destroy(Instantiate(BoostEffect, transform.position, Quaternion.identity,transform), boostTime);
        gameObject.layer = LayerMask.NameToLayer("Boost");
        yield return new WaitForSeconds(boostTime);
        speed = 5;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public virtual IEnumerator Biao(TileBase[][] tiles)
    {
        AttackedByBiao = true;
        AttackerTile = tiles;
        yield return new WaitForSeconds(boostTime);
        AttackedByBiao = false;
        AttackerTile = null;
    }
    public virtual IEnumerator Yao()
    {   
        foreach(GameObject g in PlayerManager.Instance.playingPlayers)
        {
            if(g!=this.gameObject)
            {
                GameObject YaoEffect = Resources.Load<GameObject>("Effect/YaoEffect");
                Destroy(Instantiate(YaoEffect, g.transform.position,Quaternion.identity,g.transform), yaoTime);
                g.GetComponent<BasicPlayer>().PolluteByYao = true;
            }
        }
        yield return new WaitForSeconds(yaoTime);
        foreach (GameObject g in PlayerManager.Instance.playingPlayers)
        {
            if (g != this.gameObject)
            {
                g.GetComponent<BasicPlayer>().PolluteByYao =false;
            }
        }
    }

}
