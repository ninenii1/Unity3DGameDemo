using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData //获取第三方数据
    {
        public SlotHolder originalHolder;//原来的Holder
        public RectTransform originalParent;//原来的Parent
    }

    //TODO:最后添加模板用于保存数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData; //用于做测试时可以看到数据的更改
    public InventoryData_SO actionTemplate;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;

    [Header("Containers")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;//将拖拽物品的父级设置为这个Canvas，图片就会在所有UI组件的最上方显示
    public DragData currentDrag;//在拖拽的一开始，创建一个新的数据类型的值，传输给currentDrag，临时保存新创建的数据

    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;

    bool isOpen = false;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        //保证新建数据时，这些数据是空的
        if(inventoryTemplate != null) 
            inventoryData = Instantiate(inventoryTemplate);
        if (inventoryTemplate != null)
            actionData = Instantiate(actionTemplate);
        if (inventoryTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);
    }

    void Start()
    {
        LoadData();//游戏开始时加载数据
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))//按下B键打开或关闭背包界面
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
        
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, 
                        GameManager.Instance.playerStats.attackData.maxDamage);
    }

    public void SaveData()//保存数据
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()//加载数据
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }
    public void UpdateStatsText(int health, int min, int max)
    {
        healthText.text = health.ToString();
        attackText.text = min + " - " + max;
    }

    #region 检查拖拽物品是否在每一个Slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for(int i = 0; i < inventoryUI.slotHolders.Length; i++)//循环每个格子的坐标
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;//获得每个格子的RectTransform
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;//代表在背包的三十个格子内
            }
        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}
