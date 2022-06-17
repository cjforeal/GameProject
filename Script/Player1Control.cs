using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Player1Control : BasicPlayer
{
    public TileBase[] Temps;
    private void Awake()
    {
        animPlayer = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.position = new Vector3(-2.540000021f, 1.560000002f, 0);
        tilemap = GameObject.FindGameObjectWithTag("Ground").GetComponent<Tilemap>();
        //Tiles = new TileBase[7][];
        //Tiles[0] = new TileBase[1];
        //Tiles[1] = new TileBase[4];
        //Tiles[2] = new TileBase[2];
        //Tiles[3] = new TileBase[4];
        //Tiles[4] = new TileBase[1];
        //Tiles[5] = new TileBase[4];
        //Tiles[6] = new TileBase[4];
        //for (int i = 0; i < 7; i++)
        //{
        //    int j = 0;
        //    if (Tiles[i].Length == 4)
        //    {
        //        Tiles[i][0] = Temps[j++];
        //        Tiles[i][1] = Temps[j++];
        //        Tiles[i][2] = Temps[j++];
        //        Tiles[i][3] = Temps[j++];
        //    }
        //}
        //LoadTile("none", "Green/NO else/Green.asset", 0, 0);
        //LoadTile("twoSameUD", "Green/Two/SameSide/Green_UpDown.asset", 2, 0);
        //LoadTile("twoSameRL", "Green/Two/SameSide/Green_LeftRight.asset", 2, 1);
        //LoadTile("four", "Green/Four/Green.asset", 4, 0);
        //playerFlag = PlayerFlag.Player1;
        //playTile.Add(playerFlag, Tiles);
        //PlayerManager.instance.joinPlayers[(int)playerFlag] = this.gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {   
        Tiles = new TileBase[7][];
        Tiles[0] = new TileBase[1];
        Tiles[1] = new TileBase[4];
        Tiles[2] = new TileBase[2];
        Tiles[3] = new TileBase[4];
        Tiles[4] = new TileBase[1];
        Tiles[5] = new TileBase[4];
        Tiles[6] = new TileBase[4];
        for (int i = 0, j=0; i < 7; i++)
        {
            if (Tiles[i].Length == 4)
            {
                Tiles[i][0] = Temps[j++];
                Tiles[i][1] = Temps[j++];
                Tiles[i][2] = Temps[j++];
                Tiles[i][3] = Temps[j++];
            }
        }
        LoadTile("Green/NO else/Green", 0, 0);
        LoadTile("Green/Two/SameSide/Green_UpDown", 2, 0);
        LoadTile( "Green/Two/SameSide/Green_LeftRight", 2, 1);
        LoadTile("Green/Four/Green", 4, 0);
        playerFlag = PlayerFlag.Player1;
        playTile.Add(playerFlag, Tiles);
        PlayerManager.Instance.playingPlayers[(int)playerFlag-1] = this.gameObject;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsTag("Dizzy"))
            WakeUp();      
        else if(PolluteByYao)
        {
            MoveByMoveMode(moveMode);          
        }
        else if (!AttackedByBiao)
        {
            MoveByMoveMode(moveMode);
            JudegNowPos(Tiles, transform.position, ref oldTileType);
            TestFloor(Tiles, transform.position);
            TestAroundFloor(Tiles, transform.position, oldTileType);
            //Debug.Log(rigidBody.velocity);
        }
        else
        {
            MoveByMoveMode(moveMode);
            JudegNowPos(AttackerTile, transform.position, ref oldTileType);
            TestFloor(AttackerTile,transform.position);
            TestAroundFloor(AttackerTile, transform.position,oldTileType);
        }
    }

}
