using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Text levelText;
    Image healthSlider;
    Image expSlider;
    void Awake()
    {
        //获取组件
        levelText = transform.GetChild(2).GetComponent<Text>();//Level为第三个子物体，编号为2（从0开始）
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent <Image>();
    }

    void Update()
    {
        levelText.text = "Level " + GameManager.Instance.playerStats.CharacterData.currentLevel.ToString("00");//显示为01这种格式
        UpdateHealth();
        UpdateExp();
    }
    void UpdateHealth()//实时更新Player的血量
    {
        //得到血量的百分比
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;//赋值给滑动条
    }
    void UpdateExp()//实时更新Player的经验条
    {
        //得到血量的百分比
        float sliderPercent = (float)GameManager.Instance.playerStats.CharacterData.currentExp / GameManager.Instance.playerStats.CharacterData.baseExp;
        expSlider.fillAmount = sliderPercent;//赋值给滑动条
    }
}
