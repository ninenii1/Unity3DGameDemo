//当结束游戏时，给观察者们调用的方法
public interface IEndGameObserver 
{
    //只写方法定义，在调用接口的脚本中写函数的具体方法
    void EndNotify();//结束游戏广播
}
