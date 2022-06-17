using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
/// <summary>
/// 人物标签枚举
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
/// 角色父类控制移动，动画状态机，色块绑定
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
    /// 暂停界面
    /// </summary>
    private GameObject stop;

    /// <summary>
    /// 是否获胜
    /// </summary>
    [HideInInspector]
    public int win;

    /// <summary>
    /// 旧色块所属玩家
    /// </summary>
    protected PlayerFlag oldTileType;

    /// <summary>
    /// 玩家标签
    /// </summary>
    private PlayerFlag playerflag;
    public PlayerFlag playerFlag { get { return playerflag; } set { playerflag = value; } }

    /// <summary>
    /// 静态字典存储角色和对应的色块数组
    /// </summary>
    public static Dictionary<PlayerFlag, TileBase[][]> playTile;

    /// <summary>
    /// 交错数组储存角色色块
    /// </summary>
    public TileBase[][] Tiles;

    [HideInInspector]
    public int moveMode;

    [HideInInspector]
    public int scores;

    /// <summary>
    /// 玩家面向方向
    /// </summary>
    [HideInInspector]
    public Vector2 Forward;


    [Header("速度")]
    public int speed;

    [Header("晕眩时间")]
    public float dizzyTime;

    [Header("跑鞋镖持续时间")]
    public float boostTime;

    [Header("药持续时间")]
    public float yaoTime;

    [Header("炸弹填涂瓦片数量")]
    public int tileNumBomb;

    [Header("道具栏背景")]
    public Image bagBackGround;
    [Header("道具栏")]
    public List<Image> PropsBag;
    [Header("该道具栏是否存在道具")]
    public bool[] propExist;

    [Header("道具飞镖")]
    public GameObject biao;

    [HideInInspector][Header("被镖击中")]
    public bool AttackedByBiao;

    [HideInInspector][Header("攻击方的瓦片")]
    public TileBase[][] AttackerTile;

    [HideInInspector][Header("透明药水生效")]
    public bool PolluteByYao;

    [Header("无敌帧时间")]
    public float unstop;
    protected float _unstop;
    
    //无敌帧
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
    /// 移动并在玩家停止移动时 将玩家的正前方向量适应动画所朝方向
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
    ///使用道具1
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
    /// 使用道具2
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
    /// 暂停（未实装）
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
    /// 判断玩家所在瓦片
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
    /// 四方位判断
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
    /// 判断该瓦片是否属于自己
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
    /// 对周围8格内与旧瓦片类型以及新瓦片类型相同的瓦片进行重新填涂
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
    /// 加载瓦片保存在交错数组内
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
    /// 眩晕苏醒
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
    /// 移动模式
    /// </summary>
    /// <param name="m"></param>
    public void MoveByMoveMode(int m)
    {
        if(m==0) rigidBody.MovePosition(rigidBody.position + inputMove * speed * Time.fixedDeltaTime);
        else rigidBody.AddForce(inputMove * speed );
    }
    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="image">道具栏</param>
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
