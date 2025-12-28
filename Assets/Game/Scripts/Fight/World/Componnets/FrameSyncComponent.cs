using UnityEngine;

// 存放每帧同步后的精确位置
public struct FrameSyncComponent
{
    public int status;

    // 对于精度要求搞的，可以考虑使用定点数;
    public Vector3 pos;   
    public Vector3 eulerAngles;

    public float vx;
    public float vz;
}
