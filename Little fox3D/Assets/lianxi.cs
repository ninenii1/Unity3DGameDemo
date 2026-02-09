using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lianxi : Singleton<lianxi>
{
    public List<lian> la = new List<lian>();
}

[System.Serializable]
public class lian
{
     public int an;
}
