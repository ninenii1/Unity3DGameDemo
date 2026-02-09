using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing }//石头的三个状态
    private Rigidbody rb;
    public RockStates rockStates;//存放石头的状态
    [Header("Basic Settings")]//基础设置
    public float force;
    public int damage;//石头的基本伤害
    public GameObject target;//目标，获取Player
    private Vector3 direction;

    public GameObject breakEffect;//碎石粒子
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;//初始值为1，可以跳过下面对速度是否小于1的判断
        rockStates = RockStates.HitPlayer;//初始状态
        FlyToTarget();
    }
    void FixedUpdate()//物理判断不用Update
    {
        //sqrMagnitude可以返回向量的平方长度，用来计算石头的速度，计算速度比Magnitude快得多
        if (rb.velocity.sqrMagnitude < 1f)//石头掉落到地上逐渐停止（速度<1)则改变状态
        {
            rockStates = RockStates.HitNothing;//变为HitNothing状态后，Player可以打到石头
        }
    }
    public void FlyToTarget()//石头扔向目标
    {
        if (target == null)//目标为空，则主动找到Player，避免目标突然丢失时，石头直接原地砸下
            target = FindObjectOfType<PlayerController>().gameObject;

        //扔的方向判断，Vector3.up使石头能在天上飞一会，而不是垂直砸向Player
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);//Impulse为力的模式，一瞬间扔出去
    }

    void OnCollisionEnter(Collision other)
    {
        switch(rockStates)
        {
            case RockStates.HitPlayer://石头攻击到Player
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;//Player停止移动
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;//击飞Player
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");//播放眩晕动画
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());//产生伤害

                    rockStates = RockStates.HitNothing;//攻击完成后切换到别的状态，不希望持续对Player产生伤害
                }
                break;

            case RockStates.HitEnemy://石头攻击到自己
                if(other.gameObject.GetComponent<Golem>())//GetComponent中如果能获取到Golem则返回true
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//生成碎石
                    Destroy(gameObject);
                }
                break;
        }
    }
}
