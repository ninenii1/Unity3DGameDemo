using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在Create菜单中创建一个子集菜单，右键文件夹能创建Create-Character Stats-Data
[CreateAssetMenu(fileName = "New Data",menuName = "Character Stats/Data")] 

public class CharacterData_SO : ScriptableObject //MonoBehaviour改为ScriptableObject类型
{
    [Header("stats Info")]//【统计信息】
    //在Inspector中设置数值
    public int maxHealth;//最大血量
    public int currentHealth;//当前血量
    public int baseDefence;//基础防御 
    public int currentDefence;//当前防御

    [Header("Kill")]
    public int killPoint;//消灭一个敌人获得的经验值

    [Header("Level")]
    public int currentLevel;//当前等级
    public int maxLevel;//最高等级
    public int baseExp;//升级所需的经验值
    public int currentExp;//基础经验值
    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)//升级经验
    {
        currentExp += point;//增加经验值
        if(currentExp >= baseExp)//升级
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        //可以写所有想提升的数据
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//currentLevel的值在0 - maxLevel之间
        baseExp += (int)(baseExp * LevelMultiplier);//升级之后下一个阶段升级所需的经验值

        maxHealth = (int)(maxHealth * LevelMultiplier);//升级之后最大血量增加
        currentHealth = maxHealth;//当前血量 = 最大血量

        Debug.Log("LEVEL UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}
