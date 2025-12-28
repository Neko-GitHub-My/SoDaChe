using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class RvoNavSystem {

    private static List<Vector3[]> obsSet = null;

    public static void StartRvoNavAction(BaseEntity e, float speed, int dirx, int diry)
    {
        e.uRvo.speed = speed;

        if (dirx == 0 && diry == 0)
        { // 摇杆弹起
            e.uRvo.type = -1;
            SetAgentPrefVelocity(e.uRvo.agentId, new Vector3(0, 0, 0));
            EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
            return;
        }

        e.uRvo.type = 1;

        e.uRvo.dirx = ((float)(dirx)) / (1 << 16);
        e.uRvo.diry = ((float)(diry)) / (1 << 16);

        float len = MathF.Sqrt(e.uRvo.dirx * e.uRvo.dirx + e.uRvo.diry * e.uRvo.diry);

        e.uRvo.dirx = e.uRvo.dirx / len;
        e.uRvo.diry = e.uRvo.diry / len;
    }

    public static void StartRvoNavAction(BaseEntity e, float speed, float x, float y, float z)
    {
        e.uRvo.speed = speed;
        e.uRvo.type = 0;
        e.uRvo.targetPos.x = x;
        e.uRvo.targetPos.y = y;
        e.uRvo.targetPos.z = z;
    }

    public static void SetAgentPrefVelocity(int agentId, Vector3 v)
    {
        RVO.Simulator.Instance.setAgentPrefVelocity(agentId, new RVO.Vector2(v.x, v.z));
    }

    public static void NavAgentUpdate(BaseEntity e, float dt) {
        Vector3 v;

        if (e.uRvo.type == 1) {
            
            v.x = e.uRvo.dirx * e.uRvo.speed;
            v.z = e.uRvo.diry * e.uRvo.speed;
            v.y = 0;
            SetAgentPrefVelocity(e.uRvo.agentId, v);
            return;
        }

        Vector3 pos = GetAgentPosition(e.uRvo.agentId);
        Vector3 dir = e.uRvo.targetPos - pos;
        float len = dir.magnitude;

        if (len <= 0.3f)
        { // 这里是根据游戏效果来调整就可以了;

            SetAgentPosition(e.uRvo.agentId, e.uRvo.targetPos);
            SetAgentPrefVelocity(e.uRvo.agentId, new Vector3(0, 0, 0));
            EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
            return;
        }

        v.x = dir.x * e.uRvo.speed / len;
        v.z = dir.z * e.uRvo.speed / len;
        v.y = 0;
        SetAgentPrefVelocity(e.uRvo.agentId, v);
    }

    public static void InitRVO() {
        // RVO.Simulator.Instance.setTimeStep(0.25f);
        RVO.Simulator.Instance.setAgentDefaults(15.0f, 10, 5.0f, 5.0f, 2.0f, 2.0f, new RVO.Vector2(0.0f, 0.0f));
    }

    public static void ClearRVO()
    {
        RVO.Simulator.Instance.Clear();
        obsSet = null;
    }

    public static void SetAgentPosition(int agentId, Vector3 v)
    {
        RVO.Simulator.Instance.setAgentPosition(agentId, new RVO.Vector2(v.x, v.z));
    }

    public static Vector3 GetAgentPosition(int agentId)
    {
        RVO.Vector2 pos = RVO.Simulator.Instance.getAgentPosition(agentId);

        return new Vector3(pos.x(), 0, pos.y());
    }



#if UNITY_EDITOR
    public static void RVODebugDrawAgent(int agentId)
    {
        if (agentId == -1)
        {
            return;
        }

        Vector3 center = GetAgentPosition(agentId);
        float r = GetAgentRaduis(agentId);

        center.y += 0.2f;
        RVO.DebugDraw.Editor.Draw.Circle(center, r, Color.green);
    }
#endif

    public static float GetAgentRaduis(int agentId)
    {
        return RVO.Simulator.Instance.getAgentRadius(agentId);
    }

    public static void DestroyAgent(int agentId)
    {
        RVO.Simulator.Instance.delAgent(agentId);
    }


    public static int CreateAgent(Vector3 pos, float r)
    {
        int agentId = RVO.Simulator.Instance.addAgent(new RVO.Vector2(pos.x, pos.z));
        RVO.Simulator.Instance.setAgentRadius(agentId, r);
        return agentId;
    }

    public static void RVOAddObstacles(List<Vector3[]> obsSet)
    {
        for (int i = 0; i < obsSet.Count; i++)
        {
            List<RVO.Vector2> obsData = new List<RVO.Vector2>();
            for (int j = 0; j < obsSet[i].Length; j++)
            {
                obsData.Add(new RVO.Vector2(obsSet[i][j].x, obsSet[i][j].z));
            }

            RVO.Simulator.Instance.addObstacle(obsData);
        }

        RVO.Simulator.Instance.processObstacles();
        RvoNavSystem.obsSet = obsSet;
    }

    public static void NavAgentLateUpdate(BaseEntity e, float dt)
    {
        // 同步Transform组件上的位置和旋转
        RVO.Vector2 pos = RVO.Simulator.Instance.getAgentPosition(e.uRvo.agentId);
        RVO.Vector2 vel = RVO.Simulator.Instance.getAgentVelocity(e.uRvo.agentId);
        e.uTransform.pos = new Vector3(pos.x(), 0, pos.y());

        if (Math.Abs(vel.x()) > 0.0005f && Math.Abs(vel.y()) > 0.0005f)
        {
            e.uTransform.eulerAngles.y = (MathF.Atan2(vel.y(), vel.x()) * 180) / MathF.PI;
        }

#if UNITY_EDITOR
        RVODebugDrawAgent(e.uRvo.agentId);
#endif
    }

    public static void Update(float dt) {
        RVO.Simulator.Instance.setTimeStep(dt);
        RVO.Simulator.Instance.doStep();
    }

    public static void RVODebugDrawMapEdge()
    {
        if (obsSet == null) {
            return;
        }
        int oCount = obsSet.Count, subCount;
        for (int i = 0; i < oCount; i++)
        {
            Vector3[] o = obsSet[i];
            subCount = o.Length;

            //Draw each segment
            for (int j = 1, count = o.Length; j < count; j++)
            {
                RVO.DebugDraw.Editor.Draw.Line(o[j - 1], o[j], Color.red);
                RVO.DebugDraw.Editor.Draw.Circle(o[j - 1], 0.2f, Color.magenta, 6);
            }

            RVO.DebugDraw.Editor.Draw.Line(o[subCount - 1], o[0], Color.red);
        }
    }
}

