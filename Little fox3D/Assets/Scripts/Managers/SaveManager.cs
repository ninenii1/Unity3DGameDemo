using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>//切换场景后能继续保留数据（泛型单例模式）
{
    string sceneName = "";//存储当前场景名字

    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    //使用PlayerPrefs配合Json进行数据保存
    //PlayerPrefs是unity自带的保存数据的方法，在实际硬盘中产生文件数据。它只能保存三种数据类型（Float，int，string）
    //string：名字、描述等，int和float：数值，坐标可以拆解为多个float或int
    //这里使用PlayerPrefs中的Save方法，将所有修改的偏好写入磁盘

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//按Esc键返回主菜单界面
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.CharacterData, GameManager.Instance.playerStats.CharacterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.CharacterData, GameManager.Instance.playerStats.CharacterData.name);
    }

    public void Save(object data, string key)//存储数据
    {
        //JsonUtility.ToJson：将类中所有变量变成string类型的字符串存储在系统中
        var jsonData = JsonUtility.ToJson(data, true);//true：使字符串的格式更漂亮
        //SetString(string key, string value)【关键词，匹配的数值】
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);//保存当前场景名称
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)//加载数据
    {
        if(PlayerPrefs.HasKey(key))//HasKey：key存在则返回true
        {
            //FromJsonOverwrite读取数据
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);//GetString：返回key对应的值
        }
    }
}    

