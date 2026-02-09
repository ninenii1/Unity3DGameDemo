using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Useable Item", menuName = "Inventory/Useable Item Data")]
public class UseableItemData_SO : ScriptableObject
{
    //物品功能（血量、攻击力、防御力等变化参数）
    public int healthPoint;//回血
}
