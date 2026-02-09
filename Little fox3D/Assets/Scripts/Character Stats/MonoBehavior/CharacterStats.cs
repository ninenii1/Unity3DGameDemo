using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;//模板数据
    public CharacterData_SO CharacterData;
    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;
    private RuntimeAnimatorController baseAnimator;
    [Header("Weapon")]
    public Transform weaponSlot;//生成武器的父级

    //在其他代码中可以访问，但不希望在Inspector对isCritical进行赋值
    [HideInInspector]//将isCritical在Inspector窗口中隐藏
    public bool isCritical;//是否暴击了

    void Awake()//最开始运行的部分
    {
        if(templateData !=null)//当前模板数据不为空，代表要调用这个模板数据
            CharacterData = Instantiate(templateData);//将模板数据生成副本来使用

        baseAttackData = Instantiate(attackData);//生成基础的攻击力
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;//获取原有的动画
    }

    //region用于管理代码，可以将代码折叠起来
    #region Read from Data_SO

    //作用：用MaxHealth可以直接对CharacterData里面的数值进行修改和读取
    public int MaxHealth
    {
        //get读取，必须有返回值
        get { if (CharacterData != null) return CharacterData.maxHealth; else return 0; }//CharacterData中没有该数值模板则返回0
        //使用CharacterData.MaxHealth = 2进行赋值来直接更改CharacterData.MaxHealth
        set { CharacterData.maxHealth = value; }//value为输入进来的值
    }
    public int CurrentHealth
    {
        get { if (CharacterData != null) return CharacterData.currentHealth; else return 0; }
        set { CharacterData.currentHealth = value; }
    }
    public int BaseDefence
    {
        get { if (CharacterData != null) return CharacterData.baseDefence; else return 0; }
        set { CharacterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (CharacterData != null) return CharacterData.currentDefence; else return 0; }
        set { CharacterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)//计算伤害，defener为受伤的对象
    {
        //Mathf.Max()的作用：防止攻击力<防御值时出现负数，使得被攻击方加血，所以设置最小伤害为0
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence , 0);//伤害=攻击力-防御力
        //血量
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        
        if(attacker.isCritical)//如果暴击了，播放敌人/玩家的暴击动画
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth); //当前血量，最大血量【更新血条】
        //经验update
        if (CurrentHealth <= 0)
            attacker.CharacterData.UpdateExp(CharacterData.killPoint);//死亡后将自己的killPiont加到攻击者的经验值中
    }

    public void TakeDamage(int damage, CharacterStats defener)//函数重载
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);//CurrentDefence为防御力，伤害最低为0
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);//保证血量不会小于0
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);//更新血条
        //石头人死亡后将自己的killPiont加到攻击者的经验值中
        GameManager.Instance.playerStats.CharacterData.UpdateExp(CharacterData.killPoint);
    }
    private int CurrentDamage()
    {
        //随机伤害（在最大伤害和最小伤害之间随机选取一个值）
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage,attackData.maxDamage);
        if(isCritical)//在暴击的情况下
        {
            coreDamage *= attackData.criticalMultiplier;//伤害乘以暴击的倍数
        }
        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();//卸下武器
        EquipWeapon(weapon);//装上新武器
        //InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }
    public void EquipWeapon(ItemData_SO weapon) //装备武器,升级攻击力
    {
        if(weapon.weaponPrefab != null)
        {
            Instantiate(weapon.weaponPrefab, weaponSlot);//传入预制体和父级，会保持原有的Position和Rotation生成在父级中
        }
        //更新属性
        attackData.ApplyWeaponData(weapon.weaponData);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weqponAnimator;//切换为武器动画（runtimeAnimatorController实时的动画控制器）
    }

    public void UnEquipWeapon()//卸下武器，攻击力还原
    {
        if(weaponSlot.transform.childCount != 0)
        {
            for(int i = 0; i< weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);//销毁手上的武器
            }
        }
        attackData.ApplyWeaponData(baseAttackData);
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;//切换回原始动画（没有武器时的动画）
    }
    #endregion

    #region Apply Data Change //使用物品后改变人物身上的数据（血量、攻击力、防御力等）
    public void ApplyHealth(int amount)//回血，传入回血量
    {
        if (CurrentHealth + amount <= MaxHealth)//相加不超过允许的最大血量
            CurrentHealth += amount;
        else
            CurrentHealth = MaxHealth; //超过允许的最大血量，则当前血量直接等于最大血量
        //还可以添加人物在满血状态下不能使用的方法
    }
    #endregion
}
