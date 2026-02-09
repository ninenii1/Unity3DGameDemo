using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    PlayableDirector director;
    void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        //将方法添加到点击事件中
        newGameBtn.onClick.AddListener(PlayTimeline);//点击NewGame按钮播放动画
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    
        director = FindObjectOfType<PlayableDirector>();//用寻找类的方式获取
        director.stopped += NewGame;//使用自带的Action，播放结束后执行NewGame()
    }

    void PlayTimeline()
    {
        director.Play();
    }
    void NewGame(PlayableDirector obj)//使用Action需要PlayableDirector类型的参数
    {
        PlayerPrefs.DeleteAll();//清除之前的游戏记录
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        //转换场景，读取进度
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出游戏");
    }
}
