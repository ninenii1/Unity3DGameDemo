using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//引用后才可以创建NavMeshAgent这个类

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator anim;//获得Animator控制器

    private CharacterStats characterStats; 

    private GameObject attackTarget;//攻击的目标点

    private float lastAttackTime;//攻击CD冷却时间，作为计时器

    private bool isDead;

    private float stopDistance;
    void Awake()//自身变量的获取都放在Awake中调用，eg：Rigidbody、Animator等
    {
        //在游戏的最开始，最先获得这些变量的赋值，避免出现空引用的情况
        agent = GetComponent<NavMeshAgent>();//Player Controller可以控制它的agent来移动
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;//在一开始获得默认的停止距离
    }

    void OnEnable()//人物在场景中启用时注册，这样切换场景后才能使用鼠标控制人物
    {
        //将MoveToTarget方法加入到OnMouseClicked事件中
        MouseManager.Instance.OnMouseClicked += MoveToTarget;//访问事件OnMouseClicked，+=为添加订阅/注册事件的方式
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        //直接访问，注册GameManager
        GameManager.Instance.RegisterPlayer(characterStats);
    }
    void Start()
    {
        SaveManager.Instance.LoadPlayerData();//获取原有数据
    }

    void OnDisable()//人物在场景中消失时取消注册
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }


    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;//CurrentHealth的值为0则返回true

        if(isDead) //死亡时广播
        {
            GameManager.Instance.NotifyObservers();
        }

        SwitchAnimation();//调用

        lastAttackTime -= Time.deltaTime;//冷却时间衰减
    }
    private void SwitchAnimation()//控制动画切换 
    {
        //Speed参数设置与速度保持同步，agent.velocity获得速度，sqrMagnitude将得到的值转换为浮点数
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude); 
        anim.SetBool("Death",isDead);
    }

    public void MoveToTarget(Vector3 target)//移动到目标点的方法MoveToTarget
    {
        StopAllCoroutines();//终止所有协程，打断攻击，否则无法中途暂停攻击（系统自带的方法）
        if (isDead) return;//如果Player死亡则不能再移动

        agent.stoppingDistance = stopDistance;//在移动时停止距离为默认值
        //定义OnMouseClicked时写了需要Vector3，所以MoveToTarget方法必须包含这个参数
        agent.isStopped = false;//再次点击时还原停止的状态，攻击一次后才能进行移动
        agent.destination = target;
    }
    private void EventAttack(GameObject target)//攻击事件方法
    {
        if (isDead) return;//如果Player死亡则不能再攻击
        if (target != null)//攻击目标存在
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;//判断是不是暴击
            StartCoroutine(MoveToAttackTarget());//执行协程
        }
    }
    IEnumerator MoveToAttackTarget()//协程
    {
        agent.isStopped = false;//一开始为false，确保能执行下面的while判断
        agent.stoppingDistance = characterStats.attackData.attackRange;//攻击时停止距离变远

        transform.LookAt(attackTarget.transform);//将玩家转向攻击目标，LookAt为内置函数方法
        //敌人的位置和玩家当前的位置大于攻击距离

        //修改攻击范围参数
        while (Vector3.Distance(attackTarget.transform.position,transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination  = attackTarget.transform.position;//玩家自动移动到敌人身边
            yield return null;//大于攻击距离则持续循环，小于则跳出循环
        }

        agent.isStopped = true;//走到敌人面前之后停下，true代表停止
        //Attack
        if(lastAttackTime < 0)//CD冷却结束，执行攻击方法
        {
            anim.SetBool("Critical",characterStats.isCritical);//判断是否暴击并实现
            anim.SetTrigger("Attack");//触发一次攻击动画
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;//每0.5s可以攻击一次
        }
    }
    //Animation Event
    void Hit()
    {
        if(attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;//修改石头的状态
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;//使攻击石头后可以销毁石头
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);//攻击石头
            }
                
        }
        else//执行对敌人的伤害
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//获得攻击目标身上的状态
            targetStats.TakeDamage(characterStats, targetStats);//受到伤害（当前人物，防御的）
        }
        
    }

}
