using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler,IPointerDownHandler
{
    RectTransform rectTransform;//面板坐标跟随鼠标位移进行移动
    Canvas canvas;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = InventoryManager.Instance.GetComponent<Canvas>();//运行前需要将背包面板关闭，否则可能会出错
    }
    public void OnDrag(PointerEventData eventData)
    {
        //调用OnDrag时，delta会更新，delta的值会发生很大变化
        //面板中心坐标位置加上鼠标的移动偏移，使用面板的Anchors
        //除以canvas.scaleFactor，使匹配当前屏幕
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;//实现鼠标拖动面板，delta的值为鼠标每一帧产生的位移
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);//设置此时拖拽的面板索引为2，显示在最上方
    }
}
