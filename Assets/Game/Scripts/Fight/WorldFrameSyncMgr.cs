using Game.Datas.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

enum FrameEventType
{
    JoySitck = 1 << 0,
    Attack = 1 << 1,
    Skill = 1 << 2,
    // ...
}

struct CacheJoyStickEvent {
    public bool isValid;
    public int dirx;
    public int diry;
}


public class WorldFrameSyncMgr : MonoBehaviour
{
    private int FRAME_TIME = 50; // 20FPS, 50ms间隔;
    public static WorldFrameSyncMgr Instance = null;
    protected Dictionary<int, CharactorEntity> charactors = null;
    private int selfSeatId = -1;

    private int syncedFrameId = -1;
    private ResFrameSyncData lastFrame = null;

    private CacheJoyStickEvent localJoystick;

    protected IMapWrapper mapWrapper = null;

    public void Awake() {
        WorldFrameSyncMgr.Instance = this;
    }

    public /*override*/ async void Init(int mapId/*object config*/) {
        // 创建地图对象
        if (mapId == 10004) {
            this.mapWrapper = new RectMapWrapper();
            this.mapWrapper.LoadMapData(mapId);
        }
        // end
        // 本地摇杆事件
        localJoystick.dirx = localJoystick.diry = 0;
        localJoystick.isValid = false;
        // end

        this.selfSeatId = -1;
        this.charactors = new Dictionary<int, CharactorEntity>();
        EventMgr.Instance.AddListener((int)(EventType.Logic), this.OnLogicEventProcess);
        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.RemoveListener((int)(EventType.Logic), this.OnLogicEventProcess);
    }

    private void RemoveWorld()
    {
        GameObject.Destroy(this.gameObject);
    }

    private async Task OnSelfPlayerSitdownAt(int seatId)
    {
        Debug.Log($"玩家自己座位号: {seatId}坐下了!");

        this.selfSeatId = seatId;

        string assetUrl = "Charactors/Prefabs/10004";
        CharactorEntity e = await EntityWrapper.Create(GM_DataMgr.Instance.playerData.unick,
                                                            GM_DataMgr.Instance.playerData.usex,
                                                            seatId, assetUrl, this.transform.Find("CharactorRoot"));

        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
        if (seatId == 0)
        {
            e.uTransform.pos = new Vector3(-5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            e.uTransform.pos = new Vector3(5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 180, 0);
        }

        e.uFrameSync.status = e.uStatus.status;
        e.uFrameSync.pos = e.uTransform.pos;
        e.uFrameSync.eulerAngles = e.uTransform.eulerAngles;

        EntityWrapper.SyncToUnityTransform(e);

        this.charactors.Add(seatId, e);

    }

    private async Task OnOtherPlayerArrivedSeat(ResUserArrivedSeat res)
    {
        string assetUrl = "Charactors/Prefabs/10004";

        CharactorEntity e = await EntityWrapper.Create(res.unick,
                                                       res.usex,
                                                       res.seatId, assetUrl, this.transform.Find("CharactorRoot"));


        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)e.uStatus.status);
        if (res.seatId == 0)
        {
            e.uTransform.pos = new Vector3(-5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            e.uTransform.pos = new Vector3(5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 180, 0);
        }

        e.uFrameSync.status = e.uStatus.status;
        e.uFrameSync.pos = e.uTransform.pos;
        e.uFrameSync.eulerAngles = e.uTransform.eulerAngles;

        EntityWrapper.SyncToUnityTransform(e);
        this.charactors.Add(res.seatId, e);


        Debug.Log($"玩家座位号: {res.seatId}坐下了!");
    }

    private void SyncFrameJoystickEvent(FrameOpt optEvent, CharactorEntity e)
    {
        if (optEvent.v1 == 0 && optEvent.v2 == 0) {  // 这一帧，角色要停止;
            e.uTransform.pos = e.uFrameSync.pos;
            e.uTransform.eulerAngles = e.uFrameSync.eulerAngles;
            EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
            e.uFrameSync.status = (int)CharactorStatus.Idle;
            e.uFrameSync.vx = 0;
            e.uFrameSync.vz = 0;
            return;
        }

        e.uFrameSync.status = (int)CharactorStatus.Run;
        e.uFrameSync.vx = (((float)(optEvent.v1)) / (1 << 16)) * e.uProps.speed;
        e.uFrameSync.vz = (((float)(optEvent.v2)) / (1 << 16)) * e.uProps.speed;
        e.uFrameSync.eulerAngles.y = (Mathf.Atan2(e.uFrameSync.vz, e.uFrameSync.vx) * 180) / Mathf.PI;
    }

