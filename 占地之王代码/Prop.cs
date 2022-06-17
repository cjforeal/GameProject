using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prop : MonoBehaviour
{
    public Image image;
    private MapManager MapManager;
    private void OnEnable()
    {
        MapManager = GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            BasicPlayer basicPlayer = collision.GetComponent<BasicPlayer>();
            for(int i=0;i<basicPlayer.PropsBag.Capacity;i++)
            {
                if(!basicPlayer.propExist[i])
                {   
                    int j=(int)(basicPlayer.playerFlag);
                    basicPlayer.propExist[i] = true;
                    Instantiate(image, basicPlayer.PropsBag[i].transform);
                    if (!Bkmuisic.Instance.audioSources[j].isPlaying)
                    {
                        Bkmuisic.Instance.audioSources[j].PlayOneShot(Bkmuisic.Instance.playersAuido[0]);
                    }                  
                    MapManager.propNow--;
                    Destroy(gameObject);
                    break;
                }               
            }
        }
    }
}
