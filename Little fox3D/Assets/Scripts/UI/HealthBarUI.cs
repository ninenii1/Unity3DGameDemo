using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;//敌人头顶的空物体位置
    public bool alwaysVisible;//是否场景可见，勾选则会一开始就显示

    public float visibleTime;//可视化时间
    private float timeLeft;//剩余可显示时间

    Image healthSlider;//血量滑动条
    Transform UIbar;//血条的坐标参数
    Transform cam;//摄像机位置

    CharacterStats currentStats;

    void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    void OnEnable()//生成时调用一次
    {
        cam = Camera.main.transform;
        foreach(Canvas canvas in FindObjectsOfType<Canvas>())//遍历场景中所有Canvas
        {
            //如果Canvas是WorldSpace渲染模式则执行（只适用只有一个是WorldSpace渲染模式的情况）
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;//得到血条的坐标参数
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();//获取第一个子物体（Current Health）的Image组件
                UIbar.gameObject.SetActive(alwaysVisible);//血条是否可见
            }
        }
    }
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if(currentHealth <= 0)//当前血量<=0
        {
            Destroy(UIbar.gameObject);
            return;
        }
        UIbar.gameObject.SetActive(true);//受到攻击时，强行设置为可见
        timeLeft = visibleTime;//时间重置

        float sliderPercent = (float)currentHealth / maxHealth;//数值改为百分比
        healthSlider.fillAmount = sliderPercent;
    }

    // Update is called once per frame
    void LateUpdate()//上一帧渲染后再执行，敌人移动后血条再跟上，确保没有闪烁的效果
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -cam.forward;//血条永远面对摄像机

            if(timeLeft <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;
        }
    }
}
