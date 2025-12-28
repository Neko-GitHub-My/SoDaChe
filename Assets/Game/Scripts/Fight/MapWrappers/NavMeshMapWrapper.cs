
using System;
using UnityEngine;
using UnityEngine.Events;



public class NavMeshMapWrapper : IMapWrapper
{
    public void LoadMapData(int mapId, Transform mapRoot = null, UnityAction onEnd = null)
    {
        throw new NotImplementedException();
    }

    public int MapHeight()
    {
        throw new NotImplementedException();
    }

    public int MapWidth()
    {
        throw new NotImplementedException();
    }

    public void OnNavLateUpdate(CharactorEntity e, float dt)
    {
        throw new NotImplementedException();
    }

    public void OnNavUpdate(CharactorEntity e, float dt)
    {
        throw new NotImplementedException();
    }

    public void OnNavUpdate(CharactorEntity2D e, float dt)
    {
        throw new NotImplementedException();
    }

    public void SyncEntityAlphaWithMap(CharactorEntity2D e)
    {
        throw new NotImplementedException();
    }

    public void WalkInDir(CharactorEntity e, float speed, int dirx, int diry)
    {
        throw new NotImplementedException();
    }

    public void WalkToDst(CharactorEntity e, float speed, float x, float y, float z)
    {
        throw new NotImplementedException();
    }

    public void WalkToDst(CharactorEntity2D e, float speed, float x, float z)
    {
        throw new NotImplementedException();
    }
}

