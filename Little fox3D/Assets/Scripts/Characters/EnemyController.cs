using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//获得agent的控制权

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD }//敌人的不同状态
[RequireComponent(typeof(NavMeshAgent))]//自动添加NavMeshAgent组件
[RequireComponent(typeof(CharacterStats))]//自动添加组件CharacterStats
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;

    private Collider coll;//可以获取任意类型的Collider

    protected CharacterStats characterStats; 

    [Header("Basic Settings")] //用Header进行区分，方便管理【基础设置】
    public float sightRadius;//可视范围，当Player进入敌人的可视范围，敌人进入追击状态

    public bool isGuard;//在脚本下勾选来确定是哪个类型的敌人（勾选则为站桩，否则为巡逻）
    //希望巡逻和追击玩家之后回到原位的速度为原来的一半，追击玩家为原始速度
    private float speed;//记录原来的速度，仅在系统中记录
    protected GameObject attackTarget;//获取玩家作为敌人的攻击目标

    public float LookAtTime;//敌人走到目标点停下来看的时间，在窗口中赋值
    private float remainLookAtTime;//计时器，仍然需要等待的时间
    private float lastAttackTime;//技能冷却时间计时器

    private Quaternion guardRotation;//记录敌人本身的旋转角度

    [Header("Patrol State")]//【巡逻状态】
    public float PatrolRange;//巡逻范围
    private Vector3 wayPoint;//移动的坐标位置
    private Vector3 guardPos;//存储初始坐标位置

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFolllow;
    bool isDead;

    bool playerDead;//记录Player是否死亡，初始值为false
    void Awake()
   {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;//起始速度为agent原来的速度
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = LookAtTime;
   }

    void Start()//在一开始判断敌人的类型
    {
        if(isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            //wayPoint在初始状态下没有赋值，无法判断是否走到目标点，所以在一开始给一个可移动范围内的点
            GetNewWayPoint();
        }
        //FIXME:场景切换后修改掉
        //在OnEnable中没有找到GameManager，无法加入观察者列表
        GameManager.Instance.AddObserver(this);//加入观察，添加到列表
    }
    //切换场景使启用
    //void OnEnable() //启用时
    //{
    //GameManager的Awake比这里的OnEnable后启动，导致这里GameManager为空，因此才会报错
    //    GameManager.Instance.AddObserver(this);//加入观察，添加到列表
    //}

    void OnDisable() //敌人被销毁完成之后调用
    {
        //OnDisable在人物消失的时候和游戏停止的伤害调用，会导致编辑器的额外的报错，需要使用if解决
        if (!GameManager.IsInitialized) return;//GameManager没有被生成，则return使后面的命令不会执行
        GameManager.Instance.RemoveObserver(this);//移出列表
        if(GetComponent<LootSpawner>() && isDead)//敌人身上有该脚本且死亡时执行
        {
            GetComponent<LootSpawner>().Spawnloot();//随机掉落物品
        }
    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        if (!playerDead)//player存在才执行以下命令，否则player死亡后敌人仍旧在攻击player
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;//保证在日常状态下lastAttackTime一直小于CD时间，才能随时进行攻击
        }
    }

    void SwitchAnimation()//用来切换动画
    {
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase",isChase);
        anim.SetBool("Follow",isFolllow);
        anim.SetBool("Critical", characterStats.isCritical);//暴击动画
        anim.SetBool("Death", isDead);
    }
    void SwitchStates()//判断当前枚举的类型是什么
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;//切换死亡模式

        //如果发现Player，切换到追击状态CHASE
        else if(FoundPlayer())//需要添加else，否则敌人死亡后仍会执行
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD://站桩怪（待在原地，玩家靠近时追踪玩家）
                isChase = false;
                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;//才能移动
                    agent.destination = guardPos;
                    //SqrMagnitude可以计算两个三维向量之间的差值，比用Distance的性能开销小
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        //回到原位后停止
                        isWalk = false;
                        //缓慢旋转：Quaternion.Lerp（最开始的角度，目标角度，float数值【值越小，旋转得越慢】）
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL://巡逻怪（按照巡逻点移动）
                isChase = false;//进入正常移动
                agent.speed = speed * 0.5f;//移动速度为追击时的一半
                //判断当前坐标与目标wayPoint的坐标是否相同，相同则给一个新的坐标点，不同则往目标点移动
                //Distance计算两个点之间的距离，stoppingDistance在NavMeshAgent中设置
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    //已走到目标点
                    isWalk = false;
                    //判断是否在停止时间内
                    if(remainLookAtTime > 0)//敌人停止
                        remainLookAtTime -= Time.deltaTime;//逐渐减少等待时间
                    else
                    GetNewWayPoint();//获取新的点
                }
                else
                {
                    //没有到达给定目标点
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE://追击状态
                //追击Player

                //配合动画
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                //1.如果没有发现Player
                if (!FoundPlayer())
                {
                    //Player超出范围则回到之前的状态
                    isFolllow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;//敌人停止
                        remainLookAtTime -= Time.deltaTime;
                    }
                    //判断原有状态是什么，再返回到之前的状态，否则会停止不动
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;//返回到之前的状态
                    else
                        enemyStates = EnemyStates.PATROL;

                }
                else//发现Player
                {
                    isFolllow = true;
                    agent.isStopped = false;//敌人攻击之后玩家逃跑后，敌人才能继续移动
                    agent.destination = attackTarget.transform.position;//追Player
                }
                //在攻击范围内则攻击
                if(TargetInAttackRange()||TargetInSkillRange())
                {
                    isFolllow = false;//停止跟随Player动画
                    agent.isStopped = true;//停在原地
                    if(lastAttackTime < 0)//可以进行攻击
                    {
                        lastAttackTime = characterStats.attackData.coolDown;//恢复CD时间

                        //暴击判断
                        //Random.value可以随机返回0.0-1.0之间的数值,如果小于暴击率则可以执行暴击，返回true
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD://死亡状态
                //【注意代码的顺序，写在前面会优先执行】
                coll.enabled = false;//关闭碰撞体组件，使敌人死亡后，玩家无法再攻击敌人
                //agent.enabled = false;//直接将组件关闭，为了敌人死亡后不会挡到Player前进
                agent.radius = 0;//将agent的范围缩小，则不会成为障碍物挡到Player前进

                Destroy(gameObject, 2f);//2s后敌人消失
                break;
        }
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);//敌人面向Player
        if(TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        if(TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()//发现Player是一个布尔值判断
    {
        //物理判断周围一个球体的范围之内是否有Player【Physics.OverlapSphere(球体中心,球体半径)】
        //colliders[] 返回一个数组，其中包含与球体接触或位于球体内部的所有碰撞体。
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//需要给Player添加(胶囊)碰撞体
        foreach (var target in colliders)//逐一循环遍历
        {
            if(target.CompareTag("Player"))//需要为Player设置标签
            {
                attackTarget = target.gameObject;//找到了标签为Player的物体则赋值给attackTarget
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInAttackRange()
    {
        if(attackTarget != null)
            //检测到Player且Player与敌人的距离小于基本攻击距离则返回true
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            //检测到Player且Player与敌人的距离小于技能攻击距离则返回true
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    void GetNewWayPoint()//随机生成坐标点
    {
        remainLookAtTime = LookAtTime;//已经过了停止时间，还原数值
        //Y值保存不变，不需要上下移动
        float randomX = Random.Range(-PatrolRange, PatrolRange);//PatrolRange为巡逻范围
        float randomZ = Random.Range(-PatrolRange, PatrolRange);
        //在初始坐标位置上加一个随机值，由于地形高低不平，所以Y轴需要基于当前坐标的高度加随机值
        //randomPoint直接作为wayPoint时，遇到障碍物会卡住无法行动
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        //以下为判断点是否为可移动范围，是则赋值给wayPoint
        NavMeshHit hit;//存储选中点坐标的相关信息
        //NavMesh.SamplePosition是在指定范围内找到导航网格上最近的点，找到符合范围的点则返回true
        //NavMesh.SamplePosition(随机生成的点，点的相关信息，判断范围，可以碰撞到哪个Areas【Navigation中Areas的Walkable值为1】）
        //NavMesh.SamplePosition(randomPoint, out hit, PatrolRange, 1)成立则wayPoint=randomPoint，否则保持不动，会再次获得一个新目标点
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, PatrolRange, 1) ? hit.position : transform.position;
    }

    void OnDrawGizmosSelected()//在scene中画出可视范围sightRadius，选中目标才会显示
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//绘制球形线条，（坐标中心点，范围）
    }

    //Animation Event
    void Hit()
    {
        //敌人的攻击目标不为空，且攻击目标在敌人正前方，攻击目标才会受到伤害
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        playerDead = true;//player已经死亡
        //获胜动画
        anim.SetBool("Win" , true);
        
        //停止所有移动
        isChase = false;
        isWalk = false;
        //停止Agent
        attackTarget = null;
    }
}
