using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Game.Datas.Messages;
using Unity.VisualScripting;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class EventProcesser : Attribute
{
    public int eventType;

    public EventProcesser(int eventType)
    {
        this.eventType = eventType;
    }
}

public class GMEventProcesserCenter : Singleton<GMEventProcesserCenter>
{
    private Dictionary<int, MethodInfo> eventsMap = new Dictionary<int, MethodInfo>();
    public void Init()
    {

        Type type = typeof(GMEventProcesserCenter);
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        // Debug.Log($"GMEventProcesser Init #########{methods.Length}");

        foreach (MethodInfo method in methods)
        {
            EventProcesser eventAttribute = method.GetCustomAttribute<EventProcesser>();
            if (eventAttribute == null)
            {
                continue;
            }

            // Debug.Log($"#######{method.Name}");
            this.eventsMap.Add(eventAttribute.eventType, method);
        }

        EventMgr.Instance.AddListener((int)EventType.UI, this.HandleGMEvent);
        EventMgr.Instance.AddListener((int)EventType.Logic, this.HandleGMEvent);
    }

    public void HandleGMEvent(int eventType, object udata, object param)
    {

        int uiEventType = (int)udata;
        // Debug.Log($"HandleUIEvent ####{uiEventType}");

        if (this.eventsMap.ContainsKey(uiEventType))
        {
            object[] parameters = new object[] { eventType, udata, param };
            this.eventsMap[uiEventType].Invoke(this, parameters);
        }
    }

    [EventProcesser((int)GMEvent.UIGuestLogin)]
    public void OnUIGuestLogin(int eventType, object udata, object param)
    {
        AuthProxy.Instance.DoReqGuestLoginAction();
        // 使用Http做AuthServer就开启
        // HttpAuthProxy.Instance.DoReqGuestLoginAction();
    }

    [EventProcesser((int)GMEvent.UILoadingEnd)]
    public void OnUILoadingEnd(int eventType, object udata, object param)
    {
        int nextId = (int)param;
        if (nextId == (int)SceneId.HomeScene)
        {
            GameApp.Instance.EnterHomeScene();
        }
        else if (nextId == (int)SceneId.WorldScene)
        {
            // 进入游戏
            //GameApp.Instance.EnterStdRoomScene();
        }
    }

    [EventProcesser((int)GMEvent.UISelectRole)]
    public void OnUISelectPlayer(int eventType, object udata, object param)
    {
        ReqSelectPlayer req = (ReqSelectPlayer)param;
        PlayerProxy.Instance.DoReqSelectPlayerAction(req);
    }

    [EventProcesser((int)GMEvent.UIRegisterUser)]
    public void OnUIRegisterUser(int eventType, object udata, object param)
    {
        ReqRegisterUser req = (ReqRegisterUser)param;
        AuthProxy.Instance.DoReqRegisterUserAction(req);
    }

    [EventProcesser((int)GMEvent.UIUserLogin)]
    public void OnUIUserLogin(int eventType, object udata, object param)
    {
        ReqUserLogin req = (ReqUserLogin)param;
        AuthProxy.Instance.DoReqUserLoginAction(req);
    }

    [EventProcesser((int)GMEvent.UIGuestUpgrade)]
    public void OnUIGuestUpgrade(int eventType, object udata, object param)
    {
        ReqGuestUpgrade req = (ReqGuestUpgrade)param;
        AuthProxy.Instance.DoReqGuestUpgradeAction(req);
    }

    [EventProcesser((int)GMEvent.UIRecvLoginBonues)]
    public void OnUIRecvLoginBonues(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqRecvLoginBonuesAction();
    }

    [EventProcesser((int)GMEvent.UIPullingBonuesData)]
    public void OnUIPullingBonuesData(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqPullingBonuesListAction();
    }

    [EventProcesser((int)GMEvent.UIRecvBonues)]
    public void OnUIRecvBonues(int eventType, object udata, object param) {
        PlayerProxy.Instance.DoReqRecvBonuesAction((long)param);
    }

    [EventProcesser((int)GMEvent.UIPullingTaskData)]
    public void OnUIPullingTaskData(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqPullingTaskListAction();
    }

    [EventProcesser((int)GMEvent.UITestGetGoods)]
    public void OnUITestGetGoods(int eventType, object udata, object param) {
        PlayerProxy.Instance.DoReqTestGoodsAction((ReqTestGetGoods)param);
    }

