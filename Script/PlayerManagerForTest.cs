using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerManagerForTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
        for(int i=0;i<PlayerInputManager.instance.maxPlayerCount;i++)
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            PlayerInput.Instantiate(PlayerManager.Instance.joinPlayers[i], i, null, -1, PlayerManager.Instance.inputDevices[i]);
        }
    }
    void Update()
    {
        
    }


}
