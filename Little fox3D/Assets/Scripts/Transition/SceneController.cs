using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
public class SceneController : Singleton<SceneController>, IEndGameObserver//添加接口
{
    public GameObject PlayerPrefab;//获取player的预制体
    public SceneFader sceneFaderPrefab;//在脚本组件中添加sceneFader预制体
    bool fadeFinished;//判断游戏是否已经结束一次了

    GameObject player;
    NavMeshAgent playerAgent;

    protected override void Awake() //保证跳转场景后，这个脚本仍然存在，才能继续执行生成Player
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);//注册到观察者列表中
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)//传送到目标点
    {
        switch(transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene://同场景传送
                //StartCoroutine启用协程，GetActiveScene异步加载场景
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene://异场景传送
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    //使用协程逐步加载0-90%，最后10%是启用场景
    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();//切换场景前保存数据
        InventoryManager.Instance.SaveData();

        if (SceneManager.GetActiveScene().name != sceneName) //如果传入的场景名称与所处场景不相同，则为跨场景传送
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//加载场景
            //生成Player
            yield return Instantiate(PlayerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();//读取原有数据（不同场景下）
            yield break;//跳出协程
        }
        else //否则为同场景传送
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;//传送前要关闭NavMeshAgent，否则无法传送
                                        //SetPositionAndRotation(蓝色点的位置，蓝色点的旋转方向)
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;//传送后再开启
            yield return null;
        }
        
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        //DestinationTag值为A，B，C
        //寻找到对应的传送点并返回
        var entrances = FindObjectsOfType<TransitionDestination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if(entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }

        return null;
    }
    
    public void TransitionToMain()//加载主菜单场景
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));//读取到场景名称
    }

    public void TransitionToFirstLevel()//加载第一个场景
    {
        StartCoroutine(LoadLevel("NewGame"));//开启协程
    }
    IEnumerator LoadLevel(string scenes)//创建协程
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);//在加载场景时生成sceneFader，默认Alpha为0
        if (scenes !="")
        {
            yield return StartCoroutine(fade.FadeOut(2f));//从0变1（变白屏）
            yield return SceneManager.LoadSceneAsync(scenes);//加载场景
            //生成Player
            yield return player = Instantiate(PlayerPrefab,GameManager.Instance.GetEntrance().position,GameManager.Instance.GetEntrance().rotation);

            //保存数据
            SaveManager.Instance.SavePlayerData();
            InventoryManager.Instance.SaveData();
            yield return StartCoroutine(fade.FadeIn(2f));//加载完Player和场景之后，从1变0
            yield break;//结束协程
        }
    }

    IEnumerator LoadMain()//加载回到主菜单场景
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()//Player死亡后
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}
