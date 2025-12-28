using UnityEngine;

public struct RVOComponent
{
    public int agentId;
    public Vector3 targetPos;

    public float speed;

    public float dirx;
    public float diry;

    public int type; // -1, 0,表示是目的地导航， 1, 表示是摇杆方向导航;

    public static void Init(ref RVOComponent uRvo)
    {
        uRvo.agentId = -1;
        uRvo.targetPos = Vector3.zero;
        uRvo.speed = 0;
        uRvo.diry = 0;

        uRvo.dirx = 0;
        uRvo.type = -1;
    }
}


