using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;//生成的物品
        [Range(0,1)]//锁定百分比的值的0到1之间
        public float weight;//百分比
    }
    public LootItem[] lootItems;//创建数组

    public void Spawnloot()
    {
        float currentValue = Random.value;//返回0-1之间的随机值
        for(int i = 0; i < lootItems.Length; i++)
        {
            if(currentValue <= lootItems[i].weight)//随机值小于物品的权重，则将该物品掉落下来
            {
                GameObject obj = Instantiate(lootItems[i].item);
                obj.transform.position = transform.position + Vector3.up * 2;//在敌人位置上方掉落
                break;//确保一次只掉落一个物品
            }
        }
    }
}
