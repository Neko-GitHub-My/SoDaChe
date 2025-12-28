using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNavSystem
{
    public static void StartRoadNavAction(ref AStarComponent nav, List<RoadNode> roadArray)
    {
        // nav.isWalking = true;
        nav.walkTime = nav.passedTime = 0.0f;
        nav.roadNodeArr = roadArray;
        nav.vx = nav.vz = 0;
        nav.nextIndex = 0;// 一开始你已经在第一个点; // [start, ..., end]

    }

    private static void WalkToNext(ref AStarComponent nav, ref TransformComponent uTransform)
    {
        // ref AStarComponent nav = ref e.uAStarNav;


        Vector2 startPos = new Vector2(uTransform.pos.x, uTransform.pos.z);
        Vector2 endPos = new Vector2(nav.roadNodeArr[nav.nextIndex].px, nav.roadNodeArr[nav.nextIndex].py);

        Vector2 dir = endPos - startPos;
        float len = dir.magnitude;
        nav.walkTime = len / nav.speed; // 可能后续要叠加我们的buff, 结合当前的Buff系统，来计算玩家的最终移动速度;
        nav.passedTime = 0;

        nav.vx = dir.x * nav.speed / len;
        nav.vz = dir.y * nav.speed / len;

        // 同步一下我们角色的方向, 角度为"度";
        uTransform.eulerAngles.y = (MathF.Atan2(dir.y, dir.x) * 180) / MathF.PI;

    }

    public static void StopNavAction(ref AStarComponent nav, ref StatusComponent uStatus)
    {
        // ref AStarComponent nav = ref e.uAStarNav;
        // nav.isWalking = false;
        nav.roadNodeArr = null;

        EntityWrapper.SetEntityStatus(ref uStatus, (int)CharactorStatus.Idle);
    }

    public static void NavRoadUpdate(ref AStarComponent nav, ref StatusComponent uStatus, ref TransformComponent uTransform, float dt)
    {
        // ref AStarComponent nav = ref e.uAStarNav;
        /*if (nav.isWalking == false) {
            return;
        }*/
        if (nav.roadNodeArr == null) {
            return;
        }

        if (nav.passedTime >= nav.walkTime)
        {
            nav.nextIndex++;
            if (nav.nextIndex >= nav.roadNodeArr.Count)
            { //  行走到了目的地
                AStarNavSystem.StopNavAction(ref nav, ref uStatus);
                return;
            }

            AStarNavSystem.WalkToNext(ref nav, ref uTransform);
        }

        nav.passedTime += dt;
        if (nav.passedTime > nav.walkTime)
        {
            dt -= (nav.passedTime - nav.walkTime);
        }

        uTransform.pos.x += (nav.vx * dt);
        uTransform.pos.z += (nav.vz * dt);


    }
}
