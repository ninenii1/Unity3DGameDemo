using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmout);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            //装备武器
            //GameManager.Instance.playerStats.EquipWeapon(itemData);
            //销毁
            Destroy(gameObject);
        }
    }
}
