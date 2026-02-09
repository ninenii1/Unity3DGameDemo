using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//才能使用NavMeshAgent

public class Grunt : EnemyController
{
    [Header("skill")]
    public float kickForce = 10;//击飞Player的力,控制击飞距离
    public void kickOff()
    {
        if(attackTarget != null)//如果攻击目标不为空（attackTarget改为protected，子类才能访问）
        {
            transform.LookAt(attackTarget.transform);//面向攻击目标

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//量化该方向，值为0或-1或1
            //打断Player的移动或动作
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            //击飞Player
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //Player产生眩晕
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
