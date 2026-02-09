using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;//物品名字
    public Text itemInfoText;//物品信息介绍

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void SetupTooltip(ItemData_SO item)
    {
        itemNameText.text = item.itemName;
        itemInfoText.text = item.description;
    }
    void OnEnable()
    {
        UpdatePosition();//面板被激活时先更新一次坐标，防止面板闪烁
    }
    void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()//面板位置随鼠标位置而变化
    {
        Vector3 mousePos = Input.mousePosition;//获取鼠标位置

        Vector3[] corners = new Vector3[4];//数组中有四个值
        rectTransform.GetWorldCorners(corners);//获取矩形在世界空间中的各个角
        //得到矩形面板的宽高
        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;
        //鼠标到屏幕下方的距离小于矩形的高度，则面板生成在鼠标上方
        //面板的锚点在面板的中心点，生成在鼠标上方需要用鼠标位置加上面板高度的一半
        //屏幕的中心点在左下角，鼠标位置在左下角时为（0,0）
        if (mousePos.y < height)
            rectTransform.position = mousePos + Vector3.up * height * 0.6f;//防止面板闪烁卡顿，写0.6
        else if (Screen.width - mousePos.x > width)
            rectTransform.position = mousePos + Vector3.right * width * 0.6f;
        else
            rectTransform.position = mousePos + Vector3.left * width * 0.6f;
    }
}
