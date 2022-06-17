using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IcyMapManager : MapManager
{ 
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShowProps", 1.5f, propTime);
    }

    // Update is called once per frame

   
}
