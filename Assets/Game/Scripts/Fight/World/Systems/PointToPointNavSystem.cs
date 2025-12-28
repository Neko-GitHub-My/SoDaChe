using UnityEngine;

public class PointToPointNavSystem 
{
    public static void StartJoystickNavAction(BaseEntity e, float speed, int dirx, int diry, float runTime = 10 * 50)
    {
        if (dirx == 0 && diry == 0) {
            EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
            return;
        }

        e.uNav.vx = (((float)(dirx)) / (1 << 16)) * speed;
        e.uNav.vz = (((float)(diry)) / (1 << 16)) * speed;
        e.uTransform.eulerAngles.y = (Mathf.Atan2(e.uNav.vz, e.uNav.vx) * 180) / Mathf.PI;

        e.uNav.totalTime = runTime; // 移动的时候,向前预测10个帧;
        e.uNav.passedTime = 0;
        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Run);
    }

    public static void StartNavAction(BaseEntity e, float speed, float x, float y, float z)
    {
        // 处理导航;
        Vector3 dst = new Vector3(x, y, z);
        Vector3 dir = dst - e.uTransform.pos;
        float len = dir.magnitude;
        if (len <= 0)
        {
            return;
        }
        // end 

        // Debug.Log(dst.ToString() + ":" + e.uTransform.pos.ToString());

        e.uTransform.eulerAngles.y = (Mathf.Atan2(dir.z, dir.x) * 180) / Mathf.PI;

       
        e.uNav.totalTime = len / speed;
        e.uNav.vx = (dir.x / len) * speed;
        e.uNav.vz = (dir.z / len) * speed;
        e.uNav.passedTime = 0;

        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Run);
    }

    public static void Update(BaseEntity e, float dt) {
        
        e.uNav.passedTime += dt;
        if (e.uNav.passedTime > e.uNav.totalTime)
        {
            dt -= (e.uNav.passedTime - e.uNav.totalTime);
        }

        e.uTransform.pos.x += (e.uNav.vx * dt);
        e.uTransform.pos.z += (e.uNav.vz * dt);
        // e.uAnim.unityObject.transform.position = e.uTransform.pos;

        if (e.uNav.passedTime >= e.uNav.totalTime) {
            EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
        }

    }
}
