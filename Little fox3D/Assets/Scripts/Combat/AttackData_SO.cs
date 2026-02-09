using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;//基本攻击距离
    public float skillRange;//远程技能攻击距离
    public float coolDown;//CD冷却时间
    public int minDamage;//最小攻击数值
    public int maxDamage;//最大攻击数值
    //criticalMultiplier为2，criticalChance为0.2，代表百分之二十的机率暴击，暴击后的伤害*2
    public float criticalMultiplier;//暴击后的加成伤害倍数
    public float criticalChance;//暴击率（1代表百分百暴击）

    public void ApplyWeaponData(AttackData_SO weapon) //拿到武器后替换掉Player原有的攻击属性，使攻击力升级
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;

        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalMultiplier = weapon.criticalMultiplier;
        criticalChance = weapon.criticalChance;
    }
}
