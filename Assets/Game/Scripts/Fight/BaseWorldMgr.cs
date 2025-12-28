using Game.Datas.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWorldMgr : MonoBehaviour
{
    protected int selfWorldId = -1; // 自己玩家在世界里面的Id; 


    protected int mapId = -1;
    
    protected Dictionary<int, CharactorEntity> charactors = null;

    protected IMapWrapper mapWrapper = null;

    protected Camera gameCamera = null;

    public virtual async void Init(int mapId/*object config*/) {
        this.selfWorldId = -1;
        this.charactors = new Dictionary<int, CharactorEntity>();

        this.mapId = mapId;

        if (mapId == 20001)
        {
            this.mapWrapper = new RectMapWrapper();
            this.mapWrapper.LoadMapData(mapId);
            this.gameCamera = this.transform.GetComponentInChildren<Camera>();
        }
        else if (mapId == 20003) {
            RvoNavSystem.ClearRVO();
            RvoNavSystem.InitRVO();
            this.mapWrapper = new RVOMapWrapper();
            this.mapWrapper.LoadMapData(mapId, this.transform, () => {
                this.gameCamera = this.transform.GetComponentInChildren<Camera>();
            });
        }

       
    }

    protected void OnEnterGameWorld(int status)
    {
        if (status != (int)Respones.OK)
        {
            return;
        }

        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerSpawn, null);
    }

    protected void OnEnterGameWorldIndep(ResEnterRoom res)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerSpawn, null);
    }

    protected void OnPlayerSpawnAtMap(ResPlayerSpawn res)
    {
        if (res.status != (int)Respones.OK)
        {
            return;
        }

        this.selfWorldId = res.worldId;
    }

    protected async void CreateCharactorInWorld(ArrivedCharactor ch) {

        CharactorEntity e = await EntityWrapper.Create(ch, this.transform.Find("CharactorRoot"));
        // EntityWrapper.SetEntityStatus(e, (int)CharactorStatus.Idle); 
        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)ch.status);

        this.charactors.Add(e.worldId, e);

        if (this.selfWorldId == ch.worldId){ // 是否为自己

        }
        // end
    }

    protected void OnSyncCharactorStatus(ResSyncCharactorStatus res) {
        if (!this.charactors.ContainsKey(res.worldId)) {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        CharactorEntity e = this.charactors[res.worldId];
        if (e == null)  {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }


        e.uTransform.eulerAngles.x = res.transform.eulerAngles[0];
        e.uTransform.eulerAngles.y = res.transform.eulerAngles[1];
        e.uTransform.eulerAngles.z = res.transform.eulerAngles[2];

        e.uTransform.pos.x = res.transform.pos[0];
        e.uTransform.pos.y = res.transform.pos[1];
        e.uTransform.pos.z = res.transform.pos[2];

        Debug.Log(e.uTransform.pos);
        if (e.uRvo.agentId != -1) {
            RvoNavSystem.SetAgentPosition(e.uRvo.agentId, e.uTransform.pos);
            if (res.statusData.status != (int)CharactorStatus.Run) {
                RvoNavSystem.SetAgentPrefVelocity(e.uRvo.agentId, Vector3.zero);
            }
        }
  
        EntityWrapper.SyncToUnityTransform(e);
        EntityWrapper.SetEntityStatus(ref e.uStatus, res.statusData.status);

    }

    protected void OnPlayerNavWithDir(ResNavInDir res) {
        
        if (!this.charactors.ContainsKey(res.worldId))
        {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        CharactorEntity e = this.charactors[res.worldId];
        if (e == null)
        {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        if (this.mapWrapper != null) {
            this.mapWrapper.WalkInDir(e, res.speed, res.dirx, res.diry);
        }
    }

    protected void OnPlayerNavToDst(ResNavToDst res) {
        if (!this.charactors.ContainsKey(res.worldId)) {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        CharactorEntity e = this.charactors[res.worldId];
        if (e == null) {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        if (this.mapWrapper != null) {
            this.mapWrapper.WalkToDst(e, res.speed, res.x, res.y, res.z);
        }
        
    }

    private void RemoveCharactorInWorld(int worldId) {
        if (!this.charactors.ContainsKey(worldId)) {
            return;
        }

        CharactorEntity e = this.charactors[worldId];
        // 删除掉RVO对应的agent
        if (e.uRvo.agentId != -1) {
            RvoNavSystem.DestroyAgent(e.uRvo.agentId);
            e.uRvo.agentId = -1;
        }

        GameObject.Destroy(e.uAnim.unityObject);
        e.uAnim.unityObject = null;

        this.charactors.Remove(worldId);
    }

    protected void OnCharactorEnterAOI(ResEnterAOI res) {
        for (int i = 0; i < res.charactors.Length; i++) {
            this.CreateCharactorInWorld(res.charactors[i]);
        }
    }

    protected void OnCharactorLeaveAOI(ResLeaveAOI res)
    {
        for (int i = 0; i < res.leavePlayers.Length; i++) {
            this.RemoveCharactorInWorld(res.leavePlayers[i]);
        }
    }

    private void NavSystemUpdate(float dt) {
        foreach (var e in this.charactors.Values)
        {
            if (e.uStatus.status != (int)CharactorStatus.Run || this.mapWrapper == null)
            {
                continue;
            }

            this.mapWrapper.OnNavUpdate(e, dt);
        }
    }

    private void NavSystemLateUpdate(float dt)
    {
        foreach (var e in this.charactors.Values)
        {
            if (e.uStatus.status != (int)CharactorStatus.Run || this.mapWrapper == null)
            {
                continue;
            }

            this.mapWrapper.OnNavLateUpdate(e, dt);
        }
    }

    private void CharactorAnimStatusUpdate()
    {
        foreach (var e in this.charactors.Values)
        {
            EntityWrapper.SyncEntityAnimStatus(e);
        }
    }

    public virtual void Update() {
        this.NavSystemUpdate(Time.deltaTime);
        RvoNavSystem.Update(Time.deltaTime);
        this.NavSystemLateUpdate(Time.deltaTime);

#if UNITY_EDITOR
        RvoNavSystem.RVODebugDrawMapEdge();
#endif
        this.CharactorAnimStatusUpdate();
    }
}
