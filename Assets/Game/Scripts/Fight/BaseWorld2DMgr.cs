using Game.Datas.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseWorld2DMgr : MonoBehaviour
{
    protected int selfWorldId = -1; // 自己玩家在世界里面的Id; 


    protected int mapId = -1;
    
    protected Dictionary<int, CharactorEntity2D> charactors = null;

    protected Camera gameCamera = null;

    protected GameCamera2D cameraCtrl = null;

    protected IMapWrapper mapWrapper = null;

    private float uiScreenWidth = 0;
    private float uiScreenHeight = 0;

    private Transform entityRoot = null;
    public virtual void Init(int mapId/*object config*/) {
        this.selfWorldId = -1;
        this.charactors = new Dictionary<int, CharactorEntity2D>();

        this.mapId = mapId;

        if (this.mapId == 20002) {  // 2D开放世界;
            this.mapWrapper = new AStarMapWrapper();
            this.mapWrapper.LoadMapData(this.mapId);
        }
        this.gameCamera = GameObject.Find("MapCanvas/MapCamera").GetComponent<Camera>();
        this.cameraCtrl = this.gameCamera.AddComponent<GameCamera2D>();

        RectTransform r = GameObject.Find("MapCanvas").transform as RectTransform;
        this.uiScreenWidth = r.sizeDelta.x;
        this.uiScreenHeight = r.sizeDelta.y;

        this.entityRoot = this.transform.Find("Layer/EntityLayer");
        this.entityRoot.gameObject.AddComponent<EntityLayer>();
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

    private async void CreateCharactorInWorld(ArrivedCharactor ch) {
   
        CharactorEntity2D e = await EntityWrapper.Create2D(ch, this.entityRoot, (this.selfWorldId == ch.worldId));
        EntityWrapper.SetEntityStatus(ref e.uStatus, ch.status); 

        this.charactors.Add(e.worldId, e);

        if (this.selfWorldId == ch.worldId){ // 是否为自己
            // 同步一下摄像机的位置，让摄像机对准玩家;
            Vector3 pos =e.uAnim.unityObject.transform.localPosition;
            // end

            // 架设好摄像机，让它对准玩家的出生点
            this.cameraCtrl.ResetCamera(pos, this.transform as RectTransform,
                                        uiScreenWidth, uiScreenHeight,
                                        this.mapWrapper.MapWidth(), this.mapWrapper.MapHeight());

            // 让摄像机，跟随我们的玩家
            if (this.mapWrapper.MapWidth() >= this.uiScreenWidth || this.mapWrapper.MapHeight() >= this.uiScreenHeight)
            {
                this.cameraCtrl.BindFollowTarget(e);
            }
            else
            {
                this.cameraCtrl.BindFollowTarget(null);
            }
            // end
        }
        // end
    }

    protected void OnSyncCharactorStatus(ResSyncCharactorStatus res) {
        if (!this.charactors.ContainsKey(res.worldId)) {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        CharactorEntity2D e = this.charactors[res.worldId];
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

        // Debug.Log(e.uTransform.pos);

        EntityWrapper.SyncToUnityTransform2D(e);
        EntityWrapper.SetEntityStatus(ref e.uStatus, res.statusData.status);

    }

    protected void OnPlayerNavToDst(ResNavToDst res) {
        if (!this.charactors.ContainsKey(res.worldId)) {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        CharactorEntity2D e = this.charactors[res.worldId];
        if (e == null) {
            Debug.LogWarning($"unknow Player: {res.worldId}");
            return;
        }

        this.mapWrapper.WalkToDst(e, res.speed, res.x, res.z);

        
    }

    private void RemoveCharactorInWorld(int worldId) {
        if (!this.charactors.ContainsKey(worldId)) {
            return;
        }

        CharactorEntity2D e = this.charactors[worldId];
        GameObject.Destroy(e.uAnim.unityObject);
        e.uAnim.unityObject = null;

        this.charactors.Remove(worldId);
    }

    protected void OnCharactorEnterAOI(ResEnterAOI res) {
        for (int i = 0; i < res.charactors.Length; i++) {
            Debug.Log("CreateCharactorInWorld ###");
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
            if (e.uStatus.status != (int)CharactorStatus.Run)
            {
                continue;
            }

            this.mapWrapper.OnNavUpdate(e, dt);
        }
    }

    private void CharactorAnimStatusUpdate() {
        foreach (var e in this.charactors.Values)
        {
            EntityWrapper.SyncEntityAnimStatus(e);
            this.mapWrapper.SyncEntityAlphaWithMap(e);
        }
    }

    public virtual void Update() {
        this.NavSystemUpdate(Time.deltaTime);
        this.CharactorAnimStatusUpdate();
    }
}
