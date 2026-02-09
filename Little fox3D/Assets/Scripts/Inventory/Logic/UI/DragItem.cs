using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;//与UI组件产生交互

[RequireComponent(typeof(ItemUI))]//自动添加组件
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler//继承接口
{
    ItemUI currentItemUI;
    //创建两个变量用于交换数据（会用到第三方数据，进行存储和交换）
    SlotHolder currentHolder;//当前格子
    SlotHolder targetHolder;//目标格子

    void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();
    }
    public void OnBeginDrag(PointerEventData eventData)//一开始拖拽时
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();//创建一个新的值
        //临时保存原有的数据，记录原始数据（拖拽后没有位置放，则回到原始位置）
        InventoryManager.Instance.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;
        //修改父级，使图片在最上方显示（true代表保留原始参数，如比例、大小、旋转等，可不写）
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)//拖拽过程中
    {
        //自带PointerEventData类型的参数eventData
        //eventData.position：返回当前鼠标的坐标
        //eventData.pointerEnter：接收到鼠标进入某一区域时执行
        //跟随鼠标位置移动
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)//结束拖拽（松开鼠标）
    {
        //放下物品，交换数据
        if(EventSystem.current.IsPointerOverGameObject())//是否指向UI物品
        {
            if(InventoryManager.Instance.CheckInActionUI(eventData.position) || InventoryManager.Instance.CheckInEquipmentUI(eventData.position) ||
                InventoryManager.Instance.CheckInInventoryUI(eventData.position)) //判断鼠标是否在这些格子范围内
            {
                //判断鼠标指针进入的物体上是否包含SlotHolder组件（因为SlotHolder脚本中有UpdateItem方法，在交换物品之后UpdateItem才会更换图片和数量）
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>()) 
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>(); //包含则返回这个SlotHolder组件
                else
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();//不包含则要到父级中去找SlotHolder组件
                
                if(targetHolder != InventoryManager.Instance.currentDrag.originalHolder)//判断是不是同一个格子，不是才执行以下方法
                    switch (targetHolder.slotType)//不同类型的格子实现不同的方法，例如装备栏不能随意交换
                    {
                        case SlotType.BAG:
                            SwapItem();
                            break;
                        case SlotType.WEAPON:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.ARMOR:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;
                        case SlotType.ACTION:
                            if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;
                    }
                //交换之后要刷新
                currentHolder.UpdataItem();
                targetHolder.UpdataItem();
            }     
        }
        //拖拽的物品的ItemSlot移动会到另一个父对象Canvas中，要将它该回最初的Canvas，所以要更改它的父对象
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);

        //物品放置在两个格子之间时，格子的坐标会被改变，使物体无法放到格子中，而是放在两个格子之间
        RectTransform t = transform as RectTransform;
        t.offsetMax = -Vector2.one * 15;//offsetMax：矩形右上角相对于右上锚点的偏移，之前在项目中设置的位移是5
        t.offsetMin = Vector2.one * 15;//offsetMin：矩形左下角相对于左下锚点的偏移
    }

    public void SwapItem() //交换物品或增加数量
    {
        //改变背包列表，刷新背包，就会根据背包列表重新排列
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];//想要放置的目标物品栏，用var作为类型可以看到它真实返回的类
        var tempItem = currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];//拖拽的物品存放在第三方临时变量中，实现两个物品交换

        bool isSameItem = tempItem.itemData == targetItem.itemData;//判断是不是相同的物品

        if(isSameItem && targetItem.itemData.stackable)//是相同的物品且可堆叠，则将数量增加
        {
            targetItem.amount += targetItem.amount;
            //放置后将数据清除
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else //不是相同物品或不可堆叠，则交换物品
        {
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}
