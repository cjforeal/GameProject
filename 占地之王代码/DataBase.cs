using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DataBase : MonoBehaviour
{
    private int ining;
    private MySqlConnection connection;
    private MySqlCommand command;
    private MySqlDataReader reader;
    private string Server;
    private string user;
    private static DataBase instance;
    public static DataBase Instance { get { return instance; } }
    public int Ining { get { return ining; } set { ining = value; } }
    public List<string> winner;
    /// 登录
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    //public bool Click_login(string username,string password)
    //{
    //    MySqlConnection conn = new MySqlConnection(Server);
    //    try
    //    {
    //        conn.Open();
    //        Debug.Log("------链接成功------");

    //        string sqlQuary = "select * from loginsystem where user_name =@paral1 and user_password = @paral2";
    //        MySqlCommand comd = new MySqlCommand(sqlQuary, conn);
    //        comd.Parameters.AddWithValue("paral1", username);
    //        comd.Parameters.AddWithValue("paral2", password);

    //        MySqlDataReader reader = comd.ExecuteReader();
    //        if (reader.Read())
    //        {
    //            Debug.Log("------用户存在，登录成功！------");
    //            return true;

    //        }
    //        else
    //        {
    //            Debug.Log("------用户不存在，请注册。或请检查用户名或和密码！------");
    //            return false;
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.Log(e.Message);
    //        return false;
    //    }
    //    finally
    //    {
    //        conn.Close();
    //    }
    //}
    ///// <summary>
    ///// 注册
    ///// </summary>
    ///// <param name="username"></param>
    ///// <param name="password"></param>
    //public void Click_register(string username, string password)
    //{
    //    MySqlConnection conn = new MySqlConnection(Server);
    //    try
    //    {
    //        conn.Open();
    //        Debug.Log("-----连接成功！------");

    //        string sqlQuary = "select * from loginsystem where user_name =@paral1 and user_password = @paral2";
    //        MySqlCommand comd = new MySqlCommand(sqlQuary, conn);
    //        comd.Parameters.AddWithValue("paral1", username);
    //        comd.Parameters.AddWithValue("paral2", password);

    //        MySqlDataReader reader = comd.ExecuteReader();
    //        if (reader.Read())
    //        {
    //            Debug.Log("用户名已存在");
    //        }
    //        else
    //        {
    //            Insert_User( username,  password);
    //            Debug.Log("注册成功");
    //        }
    //    }
    //    catch (System.Exception e)
    //    {

    //        Debug.Log(e.Message);
    //    }
    //    finally
    //    {   
    //        conn.Close();
    //    }
    //}
    ////插入用户
    //private void Insert_User(string username, string password)
    //{
    //    MySqlConnection conn = new MySqlConnection(Server);

    //    try
    //    {
    //        conn.Open();
    //        string sqlInsert = "insert into loginsystem(user_name,user_password) values('" + username + "','" + password + "')";
    //        MySqlCommand comd2 = new MySqlCommand(sqlInsert, conn);
    //        int resule = comd2.ExecuteNonQuery();
    //    }
    //    catch (System.Exception e)
    //    {

    //        Debug.Log(e.Message);
    //    }
    //    finally
    //    {
    //        conn.Close();
    //    }
    //}

    void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        Server = "server = localhost;port = 3306;database =finalgame_database;user = grade2;password =123;";
        user = "server = localhost;port = 3306;user = grade2;password = 123;";
        DatabaseExist();
    }
    public void DatabaseExist()
    {
        connection = new MySqlConnection(user);
        connection.Open();
        string sqlDB = "SELECT * FROM information_schema.SCHEMATA where SCHEMA_NAME='finalgame_database';";

        MySqlDataAdapter adp = new MySqlDataAdapter(sqlDB, connection);

        DataSet ds = new DataSet();

        adp.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
            Debug.Log("数据库已存在");
        else
        {
            Debug.Log("数据库不存在!");
            command = new MySqlCommand(string.Format("CREATE DATABASE finalgame_database;"),connection);
            command.ExecuteNonQuery();
            connection.Close();

            connection = new MySqlConnection(Server);
            connection.Open();
            string sqlcreatetable = string.Format("CREATE TABLE IF NOT EXISTS bird(char_name varchar(11) NOT NULL,ining int(11) not NULL,score int(11) NOT NULL,iswin tinyint(1) NOT NULL)",connection);
            command = new MySqlCommand(sqlcreatetable, connection);
            command.ExecuteNonQuery();

            string sqlcreatetable1 = string.Format("CREATE TABLE IF NOT EXISTS ghost(char_name varchar(11) NOT NULL,ining int(11) not NULL,score int(11) NOT NULL,iswin tinyint(1) NOT NULL)", connection);
            command = new MySqlCommand(sqlcreatetable1, connection);
            command.ExecuteNonQuery();

            string sqlcreatetable2 = string.Format("CREATE TABLE IF NOT EXISTS greenshit(char_name varchar(11) NOT NULL,ining int(11) not NULL,score int(11) NOT NULL,iswin tinyint(1) NOT NULL)", connection);
            command = new MySqlCommand(sqlcreatetable2, connection);
            command.ExecuteNonQuery();

            string sqlcreatetable3 = string.Format("CREATE TABLE IF NOT EXISTS pinkshit(char_name varchar(11) NOT NULL,ining int(11) not NULL,score int(11) NOT NULL,iswin tinyint(1) NOT NULL)", connection);
            command = new MySqlCommand(sqlcreatetable3, connection);
            command.ExecuteNonQuery();

            string sqlcreatetable4 = string.Format("CREATE TABLE IF NOT EXISTS ranking(char_name varchar(10) NOT NULL,win_ining int(11) not NULL,highestscore int(11) NOT NULL,total_score int(11) NOT NULL,PRIMARY KEY (char_name))", connection);
            command = new MySqlCommand(sqlcreatetable4, connection);
            command.ExecuteNonQuery();
            
            string sqlinsert1 = $"insert into ranking(char_name,win_ining,highestscore,total_score) values('GreenShit',0,0,0)";
            command = new MySqlCommand(sqlinsert1, connection);
            command.ExecuteNonQuery();

            string sqlinsert2 = $"insert into ranking(char_name,win_ining,highestscore,total_score) values('PinkShit',0,0,0)";
            command = new MySqlCommand(sqlinsert2, connection);
            command.ExecuteNonQuery();

            string sqlinsert3 = $"insert into ranking(char_name,win_ining,highestscore,total_score) values('Bird',0,0,0)";
            command = new MySqlCommand(sqlinsert3, connection);
            command.ExecuteNonQuery();

            string sqlinsert4 = $"insert into ranking(char_name,win_ining,highestscore,total_score) values('Ghost',0,0,0)";
            command = new MySqlCommand(sqlinsert4, connection);
            command.ExecuteNonQuery();
        }

        connection.Close();


    }
    /// <summary>
    /// 一局游戏结束后保存数据并更新排行榜
    /// </summary>
    /// <param name="tablename"></param>
    /// <param name="charname"></param>
    /// <param name="ining"></param>
    /// <param name="score"></param>
    /// <param name="iswin"></param>
    public void SaveData(string tablename ,string charname,int ining,int score,int iswin)
    {
        connection = new MySqlConnection(Server);
        try
        {
            connection.Open();
            //Debug.Log("连接成功");
            //插入数据
            string sqlQuary = $"insert into {tablename}(char_name,ining,score,iswin) values('{charname}',{ining},{score},{iswin})";
             command = new MySqlCommand(sqlQuary, connection);
            int result = command.ExecuteNonQuery();
            if (result>0)
            {
                //Debug.Log("插入成功");  
            }
            else
            {
                //Debug.Log("插入失败");
            }
            //更新总分
            string sqlQuary1 = $"update ranking set total_score =total_score+{score} where char_name ='{charname}'";
            command = new MySqlCommand(sqlQuary1, connection);
            if (command.ExecuteNonQuery() > 0)
            {
                //Debug.Log("更新总分成功");
            }
            else
            {
                //Debug.Log("更新总分失败");
            }

            //更新总获胜局次
            if (iswin>0)
            {
                string sqlQuary2 = $"update ranking set win_ining = win_ining+1 where char_name ='{charname}'";
                command = new MySqlCommand(sqlQuary2, connection);
                if (command.ExecuteNonQuery()>0)
                {
                    //Debug.Log("更新局次成功");
                }
                else
                {
                    //Debug.Log("更新局次失败");
                }
            }
            //更新最高分
            string sqlQuary3 = $"select max(score) from {tablename}";
            command = new MySqlCommand(sqlQuary3, connection);
             reader = command.ExecuteReader();
            if(reader.Read())
            {
                int highestScore = (int)reader[0];
                reader.Close();
                string sqlQuary4 = $"select highestscore from ranking where char_name ='{charname}'";
                command = new MySqlCommand(sqlQuary4, connection);
                 reader = command.ExecuteReader();
                if(reader.Read())
                {
                    int highestScoreRanking = (int)reader[0];
                    reader.Close();
                    if (highestScore > highestScoreRanking)
                    {
                        string sqlQuary5 = $"update ranking set highestscore = {highestScore} where char_name ='{charname}'";
                         command = new MySqlCommand(sqlQuary5, connection);
                        if (command.ExecuteNonQuery() > 0)
                        {
                            //Debug.Log("更新最高分成功");
                        }
                        else
                        {
                            //Debug.Log("更新最高分失败");
                        }
                    }
                }              
            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    /// <summary>
    /// 重排列排行榜，第一权重为获胜局数，第二权重为最高分，第三权重为总分
    /// </summary>
    public void ShowRanking(List<Text> Texts)
    {
         connection = new MySqlConnection(Server);
        try
        {
            connection.Open();
            Debug.Log("-----连接成功！------");

            string sqlQuary = $"select * from ranking order by win_ining desc,highestscore desc,total_score desc";
             command = new MySqlCommand(sqlQuary, connection);
             reader = command.ExecuteReader();
            foreach (Text t in Texts)
            {
                reader.Read();
                t.text = $"{reader[0]}   {reader[1]}   {reader[2]}   {reader[3]}";
            }
        }
        catch (System.Exception e)
        {

            Debug.Log(e.Message);
        }
        finally
        {
            connection.Close();
        }
    }
}
