using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//脚本挂在新建的空物体中，该空物体是图标和数量的父物体
public class ItemUI : MonoBehaviour
{
    public Image icon = null;//将其子物体添加到脚本下
    public Text amount = null;

    public InventoryData_SO Bag { get; set; } //Bag表示当前格子对应的是哪个背包的数据源（普通背包、装备栏、快捷栏）
    public int Index { get; set; } = -1; //用get和set是为了外部能访问和修改它的值
    
    public void SetUpItemUI(ItemData_SO item, int itemAmount)//获取物品图片和数量
    {
        if(itemAmount == 0)
        {
            Bag.items[Index].itemData = null;//物品数量为0时，在背包中不显示图片
            icon.gameObject.SetActive(false);
            return;//失活后不执行下面的命令，避免产生冲突
        }
        if(item != null)
        {
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();//转换为数字
            icon.gameObject.SetActive(true);
        }
        else
            icon.gameObject.SetActive(false);
    }
    public ItemData_SO GetItem()//获取物品
    {
        return Bag.items[Index].itemData;
    }
}
