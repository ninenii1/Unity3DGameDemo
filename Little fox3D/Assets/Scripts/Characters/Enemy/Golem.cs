using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("skill")]
    public float kickForce = 25;//击飞Player的力,控制击飞距离
    public GameObject rockPrefab;//石头预制体
    public Transform handPos;//手部位置

    //Animation Event：需要在Animation（Ctrl+6）面板中的某一帧上添加事件
    public void KickOff()//Animation Event:击飞Player
    {
        //敌人的攻击目标不为空，且攻击目标在敌人正前方，攻击目标才会受到伤害
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //写法一：
            //Vector3 direction = attackTarget.transform.position - transform.position;
            //direction.Normalize();//将方向向量化
            //写法二：
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //击飞前需要关掉Agent
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            //击飞
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //Player被击晕
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            //伤害
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
    public void ThrowRock()//Animation Event:扔石头
    {
        if(attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;//获取rock中的脚本，访问target变量
        }
    }
}
