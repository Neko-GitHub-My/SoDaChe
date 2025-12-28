using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PropsComponent
{
    public float speed; // 玩家的移动速度，

    public int hp;
    public int attack; // 基本的攻击力，不包含buff叠加
    public int defense; // 基本的防御力，不包含buff叠加

    public static void Init(ref PropsComponent uProps/*暂时在代码写死，后续Config传入*/)
    {
        uProps.speed = 5.0f;
        uProps.hp = 500;
        uProps.attack = 75;
        uProps.defense = 50;
    }
}
