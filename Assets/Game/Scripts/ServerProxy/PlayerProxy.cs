using Framework.Core.Serializer;
using Framework.Core.Utils;
using Game.Datas.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ResponesProcesser]
public class PlayerProxy 
{
    public static PlayerProxy Instance = null;
    public bool useHttp = false;

    public PlayerProxy() {
        if (PlayerProxy.Instance == null) {
            PlayerProxy.Instance = this;
        }
        else {
            Debug.LogWarning("Player Proxy has multiple Instance !!!");
        }
    }

    private void SendMsg(Message msg, short machineId)
    {
        if (this.useHttp)
        {
            HttpServerNet.Instance.SendMsg(msg, machineId);
        }
        else
        {
            SocketMgr.Instance.SendMsg(msg, machineId);
        }
    }

    public void DoReqPullingPlayerDataAction(long openId = 0)
    {
        ReqPullingPlayerData req = new ReqPullingPlayerData();
        req.job = 0; // 任意职业;
        req.openId = openId; // openId;

        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPullingPlayerDataReturn(ResPullingPlayerData respones) {
        Debug.Log("DoResPullingPlayerDataAction:" + respones.status);

        if (respones.status == (int)Respones.OK) { // 直接成功的进入游戏，加载其它数据;
            Debug.Log("Load Player Data Success!!!!");


            GM_DataMgr.Instance.playerData = respones.pInfo;
            if (respones.isReConnectGame == 1) { // 走断线重连的流程
                //GameApp.Instance.EnterLoadingScene((int)SceneId.StdRoomScene);
            }
            else {
                GameApp.Instance.EnterLoadingScene((int)SceneId.HomeScene);
            }
            return;
        }

        if (respones.status == (int)Respones.PlayerIsNotExist) { // 进入游戏选角的流程
            //GameApp.Instance.EnterSelectScene();
            return;
        }
    }

    public void DoReqSelectPlayerAction(ReqSelectPlayer req)
    {
        // 提交请求到服务器
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResSelectPlayerReturn(ResSelectPlayer respones)
    {
        Debug.Log("DoResSelectPlayerAction:" + respones.status);

        if (respones.status == (int)Respones.OK) 
        {
            Debug.Log("Select Player Success!!!");
            GM_DataMgr.Instance.playerData = respones.pInfo;

            if (respones.isReConnectGame == 1) { // 走断线重连的流程
                //GameApp.Instance.EnterLoadingScene((int)SceneId.StdRoomScene);
            }
            else {
                GameApp.Instance.EnterLoadingScene((int)SceneId.HomeScene);
            }
            
            // GameApp.Instance.EnterHomeScene();
        }
    }

    public void DoReqRecvLoginBonuesAction()
    {
        ReqRecvLoginBonues req = new ReqRecvLoginBonues();
        req.type = 0;

        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResRecvLoginBonuesReturn(ResRecvLoginBonues respones) {
        GM_DataMgr.Instance.playerData.ucion += respones.num;
        GM_DataMgr.Instance.playerData.hasBonues = 0;
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowLoginBonuesInfo, respones.status);

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UISyncPlayerInfo, null);
    }

    public void DoReqPullingBonuesListAction() {
        ReqPullingBonuesList req = new ReqPullingBonuesList();

        req.typeId = -1; // 拉取所有的奖励
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPullingBonuesListReturn(ResPullingBonuesList respones) {

        if (respones.status == (int)Respones.OK) {
            GM_DataMgr.Instance.bonuesArray = respones.bonues;
        }

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowBonuesList, respones.status);
    }

    public void DoReqRecvBonuesAction(long bonuesId) {
        ReqRecvBonues req = new ReqRecvBonues();
        req.bonuesId = bonuesId;
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResRecvBonuesReturn(ResRecvBonues respones) {
        if (respones.status == (int)Respones.OK) {

            GM_BonuesMgr.Instance.ApplayBonuesToPlayer(respones);

            // 通知主界面刷新一下
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UISyncPlayerInfo, null);
        }
        
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIFlushBonuesList, respones);
    }