    private void SyncLastFrameOptEvent(FrameOpt optEvent) {
        if (!this.charactors.ContainsKey(optEvent.seatId)) {
            return;
        }
        CharactorEntity e = this.charactors[optEvent.seatId];
        if (e == null) {
            return;
        }

        if ((optEvent.optType & (int)FrameEventType.JoySitck) != 0) { // 摇杆
            this.SyncFrameJoystickEvent(optEvent, e);
        }
        // 如果有其他事件，这里来进行处理;
    }

    private void ProcessFrameJoystickEvent(FrameOpt optEvent, CharactorEntity e)
    {
        this.mapWrapper.WalkInDir(e, e.uProps.speed, optEvent.v1, optEvent.v2);
    }


    private void ProcessFrameOptEvent(FrameOpt optEvent) {
        if (!this.charactors.ContainsKey(optEvent.seatId)) {
            return;
        }
        // 本地处理事件
        CharactorEntity e = this.charactors[optEvent.seatId];
        if (e == null) {
            return;
        }
        // end

        if ((optEvent.optType & (int)FrameEventType.JoySitck) != 0) { // 摇杆;
            this.ProcessFrameJoystickEvent(optEvent, e);
        }
        // 如果有其他事件，这里来进行处理;
        if ((optEvent.optType & (int)FrameEventType.Attack) != 0) { // 攻击
        }


    }

    private void OnFrameSyncUpdate(int dtms) {
        // 这里如果对精度有要求,可以采用定点数
        // 遍历所有的角色，计算出从|====| 经过50ms后的结果;
        // 迭代1: 移动的角色
        foreach (var v in this.charactors.Values) {
            if (v.uFrameSync.status == (int)CharactorStatus.Run) {
                v.uFrameSync.pos.x += v.uFrameSync.vx * dtms / 1000;
                v.uFrameSync.pos.z += v.uFrameSync.vz * dtms / 1000;

                // 优化:添加上条件，如果你本地的速度，方向，状态与 StatusComponet;
                v.uTransform.pos = v.uFrameSync.pos;
                v.uTransform.eulerAngles = v.uFrameSync.eulerAngles;

                v.uNav.totalTime = 10 * this.FRAME_TIME; // 移动本地的时间更新10个帧
                v.uNav.passedTime = 0;
            }
        }
        // end

        // 迭代2: 伤害计算,技能与Buff迭代

        // end
    }

