using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Useable, Weapon, Armor }//Armor装备栏
[CreateAssetMenu(fileName ="New Item", menuName ="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;//物品类型
    public string itemName;//物品名字
    public Sprite itemIcon;//图标
    public int itemAmout;//堆叠数量

    [TextArea]//变成可以输入更多字数的文本框区域
    public string description = "";//物品详情

    public bool stackable;//物品是否可以堆叠

    [Header("Useable Item")]
    public UseableItemData_SO useableData;
    
    [Header("Weapon")]
    public GameObject weaponPrefab;//武器要使用另外的预制体，去掉碰撞体和刚体，防止与Player碰撞
    public AttackData_SO weaponData;
    public AnimatorOverrideController weqponAnimator;//有武器时的动画
}
