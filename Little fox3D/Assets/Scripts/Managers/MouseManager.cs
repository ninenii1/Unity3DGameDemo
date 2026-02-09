using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//可以直接创建event
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;
//using UnityEngine.Events;//引用该命名空间才能使用Event功能


//在unity编辑器中可以查看event事件，为了舍弃拖拽的方式因此注释掉
//[System.Serializable]//系统序列化才能显示代码,在Unity中成功创建Event事件（因为上面的class不是继承MonoBehaviour）
//public class EventVector3 : UnityEvent<Vector3> { } //随便命名（需要一个Vector3知道往哪个世界坐标移动)：继承UnityEvent<引用类型>

public class MouseManager : Singleton<MouseManager> //继承泛型单例
{
    //public static MouseManager Instance;//创建一个static的自身变量Instance

    public Texture2D point, doorway, attack,target, arrow;

    RaycastHit hitInfo;//保持射线碰撞到的物体的相关信息，RaycastHit类型可以返回collider、point、transform等多种值
    //public EventVector3 OnMouseClicked;//创建OnMouseClicked事件

    //OnMouseClicked运用在PlayerController脚本中，通过“+=”订阅使用
    //OnMouseClicked相当于一个门铃（事件），Invoke()是按门铃的动作，通知所有订阅者
    public event Action<Vector3> OnMouseClicked;//创建点击地面event，关键字event 类型Action<变量参数Vector3>
    public event Action<GameObject> OnEnemyClicked;//点击敌人

    //单例模式不希望在转换场景时被销毁
    protected override void Awake()
    {
        base.Awake();//类的继承中，base代表基于原有父类里面的函数方法之上，额外运行的
        DontDestroyOnLoad(this);
    }

    //void Awake()//使Instance成为MouseManager中唯一的实例
    //{
    //    //保证只有一个MouseManager被创建，把当前对象赋值给Instance
    //    if (Instance != null)
    //        Destroy(gameObject);//不为空则删除多余部分，保证只有一个实例存在

    //    Instance = this;

    //}

    void Update()
    {
        SetCursorTexture();//实时检测射线相关信息，并持续返回信息
        if (InteractWithUI()) return;
        MouseControl();
    }

    void SetCursorTexture()//设置指针贴图
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发射的射线，用鼠标点击的点返回射线

        if(Physics.Raycast(ray,out hitInfo))
        {
            //切换鼠标贴图
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    //Vector2记录偏移值，只有鼠标左上角才能判断点击，圆圈鼠标的半径为32
                    //target是鼠标指针方向位置的图片，Auto为自动切换
                    //Cursor.SetCursor(图片，光标位置偏移，光标模式)
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);//设置图片
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);//设置图片
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);//设置图片
                    break;
                case "Item": //可捡起的物品
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);//设置图片
                    break;
                default:  //在默认情况下为指针图标
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);//设置图片
                    break;
            }
        }

    }
    void MouseControl()//控制鼠标
    {
        if(Input.GetMouseButtonDown(0) && hitInfo.collider != null)//用鼠标左键点击且位置不是空值（点击地图外则为空值）
        {
            if(hitInfo.collider.gameObject.CompareTag( "Ground"))//当前碰撞物体的标签是地面，需要先给地面添加标签
                //点击鼠标时触发事件OnMouseClicked，所有订阅这个事件添加的方法都会被执行，写在PlayerController脚本中
                //?.是空值条件运算符。只有当 OnMouseClicked 不为 null 时，才调用它，否则什么都不做，避免了空引用异常
                OnMouseClicked?.Invoke(hitInfo.point);//判断当前碰撞物体是否为空，不为空则执行Invoke
                                                      //Invoke用于延迟调用某个函数，点击到地面时就会执行所有加入到OnMouseClicked里面的函数方法
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))//鼠标可以点击石头进行攻击
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Portal"))//传送
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Item"))
                OnMouseClicked?.Invoke(hitInfo.point);
        }
    }

    bool InteractWithUI() //判断是否与UI面板有互动
    {
        //IsPointerOverGameObject是否指向UI物品
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else return false;
    }
}