    private void SendNextFrameOptToServer(int nextFrameId) {
        ReqPlayerOpt req = null;
        if (this.localJoystick.isValid)
        { // 摇杆

            this.localJoystick.isValid = false; // 只有事件改变了才上报,上报后事件就不再报了;
            if (req == null) {
                req = new ReqPlayerOpt();
                req.optType = 0;
                req.v3 = nextFrameId;
            } 
            
            req.optType |= (int)FrameEventType.JoySitck; // (1 << 0) | (1 << 2);
            req.v1 = this.localJoystick.dirx;
            req.v2 = this.localJoystick.diry;
            
            // req.v4 = skillId + buffId;
            // req.v5 = ;

            
        }
        // 发送其他的事件...  摇杆+攻击按钮;  摇杆+攻击
        // end

        // 可以考虑优化，把所有操作，一起打包一个命令发送;
        if (req != null) {
            EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerOpt, req);
        }
        
    }

    private void SyncLastFrame() {
        // 基于上一帧的状态，来处理上一帧的逻辑事件;
        if (this.lastFrame.playersOptSet != null)
        {
            for (int i = 0; i < this.lastFrame.playersOptSet.Length; i++)
            {
                // 处理游戏里面的每个事件
                this.SyncLastFrameOptEvent(this.lastFrame.playersOptSet[i]);
                // end
            }
        }


        // 基于帧同步的数据+时间间隔来同步+迭代, 推动战斗，移动等计算
        this.OnFrameSyncUpdate(this.FRAME_TIME); // 50毫秒来做时间间隔;
                                                 // end
    }

    private void OnServerFrameSyncDataUpdate(ResFrameSyncData frameData) {
        if (this.lastFrame != null) {  // 同步上一帧;
            this.SyncLastFrame();
        }

        if (this.syncedFrameId + 1 != frameData.frameId) { 
            Debug.LogError($"lost frame{this.syncedFrameId}-{frameData.frameId}");
            // return;
        }

        this.syncedFrameId = frameData.frameId;

        // 处理当前帧, 本地处理操作的持续时间是 10个帧;  预测;
        if (frameData.playersOptSet != null) {
            for (int i = 0; i < frameData.playersOptSet.Length; i++) {
                // 处理游戏里面的每个事件
                this.ProcessFrameOptEvent(frameData.playersOptSet[i]);
                // end
            }
        }
        this.lastFrame = frameData;
        // end

        // 上报我们下一帧的操作
        this.SendNextFrameOptToServer(this.syncedFrameId + 1);
    }

    private void OnUIEventProcess(int eventType, object udata, object param) {
        switch (udata) {
            case (int)GMEvent.UIJoyStick:
                JoyStickDir dir = (JoyStickDir)param;
                this.localJoystick.isValid = true;
                this.localJoystick.dirx = (int)(dir.x * (1 << 16));
                this.localJoystick.diry = (int)(dir.y * (1 << 16));
                break;
        }
    }

    private void RemoveCharactorInSeat(int seatId)
    {
        if (!this.charactors.ContainsKey(seatId))
        {
            return;
        }

        CharactorEntity e = this.charactors[seatId];
        GameObject.Destroy(e.uAnim.unityObject);
        e.uAnim.unityObject = null;

        this.charactors.Remove(seatId);
    }

    private void OnPlayerStandup(int status)
    {
        if (status != (int)Respones.OK)
        {
            // this.ShowTipInfo($"eror status: {status}");
            return;
        }

        Debug.Log($"玩家自己座位号: {this.selfSeatId}站起了!");
    }

    private void OnOtherPlayerExitSeat(int seatId)
    {
        this.RemoveCharactorInSeat(seatId);
        Debug.Log($"玩家座位号: {seatId}离开座位了!");
    }

    private async void OnLogicEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.EnterLogicServerReturn:
                break;
            case (int)GMEvent.ShowLogicEchoInfo:
                break;
            case (int)GMEvent.UserExitLogicServerReturn:
                int status = (int)param;
                if (status == (int)Respones.OK) {
                    this.RemoveWorld();
                }
                
                break;
            case (int)GMEvent.UserEnterRoomReturn:
                break;
            case (int)GMEvent.UserSitdownReturn:
                ResSitdown res = (ResSitdown)param;
                if (res.status != (int)Respones.OK) {
                    return;
                }
                await this.OnSelfPlayerSitdownAt(res.seatId);
                break;
            case (int)GMEvent.UserStandupReturn:
                this.OnPlayerStandup((int)param);
                break;
            case (int)GMEvent.UserArrivedSeatReturn:
                await this.OnOtherPlayerArrivedSeat((ResUserArrivedSeat)param);
                break;
            case (int)GMEvent.UserExitSeatReturn:
                this.OnOtherPlayerExitSeat(((ResUserExitSeat)param).seatId);
                break;
            case (int)GMEvent.PlayerIsReadyReturn:
                break;
            case (int)GMEvent.RoundIsReadyReturn:
                break;
            case (int)GMEvent.RoundIsStartedReturn:
                break;
            case (int)GMEvent.RoundCheckOutReturn:
                break;
            case (int)GMEvent.RoundClearReturn:
                break;
            case (int)GMEvent.ReconnRoomReturn:
                break;
            case (int)GMEvent.PlayerOptReturn:
                break;
            case (int)GMEvent.PlayerEscapeReturn:
                break;
            case (int)GMEvent.FrameSyncDataReturn: // 帧事件处理
                this.OnServerFrameSyncDataUpdate((ResFrameSyncData) param);
                break;
        }
    }

    private void NavSystemUpdate(float dt)
    {
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

    public virtual void Update()
    {
        this.NavSystemUpdate(Time.deltaTime);
        this.NavSystemLateUpdate(Time.deltaTime);
        this.CharactorAnimStatusUpdate();
    }
}
