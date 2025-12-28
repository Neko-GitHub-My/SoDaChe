using Framework.Core.Utils;
using Game.Datas.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 各种不同的逻辑服务通用的一些命令处理，
 * 我们统一使用Scene模块
 */

[ResponesProcesser]
public class SceneProxy
{
    public static SceneProxy Instance = null;


    public SceneProxy()
    {
        if (SceneProxy.Instance == null)
        {
            SceneProxy.Instance = this;
        }
        else
        {
            Debug.LogWarning("SceneProxy Proxy has multiple Instance !!!");
        }
    }

    public void DoReqTestLogicCmdEchoAction()
    {
        ReqTestLogicCmdEcho req = new ReqTestLogicCmdEcho();
        req.content = "Hello World Unity!";

        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResTestLogicCmdEchoReturn(ResTestLogicCmdEcho respones)
    {
        Debug.Log($"DoResTestLogicCmdEchoReturn status: {respones.content}");
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.ShowLogicEchoInfo, respones.content);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.ShowLogicEchoInfo, respones.content);
    }

    public void DoReqExitLogicServerAction(int quitReason)
    {
        ReqExitLogicServer req = new ReqExitLogicServer();
        req.quitReason = quitReason;

        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResExitLogicServerReturn(ResExitLogicServer respones)
    {
        Debug.Log($"DoResExitLogicServerReturn status: {respones.status}");
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserExitLogicServerReturn, respones.status);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserExitLogicServerReturn, respones.status);

        if (respones.status == (int)Respones.OK) {
            GameApp.Instance.EnterHomeScene();
            return;
        }
    }


    public void DoReqSitdownAction(int seatId)
    {
        // seatId = -1 ===>自动找一个作为;
        ReqSitdown req = new ReqSitdown();
        req.seatId = seatId;

        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResSitdownReturn(ResSitdown respones) {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserSitdownReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserSitdownReturn, respones);
    }

    public void DoReqTalkMsgAction(ReqTalkMsg req)
    {
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResTalkMsgReturn(ResTalkMsg respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserTalkMsgReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserTalkMsgReturn, respones);
    }

    public void DoReqStandupAction()
    {
        // seatId = -1 ===>自动找一个作为;
        ReqStandup req = new ReqStandup();
        req.reserve = -1;

        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResStandupReturn(ResStandup respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserStandupReturn, respones.status);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserStandupReturn, respones.status);
    }

    public void DoReqPlayerIsReadyAction()
    {
        ReqPlayerIsReady req = new ReqPlayerIsReady();
        req.reserve = -1;

        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPlayerIsReadyReturn(ResPlayerIsReady respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerIsReadyReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerIsReadyReturn, respones);
    }

    public void DoReqPlayerOptAction(ReqPlayerOpt req)
    {
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPlayerOptReturn(ResPlayerOpt respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerOptReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerOptReturn, respones);
    }

    [ResponesMapping]
    public void DoResFrameSyncDataReturn(ResFrameSyncData respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.FrameSyncDataReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.FrameSyncDataReturn, respones);
    }

    public void DoReqPlayerSpawnAction()
    {
        ReqPlayerSpawn req = new ReqPlayerSpawn();
        req.SpawnPoint = -1; // 让服务器自己去决定，放哪个出生点;

        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPlayerSpawnReturn(ResPlayerSpawn respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerSpawnReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerSpawnReturn, respones);
    }

    public void DoReqNavInDirAction(int dirx, int diry) {
        ReqNavInDir req = new ReqNavInDir();
        req.dirx = dirx;
        req.diry = diry;
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResNavInDirReturn(ResNavInDir respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.NavInDirReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.NavInDirReturn, respones);
    }

    public void DoReqNavToDstAction(ReqNavToDst req)
    {
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResNavToDstReturn(ResNavToDst respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.NavToDstReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.NavToDstReturn, respones);
    }

    public void DoReqStartSkillAction(int skillId)
    {
        ReqStartSkill req = new ReqStartSkill();
        req.skillId = skillId;
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResStartSkillReturn(ResStartSkill respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.StartSkillReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.StartSkillReturn, respones);
    }

    public void DoReqStartBuffAction(int buffId)
    {
        ReqStartBuff req = new ReqStartBuff();
        req.buffId = buffId;
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResStartBuffReturn(ResStartBuff respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.StartBuffReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.StartBuffReturn, respones);
    }

    [ResponesMapping]
    public void DoResEnterRoomReturn(ResEnterRoom respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserEnterRoomReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserEnterRoomReturn, respones);
    }

    [ResponesMapping]
    public void DoResUserArrivedSeatReturn(ResUserArrivedSeat respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserArrivedSeatReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserArrivedSeatReturn, respones);
    }

    [ResponesMapping]
    public void DoResUserExitSeatReturn(ResUserExitSeat respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserExitSeatReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserExitSeatReturn, respones);
    }

    [ResponesMapping]
    public void DoResRoundReadyReturn(ResRoundReady respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.RoundIsReadyReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.RoundIsReadyReturn, respones);
    }

    [ResponesMapping]
    public void DoResRoundStartedReturn(ResRoundStarted respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.RoundIsStartedReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.RoundIsStartedReturn, respones);
    }

    [ResponesMapping]
    public void DoResRoundStartedReturn(ResRoundCheckOut respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.RoundCheckOutReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.RoundCheckOutReturn, respones);
    }

    [ResponesMapping]
    public void DoResRoundStartedReturn(ResRoundClear respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.RoundClearReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.RoundClearReturn, respones);
    }

    [ResponesMapping]
    public void DoResReconnRoomReturn(ResReconnRoom respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.ReconnRoomReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.ReconnRoomReturn, respones);
    }

    [ResponesMapping]
    public void DoResPlayerEscapeReturn(ResPlayerEscape respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerEscapeReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerEscapeReturn, respones);
    }

    [ResponesMapping]
    public void DoResEnterAOIReturn(ResEnterAOI respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerEnterAOIReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerEnterAOIReturn, respones);
    }

    [ResponesMapping]
    public void DoResLeaveAOIReturn(ResLeaveAOI respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.PlayerLeaveAOIReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerLeaveAOIReturn, respones);
    }

    [ResponesMapping]
    public void DoSyncCharactorStatusReturn(ResSyncCharactorStatus respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.SyncCharactorStatusReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.SyncCharactorStatusReturn, respones);
    }

    [ResponesMapping]
    public void DoLostHpReturn(ResLostHp respones)
    {
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.LostHpReturn, respones);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.LostHpReturn, respones);
    }

}
