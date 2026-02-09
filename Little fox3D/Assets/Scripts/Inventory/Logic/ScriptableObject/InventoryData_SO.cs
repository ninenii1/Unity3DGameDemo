using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject //背包数据资源
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO NewItemData, int amount)//在背包中添加物品和数量
    {
        bool found = false;
        if(NewItemData.stackable)//如果是可堆叠的物品
        {
            foreach (InventoryItem item in items) //遍历整个列表，找已存在的物品用foreach
            {
                if (item.itemData == NewItemData)//如果在列表中找到一样的物品，则累加数量即可
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }
        for(int i = 0; i < items.Count; i++)//找到最近的空格，填写物品数据
        {
            if (items[i].itemData == null && !found)//如果当前背包格为空且没有找到一样的物品
            {
                items[i].itemData = NewItemData;//将物品加到列表中
                items[i].amount = amount;
                break;
            }
        }
    }
}

[System.Serializable] //需要序列化，以下两个参数才能出现在Inspector窗口
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;//持有该物品的数量
}

