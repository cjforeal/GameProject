using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Biao : MonoBehaviour
{
    public int speed;
    public BasicPlayer biaoParent;
    private PlayerFlag oldTileType;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&& collision.GetComponent<BasicPlayer>()!=biaoParent)
        {
            collision.GetComponent<BasicPlayer>().StartCoroutine("Biao", biaoParent.Tiles);
            Destroy(gameObject);
        }
    }
    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime*transform.right,Space.World);///translate
        biaoParent.JudegNowPos(biaoParent.Tiles,transform.position,ref oldTileType);
        biaoParent.TestFloor(biaoParent.Tiles, transform.position);
        biaoParent.TestAroundFloor(biaoParent.Tiles, transform.position, oldTileType);
        if (transform.position.x > 10f || transform.position.x < -10f|| transform.position.y>5|| transform.position.y<-4.5f)
        {
            Destroy(gameObject);
        }
    }
}
