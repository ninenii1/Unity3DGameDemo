using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//控制当前的单元格生成物品的基本信息
public enum SlotType { BAG, WEAPON, ARMOR, ACTION }//背包、武器栏、装备栏、快捷栏
public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public SlotType slotType;//背包类型
    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount % 2 == 0)//鼠标双击物品则执行
        {
            UseItem();
        }
    }

    public void UseItem()//使用物品
    {
        if(itemUI.GetItem() != null)//当前物品栏不为空才执行
        {
            //物品是Useable类型的物品且数量大于0才执行
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount > 0)
            {
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);//回血
                itemUI.Bag.items[itemUI.Index].amount -= 1;//使用物品后数量减一
            }
            UpdataItem();//更新物品图片显示
        }
        
    }

    public void UpdataItem()
    {
        switch(slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;//显示普通背包的数据
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                //装备武器 切换武器
                if (itemUI.Bag.items[itemUI.Index].itemData != null)
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)//鼠标在格子上
    {
        if(itemUI.GetItem())//当前格子有物品
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());//获取物品信息
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);//激活物品信息面板
        }
    }

    public void OnPointerExit(PointerEventData eventData)//鼠标离开格子
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);//关闭物品信息面板
    }

    void OnDisable()//背包关闭时执行
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);//关闭物品信息面板,防止背包关闭后仍显示信息
    }
}
