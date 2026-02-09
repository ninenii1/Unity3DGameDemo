using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型单例: Singleton<类型>【T代表type】。where是约束，代表是Singleton类型
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;//T可以代表所有的Manager类型

    public static T Instance //在外部可以访问
    {
        get { return instance; }//单例模式是唯一的，不需要被更改，所以只可读
    }

    //使用类的继承的方式，继承这个泛型类的其他类都可以再次更改这个Awake方法
    //protected只允许继承类可以访问的变量/函数方法，virtual可以在继承函数中进行重写
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }

    public static bool IsInitialized //返回当前泛型单例模式是否已经生成
    {
        get { return instance != null; }//不为空则返回true，代表已经生成
    }

    protected virtual void OnDestroy()//清空当前static静态的类的变量
    {
        if(instance == this)
        {
            instance = null;//被销毁时设置为空
        }
    }
}
