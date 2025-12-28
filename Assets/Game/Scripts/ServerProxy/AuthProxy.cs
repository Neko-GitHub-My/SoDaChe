using Framework.Core.Serializer;
using Framework.Core.Utils;
using Game.Datas.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ResponesProcesser]
public class AuthProxy
{
    public static AuthProxy Instance = null;

    public bool useHttp = false;

    public AuthProxy()
    {
        if (AuthProxy.Instance == null)
        {
            AuthProxy.Instance = this;
        }
        else
        {
            Debug.LogWarning("Auth Proxy has multiple Instance !!!");
        }
    }

    private void SendMsg(Message msg, short machineId) {
        if (this.useHttp)
        {
            HttpServerNet.Instance.SendMsg(msg, machineId);
        }
        else {
            SocketMgr.Instance.SendMsg(msg, machineId);
        }
    }


    public void DoReqGuestLoginAction()
    {
        // 处理游客登录
        var req = new ReqGuestLogin();
        // req.guestKey = "BYCW1234567890123456789";
        req.guestKey = GM_DataMgr.Instance.guestKey;
        if (req.guestKey == null || req.guestKey.Length <= 0)
        {
            req.guestKey = DataUtils.RandString(32);
            GM_DataMgr.Instance.SaveGuestKey(req.guestKey);
        }


        req.channal = (int)Channal.SelfChannal;
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
        // end
    }

    [ResponesMapping]
    public void DoReqGuestLoginReturn(ResGuestLogin respones)
    {
        Debug.Log("DoReqGuestLoginReturn:" + respones.status);

        if (respones.status == (int)Respones.OK)
        {
            GM_DataMgr.Instance.account = respones.uinfo;
            GM_DataMgr.Instance.OpenId = respones.accountIdOrOpenId;
            PlayerProxy.Instance.DoReqPullingPlayerDataAction(GM_DataMgr.Instance.OpenId);
        }
    }

    public void DoReqRegisterUserAction(ReqRegisterUser req)
    {
        req.channal = (int)Channal.SelfChannal;
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoReqRegisterUserReturn(ResRegisterUser respones)
    {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowRegUserInfo, respones.status);
    }

    public void DoReqUserLoginAction(ReqUserLogin req)
    {
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResUserLoginReturn(ResUserLogin respones)
    {
        Debug.Log($"DoResUserLoginReturn #### {respones.status}");

        if (respones.status != (int)Respones.OK) {
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowUserLoginInfo, respones.status);
            return;
        }
        else {
            GM_DataMgr.Instance.account = respones.uinfo;
            GM_DataMgr.Instance.OpenId = respones.accountIdOrOpenId;
            PlayerProxy.Instance.DoReqPullingPlayerDataAction(GM_DataMgr.Instance.OpenId);
        }                
    }

    public void DoReqGuestUpgradeAction(ReqGuestUpgrade req)
    {
        this.SendMsg(req, (short)GM_DataMgr.Instance.machineId);
    }

    [ResponesMapping]
    public void DoResGuestUpgradeReturn(ResGuestUpgrade respones)
    {
        if (respones.status != (int)Respones.OK)
        {
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIShowGuestUpgradeInfo, respones.status);
            return;
        }
        else {
            GM_DataMgr.Instance.account = respones.uinfo;
            UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIGuestUpgrade");
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UISyncAccountInfo, respones.uinfo);
        }
    }
}
