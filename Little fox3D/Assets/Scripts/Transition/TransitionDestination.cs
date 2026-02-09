using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag //设置终点的类型
    {
        ENTER,A,B,C //场景入口，A点，B点...
    }
    public DestinationTag destinationTag;
}
