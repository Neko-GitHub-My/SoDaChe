using Game.Datas.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RoomFightMgr : MonoBehaviour
{
    public static RoomFightMgr Instance = null;

    public int selfSeatId = -1;
    public int selfInViewId = -1;

    private int roomState = (int)RoomState.Invalid;

    protected Dictionary<int, CharactorEntity> charactors = null;


    public void Awake()
    {
        RoomFightMgr.Instance = this;    
    }


    public void Init(/*object config*/)
    {

        this.charactors = new Dictionary<int, CharactorEntity>();

        this.selfSeatId = -1;
        this.selfInViewId = -1;
        
        

        EventMgr.Instance.AddListener((int)(EventType.Logic), this.OnLogicEventProcess);
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.EnterLogicServer, null);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.Logic), this.OnLogicEventProcess);
    }

 

    private void OnEnterRoom(ResEnterRoom res) {

       

        Debug.Log($"玩家旁观ID: {res.roomInViewId }进入房间了!");
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

    private void OnOtherPlayerExitSeat(int seatId) {

        this.RemoveCharactorInSeat(seatId);
        Debug.Log($"玩家座位号: {seatId}离开座位了!");
    }

    private async Task OnOtherPlayerArrivedSeat(ResUserArrivedSeat res) {

        string assetUrl = (res.seatId == 0) ? "Charactors/Prefabs/10002/lhs/10002" : "Charactors/Prefabs/10002/rhs/10002";

        CharactorEntity e = await EntityWrapper.Create(res.unick,
                                                       res.usex,
                                                       res.seatId, assetUrl, this.transform.Find("CharactorRoot"));


        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)e.uStatus.status);
        if (res.seatId == 0) {
            e.uTransform.pos = new Vector3(-5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            e.uTransform.pos = new Vector3(5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 180, 0);
        }
        EntityWrapper.SyncToUnityTransform(e);

        this.charactors.Add(res.seatId, e);


        Debug.Log($"玩家座位号: {res.seatId}坐下了!");
    }

    

    private async Task OnSelfPlayerSitdownAt(int seatId) {
        Debug.Log($"玩家自己座位号: {seatId}坐下了!");

        this.selfSeatId = seatId;

        string assetUrl = (seatId == 0) ? "Charactors/Prefabs/10002/lhs/10002" : "Charactors/Prefabs/10002/rhs/10002";
        CharactorEntity e = await EntityWrapper.Create(GM_DataMgr.Instance.playerData.unick,
                                                            GM_DataMgr.Instance.playerData.usex, 
                                                            seatId, assetUrl, this.transform.Find("CharactorRoot"));

        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Idle);
        if (seatId == 0)
        {
            e.uTransform.pos = new Vector3(-5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            e.uTransform.pos = new Vector3(5, 0, 0);
            e.uTransform.eulerAngles = new Vector3(0, 180, 0);
        }
        
        EntityWrapper.SyncToUnityTransform(e);

        this.charactors.Add(seatId, e);
        
    }

    private void OnPlayerStandup(int status) {
        if (status != (int)Respones.OK) {
            // this.ShowTipInfo($"eror status: {status}");
            return;
        }

        

        Debug.Log($"玩家自己座位号: {this.selfSeatId}站起了!");
    }

    private void OnPlayerIsReady(ResPlayerIsReady res) {
        if (res.status != (int)Respones.OK) {
            // this.ShowTipInfo($"找逻辑服实例返回 status: {(int)res.status}");
            return;
        }

        Debug.Log($"在座位号:{res.seatId}的玩家准备好了!");
    }

    private void OnRoundIsReady(ResRoundReady res) {
        this.roomState = (int)RoomState.Ready;
        Debug.Log($"游戏马上要开始了，不能离开了，本局时长为:{res.roundTime}!");
    }

    private void OnRoundIsStarted(ResRoundStarted res)
    {
        Debug.Log($"游戏正式开始了!");
        this.roomState = (int)RoomState.Started;
    }

    private void OnRoundCheckOut(ResRoundCheckOut res)
    {
        this.roomState = (int)RoomState.CheckOut;
        Debug.Log($"游戏结束，Winner 为{res.reserve}");
    }

    private void OnRoundClear(ResRoundClear res)
    {
        this.roomState = (int)RoomState.RoundClear;
        Debug.Log($"游戏结束，进入本局清理状态，可以开始下一局!");
    }

    private void OnReconnRoomGame(ResReconnRoom res) {
       
    }

    private void OnPlayerOptAction(ResPlayerOpt opt) {
        if (opt.status != (int)Respones.OK) {
            // this.ShowTipInfo($"玩家操作出错: [{opt.status}]");
            return;
        }

        // do some thing
        Debug.Log($"[玩家座位:{opt.seatId}]操作动作:[{opt.optType}]");
    }

    private void OnOtherPlayerEscape(ResPlayerEscape res) {
        this.OnOtherPlayerExitSeat(res.seatId);
        Debug.Log($"[玩家座位:{res.seatId}]逃跑了!");
    }

    private void OnPlayerStartBuff(ResStartBuff res) {
        Debug.Log($"[玩家座位:{res.seatOrWorldId}]放了一个Buff");
        if (!this.charactors.ContainsKey(res.seatOrWorldId)) {
            Debug.LogWarning($"[玩家座位:{res.seatOrWorldId}]不存在");
            return;
        }

        CharactorEntity e = this.charactors[res.seatOrWorldId];
        e.uSkillAndBuff.buffTimeLine.StartBuff(res.buffId);
    }

    private void OnPlayerStartSkill(ResStartSkill res) {
        Debug.Log($"[玩家座位:{res.seatOrWorldId}]释放了一个技能");
        if (!this.charactors.ContainsKey(res.seatOrWorldId)) {
            Debug.LogWarning($"[玩家座位:{res.seatOrWorldId}]不存在");
            return;
        }
        CharactorEntity e = this.charactors[res.seatOrWorldId];

        e.uSkillAndBuff.skillTimeLine.StartSkill(e, res.skillId, null);
    }

    private void SkillAndBuffSystemUpdate(float dt) {
        foreach (var e in this.charactors.Values) {
            SkillAndBuffSystem.Update(e, dt);
        }
    }

    private void SyncAnimUpdate()
    {
        foreach (var e in this.charactors.Values)
        {
            EntityWrapper.SyncEntityAnimStatus(e);
        }
    }

    public void Update()
    {
        if (this.roomState == (int)RoomState.Started) {
            this.SkillAndBuffSystemUpdate(Time.deltaTime);
            this.SyncAnimUpdate();
        }
        
    }

    private void OnSyncCharactorStatus(ResSyncCharactorStatus res) {
        CharactorEntity e = this.charactors[res.worldId];
        EntityWrapper.SetEntityStatus(ref e.uStatus, res.statusData.status);
    }

    private void RemoveRoom()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void OnLogicEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.EnterLogicServerReturn:
                // this.ShowTipInfo($"找逻辑服实例返回 status: {(int)param}");
                break;
            case (int)GMEvent.UserExitLogicServerReturn:
                this.RemoveRoom();
                // this.ShowTipInfo($"ExitLogicServerReturn status: {(int)param}");
                break;
            case (int)GMEvent.UserEnterRoomReturn:
                this.OnEnterRoom((ResEnterRoom)param);
                break;
            case (int)GMEvent.UserSitdownReturn:
                ResSitdown res = (ResSitdown) param;
                this.OnSelfPlayerSitdownAt(res.seatId);
                break;
            case (int)GMEvent.UserStandupReturn:
                this.OnPlayerStandup((int)param);
                break;
            case (int)GMEvent.UserArrivedSeatReturn:
                this.OnOtherPlayerArrivedSeat((ResUserArrivedSeat) param);
                break;
            case (int)GMEvent.UserExitSeatReturn:
                this.OnOtherPlayerExitSeat(((ResUserExitSeat) param).seatId);
                break;
            case (int)GMEvent.PlayerIsReadyReturn:
                this.OnPlayerIsReady((ResPlayerIsReady)param);
                break;
            case (int)GMEvent.RoundIsReadyReturn:
                this.OnRoundIsReady((ResRoundReady) param);
                break;
            case (int)GMEvent.RoundIsStartedReturn:
                this.OnRoundIsStarted((ResRoundStarted)param);
                break;
            case (int)GMEvent.RoundCheckOutReturn:
                this.OnRoundCheckOut((ResRoundCheckOut) param);
                break;
            case (int)GMEvent.RoundClearReturn:
                this.OnRoundClear((ResRoundClear) param);
                break;
            case (int)GMEvent.ReconnRoomReturn:
                this.OnReconnRoomGame((ResReconnRoom) param);
                break;
            case (int)GMEvent.PlayerOptReturn:
                this.OnPlayerOptAction((ResPlayerOpt) param);
                break;
            case (int)GMEvent.PlayerEscapeReturn:
                this.OnOtherPlayerEscape((ResPlayerEscape) param);
                break;
            case (int)GMEvent.StartSkillReturn:
                this.OnPlayerStartSkill((ResStartSkill) param);
                break;
            case (int)GMEvent.StartBuffReturn:
                this.OnPlayerStartBuff((ResStartBuff)param);
                break;
            case (int)GMEvent.SyncCharactorStatusReturn:
                this.OnSyncCharactorStatus((ResSyncCharactorStatus)param);
                break;
            
        }
    }
}
