using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PigMapManager : MapManager
{
    public GameObject pig;
    [HeaderAttribute("猪群冲锋间隔")]
    public float rushTime=5.5f;
    [Header("猪数量")]
    public int num;

    // Start is called before the first frame update
    void Start()
    {
        Coroutine c = StartCoroutine(CreatePig(false));
        InvokeRepeating("ShowProps", 1.5f, propTime);
    }

    IEnumerator CreatePig(bool IsRight)
    {
        yield return new WaitForSeconds(5f);
        while(true)
        {   
            for(int i=0;i<num; i++)
            {
                Instantiate(pig).GetComponent<PigRush>().isRight = IsRight; 
                IsRight = !IsRight;
                yield return new WaitForSeconds(0.5f);
            }         
            yield return new WaitForSeconds(rushTime);
        }

    }
  
}
