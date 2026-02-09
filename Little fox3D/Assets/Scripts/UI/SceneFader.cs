using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneFader : MonoBehaviour
{
    CanvasGroup CanvasGroup;
    public float fadeInDuration;//控制渐入时间
    public float fadeOutDuration;//控制渐出时间
    void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        
        DontDestroyOnLoad(gameObject);//不要在转换场景时销毁
    }

    //伴随其他事件同步运行（在游戏运行过程中逐渐从0变1或从1变0）则使用协程方法

    public IEnumerator FadeOutIn()//用协程调用另一个协程
    {
        //先变白屏再变为没有（Alpha变1再变0）
        yield return FadeOut(fadeOutDuration);//直接返回另一个协程
        yield return FadeIn(fadeInDuration);
    }
    public IEnumerator FadeOut(float time) //在SceneController中执行，因此要public
    {
        while(CanvasGroup.alpha < 1)//Alpha从0变1（变为白屏）
        {
            CanvasGroup.alpha += Time.deltaTime / time;//随着时间变化逐渐将Alpha值加到1
            yield return null;
        }
    }

    public IEnumerator FadeIn(float time)
    {
        while (CanvasGroup.alpha != 0)
        {
            CanvasGroup.alpha -= Time.deltaTime / time;//逐渐从1到指定时间变为0
            yield return null;
        }
        Destroy(gameObject);//切换场景之后被销毁
    }
}
