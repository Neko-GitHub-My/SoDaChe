using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;



public class RectMapWrapper : IMapWrapper
{
    int mapWidth;
    int mapHeight;

    public void LoadMapData(int mapId, Transform mapRoot = null, UnityAction onEnd = null)
    {
        // 正式项目的时候，根据我们的mapId来加载地图的大小就可以了;
        this.mapWidth = 40;
        this.mapHeight = 40;

    }

    public int MapHeight()
    {
        throw new System.NotImplementedException();
    }

    public int MapWidth()
    {
        throw new System.NotImplementedException();
    }

    public void OnNavLateUpdate(CharactorEntity e, float dt)
    {
        // throw new System.NotImplementedException();
    }

    public void OnNavUpdate(CharactorEntity2D e, float dt)
    {
        throw new System.NotImplementedException();
    }

    public void OnNavUpdate(CharactorEntity e, float dt)
    {
        PointToPointNavSystem.Update(e, dt);
        EntityWrapper.SyncToUnityTransform(e);
    }

    public void SyncEntityAlphaWithMap(CharactorEntity2D e)
    {
        throw new System.NotImplementedException();
    }

    public void WalkInDir(CharactorEntity e, float speed, int dirx, int diry)
    {
        PointToPointNavSystem.StartJoystickNavAction(e, speed, dirx, diry);
    }

    public void WalkToDst(CharactorEntity e, float speed, float x, float y, float z)
    {
        PointToPointNavSystem.StartNavAction(e, speed, x, y, z);
    }


    public void WalkToDst(CharactorEntity2D e, float speed, float x, float z)
    {
        throw new System.NotImplementedException();
    }
}
