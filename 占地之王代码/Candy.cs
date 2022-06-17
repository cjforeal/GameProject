using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{   
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BasicPlayer b = other.GetComponent<BasicPlayer>();
            if (!b.UnStopable)
            {
                if(!Bkmuisic.Instance.audioSources[(int)(b.playerFlag)].isPlaying)
                {
                    Bkmuisic.Instance.audioSources[(int)(b.playerFlag)].PlayOneShot(Bkmuisic.Instance.playersAuido[6]);
                }
                Animator anim = other.GetComponent<Animator>();
                anim.SetBool("Stuck", true);
                b.UnStopable = true;
                Destroy(gameObject);
            }
        }
    }
    /// <summary>
    /// ¶¯»­Ö¡ÊÂ¼þ
    /// </summary>
    public void BeginStuck() => GetComponent<BoxCollider2D>().enabled = true;

}
