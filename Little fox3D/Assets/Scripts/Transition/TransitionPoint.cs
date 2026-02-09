using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType//判断是同场景传送还是异场景传送，创建枚举型变量方便在下拉菜单进行选择
    {
        SameScene,DifferentScene//同场景，不同场景
    }
    [Header("Transition Info")]//传送信息
    public string sceneName;//记录场景的名字，同场景则不用写
    public TransitionType transitionType;//选择是同场景传送还是异场景传送
    public TransitionDestination.DestinationTag destinationTag;//选择所要传送到的点

    private bool canTrans;//能否被传送

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canTrans)//按下E键且可以被传送时执行
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans = true;//是Player则可以被传送
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