    public void DoReqPullingTaskListAction()
    {
        ReqPullingTaskList req = new ReqPullingTaskList();

        req.typeId = -1; // 拉取所有的任务
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPullingTaskListReturn(ResPullingTaskList respones) {
        if (respones.status == (int)Respones.OK) {
            GM_DataMgr.Instance.tasksArray = respones.tasks;
        }

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowTaskList, respones);
    }

    public void DoReqTestGoodsAction(ReqTestGetGoods req) {
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResTestGetGoodsReturn(ResTestGetGoods respones) {
        Debug.Log(respones.status);
    }

    public void DoReqPullingMailMsgAction()
    {
        ReqPullingMailMsg req = new ReqPullingMailMsg();
        req.typeId = -1;

        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPullingMailMsgReturn(ResPullingMailMsg respones)
    {
        Debug.Log(respones.status);
        if (respones.status == (int)Respones.OK) {
            GM_DataMgr.Instance.mailMsgArray = respones.mailMessages;
        }
        else {
            GM_DataMgr.Instance.mailMsgArray = null;
        }

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowMailMsgList, respones.status);
    }

    public void DoReqUpdateMailMsgAction(long mailMsgId)
    {
        ReqUpdateMailMsg req = new ReqUpdateMailMsg();
        req.mailMsgId = mailMsgId;
        req.status = 1; // 标记已读;

        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResUpdateMailMsgReturn(ResUpdateMailMsg respones)
    {
        if (respones.status == (int)Respones.OK)
        {
        }

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIFlushMailMsgList, respones);
    }

    public void DoReqPullingRankAction() {
        ReqPullingRank req = new ReqPullingRank();
        req.typeId = (int)RankType.WorldCoin; // 表示要拉取所有的排行榜数据;
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPullingRankReturn(ResPullingRank respones) {
        if (respones.status == (int)Respones.OK)
        {
        }

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowRankList, respones);
    }

    public void DoReqPullingPackDataAction() {
        ReqPullingPackData req = new ReqPullingPackData();
        req.typeId = -1;

        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResPullingPackDataReturn(ResPullingPackData respones) {
        if (respones.status == (int)Respones.OK) {
        }

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowBackpackList, respones);
    }

    public void DoReqTestUpdateGooodsAction(string text)
    {
        string[] items = text.Split('=');
        if (items.Length != 2) {
            return;
        }

        ReqTestUpdateGooods req = new ReqTestUpdateGooods();
        req.typeId = int.Parse(items[0]);
        req.num = int.Parse(items[1]);
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResTestUpdateGoodsReturn(ResTestUpdateGoods respones)
    {
        Debug.Log($"DoResTestUpdateGoodsReturn status: {respones.status}");
    }


    public void DoReqExchangeProductAction(int productId)
    {
        ReqExchangeProduct req = new ReqExchangeProduct();
        req.productId = productId;

        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResExchangeProductReturn(ResExchangeProduct respones) {
        Debug.Log($"DoResExchangeProductReturn status: {respones.status}");
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowExchangeProduct, respones.status);
    }

    public void DoReqEnterLogicServer()
    {
        ReqEnterLogicServer req = new ReqEnterLogicServer();
        req.typeId = GM_DataMgr.Instance.stype;
        req.zoneId = GM_DataMgr.Instance.zid;
        req.instId = -1; // 表示由服务器自动分配;
        req.openId = GM_DataMgr.Instance.OpenId;
        // this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
        SocketMgr.Instance.SendMsg(req, (short)GM_DataMgr.Instance.machineId, 
            GM_DataMgr.Instance.OpenId);
    }

    [ResponesMapping]
    public void DoResEnterLogicServerReturn(ResEnterLogicServer respones)
    {
        Debug.Log($"DoResEnterLogicServerReturn status: {respones.status}");
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.EnterLogicServerReturn, respones.status);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.EnterLogicServerReturn, respones.status);
    }
}
