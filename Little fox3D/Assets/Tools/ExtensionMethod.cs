using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod//设置为静态类，可以随时随地调用
{
    private const float dotThreshold = 0.5f;

    //判断当前人物是否面向攻击目标IsFacingTarget(this 扩展对应的类，函数变量)
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        //获得攻击目标的相对位置
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();//得到向量方向
        //Vector3.Dot:两个向量的点积，指向完全相同的方向返回1，相反返回-1，垂直返回0
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        return dot >= dotThreshold;//dot>=0.5则执行攻击
    }
}
