using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [x, 0, z] 我们2D游戏与3D的平面游戏，都可以使用;
public struct AStarComponent
{
    public List<RoadNode> roadNodeArr; // 保存我们寻路的路径点的数据;

    public float walkTime; // 从当前点走到下一个点的所需要的时间;
    public float passedTime; // 当前已经过去了多少时间;

    public float speed; // 移动的速度
    public float vx; // 移动时候的x方向的速度;
    public float vz; // 移动时候的z方向的速度;

    // public bool isWalking; // 标记一下当前的entity是否正在行走;
    public int nextIndex; // 当前走到第几个点了;

    public static void Init(ref AStarComponent uAStarNav)
    {
        uAStarNav.nextIndex = -1;
        uAStarNav.vx = uAStarNav.vz = 0;
        // e.uAStarNav.isWalking = false;
        uAStarNav.roadNodeArr = null;
    }
}
