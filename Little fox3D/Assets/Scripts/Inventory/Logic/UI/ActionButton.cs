using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;//为每个框设置不同的快捷键
    private SlotHolder currentSlotHolder;

    //按下按键，对应的SlotHolder执行UseItem()来使用物品
    void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    void Update()
    {   //检测按下哪个按键，且物品栏上要有物品，GetItem()可以获取实际物品的数据
        if (Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItem())
        {
            currentSlotHolder.UseItem();
        }
    }
}
