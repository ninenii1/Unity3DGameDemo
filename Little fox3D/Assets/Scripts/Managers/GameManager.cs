using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//由GameManager脚本来控制整个游戏的进度，player死亡代表整个游戏结束
public class GameManager : Singleton<GameManager>
{
    //使用public的原因：希望其他代码想要访问playerStats时，都通过GameManager来访问
    public CharacterStats playerStats;//获得player的状态

    private CinemachineFreeLook followCamera;

    //列表类型为接口的类型，将观察者加入到列表中
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    protected override void Awake() //保证跳转场景后，这个脚本仍然存在
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;//注册Player

        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if(followCamera != null )//为摄像机的组件获取Player的子物体，实现跟随Player移动
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }
    //敌人生成时加入到列表
    public void AddObserver(IEndGameObserver observer)
    {
        //只有每个敌人在启用时，才注册这个命令，所以不会重复添加
        endGameObservers.Add(observer);
    }

    //敌人死亡销毁后要从列表中移除
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //实现广播
    public void NotifyObservers()
    {
        foreach(var observer in endGameObservers)//循环遍历每个观察者，告诉他们要执行EndNotify函数
        {
            observer.EndNotify();
        }
    }
    
    public Transform GetEntrance()//找到跳转场景的入口
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())//遍历所有目标点
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform;
        }
        return null;
    }

}
