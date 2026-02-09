using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders;

    public void RefreshUI() //统一刷新整个背包界面
    {
        for(int i = 0; i < slotHolders.Length; i++)
        {
            slotHolders[i].itemUI.Index = i;
            slotHolders[i].UpdataItem();
        }
    }
}
