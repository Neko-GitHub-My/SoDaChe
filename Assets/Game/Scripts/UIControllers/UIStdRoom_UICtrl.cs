using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;
using System.Security.Cryptography;

public class UIStdRoom_UICtrl : UICtrl {


    void Start() {
        this.AddButtonListener("Bottom/EchoTest", this.OnEchoTestClick);
        this.AddButtonListener("Bottom/ExitTest", this.OnExitTestClick);
        this.AddButtonListener("Bottom/StandupTest", this.OnExitStandupTestClick);
        this.AddButtonListener("Bottom/TalkTest", this.OnShowOrHideTalkMsgClick);
        this.AddButtonListener("TalkBox/SendBtn", this.OnTalkMsgSendClick);
        this.AddButtonListener("Bottom/ReadyTest", this.OnPlayerIsReadyClick);
        this.AddButtonListener("Bottom/OptTest", this.OnPlayerOptClick);
        this.AddButtonListener("Bottom/ForceQuitTest", this.OnPlayerForceQuitClick);

        this.ViewNode("TalkBox").SetActive(false);
        this.ViewNode("Waiting").SetActive(false);

        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void OnPlayerForceQuitClick() {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIExitLogicServer, (int)QuitReason.ForceQuit);
    }


    private void OnPlayerOptClick() {
        ReqPlayerOpt req = new ReqPlayerOpt();
        req.optType = 100001;
        req.v1 = 1;

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerOpt, req);
    }

    private void OnPlayerIsReadyClick() {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerIsReady, null);
    }

    private void OnShowOrHideTalkMsgClick() {
        bool ret = this.ViewNode("TalkBox").activeInHierarchy;
        this.ViewNode("TalkBox").SetActive(!ret);
    }

    private void OnTalkMsgSendClick()
    {
        var content = this.View<InputField>("TalkBox/Input").text;
        if (content.Length <= 0) {
            return;
        }

        ReqTalkMsg msg = new ReqTalkMsg();
        msg.talkType = 0;
        msg.msgBodyOrAudioUrl = content;

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserTalkMsg, msg);
    }

    private void OnEchoTestClick() {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UITestEchoLogicServer);
    }

    private void OnExitStandupTestClick() {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UserStandup, null);
    }

    private void OnExitTestClick()
    {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIExitLogicServer, (int)QuitReason.PlayerQuit);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private async void ShowTextTalkMsg(ResTalkMsg res) {
        GameObject itemPrefab = null;
        string content = null;
        if (RoomMgr.Instance.selfInViewId == res.roomInViewId)
        {
            content = $"我说: {res.msgBodyOrAudioUrl}";
            itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/SelfTalkMsg");
        }
        else {
            content = $"玩家{res.roomInViewId}说: {res.msgBodyOrAudioUrl}";
            itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/OtherTalkMsg");
        }

        GameObject itemNode = GameObject.Instantiate(itemPrefab);
        itemNode.transform.SetParent(this.View<ScrollRect>("TalkBox/ContentList").content, false);
        itemNode.transform.Find("MsgBody").GetComponent<Text>().text = content;
    }

    private void OnShowTalkMsg(ResTalkMsg res) {
        if (res.talkType == 0) { // 文本模式
            this.ShowTextTalkMsg(res);
        }
        else if(res.talkType == 1){ // 语言聊天
        }
    }

    private void OnUIEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.UserTalkMsgReturn:
                this.OnShowTalkMsg((ResTalkMsg) param);
                break;
        }
    }
}
