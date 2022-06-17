using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CandyMapManager :  MapManager
{
    public GameObject candy;
    [HeaderAttribute("糖果掉落间隔")]
    public float fallingTime;
    [HeaderAttribute("糖果数量")]
    public int candyNum;
    //一排产生
    private float regularRow;
    //一列产生
    private float regularColumn;
    private bool isright;
    private bool isabove;
    // Update is called once per frame
    void Start()
    {
        InvokeRepeating("ShowProps", 1.5f, propTime);
        if(CrazyGameOn)
        {
            Coroutine rowFromRight = StartCoroutine(CreateRegularRowCandy(3.5f,true));
            Coroutine rowFromLeft = StartCoroutine(CreateRegularRowCandy(-4f, false));
            Coroutine columnFromUp = StartCoroutine(CreateRegularColumnCandy(7.5f,true));
            Coroutine columnFromBottom = StartCoroutine(CreateRegularColumnCandy(-7.5f, false));
        }
        else
        {
            InvokeRepeating("CreateFallingCandy", 1.5f, fallingTime);
        }
    }
    
    private void CreateFallingCandy()
    {
        int i = 0;
        while(i<candyNum)
        {
            x = Random.Range(-8, 9);
            y = Random.Range(-4, 4);
            x =x > 0 ? x- 0.5f : x+ 0.5f;
            y = y > 0 ? y - 0.5f : y + 0.5f;
            if (!tilemap.GetTile(tilemap.WorldToCell(new Vector3(x,y, 0)))&&!obstacleTilemap.GetTile(obstacleTilemap.WorldToCell(new Vector3(x, y, 0))))
            {
                Destroy(Instantiate(candy, new Vector3(x, y, 0), Quaternion.identity),1.8f);
                i++;
            }
        }
    }
    /// <summary>
    /// 生成横排糖果
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateRegularRowCandy(float regularRow,bool isabove)
    {   
        while(true)
        {
            if (isabove)
            {
                for (float i = -7.5f; i < 8; i+=1f)
                {
                    Vector3Int temp = tilemap.WorldToCell(new Vector3(i, regularRow, 0));
                    if (!tilemap.GetTile(temp) && !obstacleTilemap.GetTile(temp))
                        Destroy(Instantiate(candy, new Vector3(i, regularRow, 0), Quaternion.identity), 1.8f);
                }
                if (--regularRow < -4) isabove = false;
            }
            else
            {
                for (float i = -7.5f; i < 8; i += 1f)
                {
                    Vector3Int temp = tilemap.WorldToCell(new Vector3(i, regularRow, 0));
                    if (!tilemap.GetTile(temp) && !obstacleTilemap.GetTile(temp))
                        Destroy(Instantiate(candy, new Vector3(i, regularRow, 0), Quaternion.identity), 1.8f);
                }
                if (++regularRow > 3.5f) isabove = true;
            }
            yield return new WaitForSeconds(1f);
        }
       
    }

    /// <summary>
    /// 生成竖排糖果
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateRegularColumnCandy(float regularColumn,bool isright )
    {   
        while(true)
        {
            if (isright)
            {
                for (float i = -3.5f; i < 4f; i+=1f)
                {
                    Vector3Int temp = tilemap.WorldToCell(new Vector3(regularColumn, i, 0));
                    if (!tilemap.GetTile(temp) && !obstacleTilemap.GetTile(temp))
                        Destroy(Instantiate(candy, new Vector3(regularColumn, i, 0), Quaternion.identity), 1.8f);
                }
                if (--regularColumn < -7.5) isright = false;
            }
            else
            {
                for (float i = -3.5f; i < 4f; i += 1f)
                {   
                    Vector3Int temp = tilemap.WorldToCell(new Vector3(regularColumn, i, 0));
                    if (!tilemap.GetTile(temp) && !obstacleTilemap.GetTile(temp))
                        Destroy(Instantiate(candy, new Vector3(regularColumn, i, 0), Quaternion.identity), 1.8f);
                }
                if (++regularColumn > 7.5) isright = true;
            }
            yield return new WaitForSeconds(1f);
        }
        
    }

}
