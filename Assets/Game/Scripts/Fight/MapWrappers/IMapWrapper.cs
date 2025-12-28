using UnityEngine;
using UnityEngine.Events;

public interface IMapWrapper
{
    public void LoadMapData(int mapId, Transform mapRoot = null, UnityAction onEnd = null);

    public int MapWidth();

    public int MapHeight();

    public void WalkToDst(CharactorEntity e, float speed, float x, float y, float z);

    public void WalkInDir(CharactorEntity e, float speed, int dirx, int diry);


    public void OnNavUpdate(CharactorEntity e, float dt);

    public void OnNavLateUpdate(CharactorEntity e, float dt);

    public void WalkToDst(CharactorEntity2D e, float speed, float x, float z);

    public void OnNavUpdate(CharactorEntity2D e, float dt);

    public void SyncEntityAlphaWithMap(CharactorEntity2D e);


    // ....
}

