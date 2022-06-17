//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class LoginSystem : MonoBehaviour
//{
//    public bool login_success;
//    public bool register_success;
//    public InputField USERNAME;
//    public InputField PASSWORD;
//    private string username;
//    private string password;
//    public GameObject start;
//    // Start is called before the first frame update
//    void Start()
//    {
//        username = USERNAME.text;
//        password = PASSWORD.text;
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    public void LoginIn()
//    {
//        if(DataBase.Instance.Click_login(username, password))
//        {
//            start.SetActive(true);
//            Destroy(gameObject);
//        }

//    }
//    public void Register()
//    {
//        DataBase.Instance.Click_register(username, password);
//    }
//}