    [EventProcesser((int)GMEvent.UIPullingMailMsg)]
    public void OnUIPullingMailMsg(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqPullingMailMsgAction();
    }

    [EventProcesser((int)GMEvent.UIUpdateMailMsgStatus)]
    public void OnUIUpdateMailMsgStatus(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqUpdateMailMsgAction((long)param);
    }

    [EventProcesser((int)GMEvent.UIPullingRank)]
    public void OnUIPullingRank(int eventType, object udata, object param) {
        PlayerProxy.Instance.DoReqPullingRankAction();
    }

    [EventProcesser((int)GMEvent.UIPullingPackData)]
    public void OnUIPullingPackData(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqPullingPackDataAction();
    }

    [EventProcesser((int)GMEvent.UITestUpdateGoods)]
    public void OnUITestUpdateGoods(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqTestUpdateGooodsAction((string)param);
    }

    [EventProcesser((int)GMEvent.UIExchangeProduct)]
    public void OnUIExchangeProduct(int eventType, object udata, object param)
    {
        PlayerProxy.Instance.DoReqExchangeProductAction((int)param);
    }

    [EventProcesser((int)GMEvent.UIEnterGameScene)]
    public void OnUIEnterGameScene(int eventType, object udata, object param)
    {
        int zid = (int)param;
        if (zid == 10001)
        {
            GM_DataMgr.Instance.SetMachineIdWithLogicEntry(1);
            GameApp.Instance.EnterLoadingScene((int)SceneId.WorldScene);
        }
    }

    [EventProcesser((int)GMEvent.EnterLogicServer)]
    public void OnEnterLogicServer(int eventType, object udata, object param) {
        PlayerProxy.Instance.DoReqEnterLogicServer();
    }

    [EventProcesser((int)GMEvent.UITestEchoLogicServer)]
    public void OnUITestEchoLogicServer(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqTestLogicCmdEchoAction();
    }

    [EventProcesser((int)GMEvent.UIExitLogicServer)]
    public void OnUIExitLogicServer(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqExitLogicServerAction((int)param);
    }

    [EventProcesser((int)GMEvent.UserSitdown)]
    public void OnUserSitdownAtSeat(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqSitdownAction((int)param);
    }

    [EventProcesser((int)GMEvent.UserStandup)]
    public void OnUserStandup(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqStandupAction();
    }

    [EventProcesser((int)GMEvent.UserTalkMsg)]
    public void OnUserTalkMsg(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqTalkMsgAction((ReqTalkMsg)param);
    }

    [EventProcesser((int)GMEvent.PlayerIsReady)]
    public void OnPlayerIsReady(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqPlayerIsReadyAction();
    }

    [EventProcesser((int)GMEvent.PlayerOpt)]
    public void OnDoPlayerOpt(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqPlayerOptAction((ReqPlayerOpt) param);
    }

    [EventProcesser((int)GMEvent.PlayerSpawn)]
    public void OnDoPlayerSpawn(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqPlayerSpawnAction();
    }

    [EventProcesser((int)GMEvent.NavToDst)]
    public void OnDoPlayerNavToDst(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqNavToDstAction((ReqNavToDst)param);
    }

    [EventProcesser((int)GMEvent.StartSkill)]
    public void OnDoPlayerStartSkill(int eventType, object udata, object param) {
        SceneProxy.Instance.DoReqStartSkillAction((int)param);
    }

    [EventProcesser((int)GMEvent.StartBuff)]
    public void OnDoPlayerStartBuff(int eventType, object udata, object param)
    {
        SceneProxy.Instance.DoReqStartBuffAction((int)param);
    }

    [EventProcesser((int)GMEvent.UIJoyStick)]
    public void OnUIDoPlayerJoyStick(int eventType, object udata, object param)
    {
        if (GM_DataMgr.Instance.zid == 10004) { // 帧同步的摇杆操作不发往服务端;
            return;
        }

        JoyStickDir dir = (JoyStickDir)param;
        int dirx = (int)(dir.x * (1 << 16));
        int diry = (int)(dir.y * (1 << 16));

        SceneProxy.Instance.DoReqNavInDirAction(dirx, diry);
    }
}
