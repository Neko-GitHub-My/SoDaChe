using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;
using UnityEngine.EventSystems;

public class UIWorld_UICtrl : UICtrl {

	void Start() {
        if (GM_DataMgr.Instance.zid == 200020)
        {
            this.ViewNode("JoyStick").gameObject.SetActive(false);
            this.View<Text>("Tital").gameObject.SetActive(true);
            this.View<Text>("Tital").text = $"µØÍ¼¸±±¾:{GM_DataMgr.Instance.zid / 10}";
        }
        else {
            this.ViewNode("Tital").gameObject.SetActive(false);
        }

        if (GM_DataMgr.Instance.zid == 20002) {
            this.ViewNode("JoyStick").gameObject.SetActive(false);
        }

        EventTrigger eventTrigger = this.gameObject.GetComponentInChildren<EventTrigger>();
        if (eventTrigger != null) {
            var entry = new EventTrigger.Entry();
            entry.callback.AddListener(this.OnEventMaskClick);
            eventTrigger.triggers.Add(entry);
        }

        this.AddButtonListener("Bottom/ExitTest", this.OnExitTestClick);
        this.AddButtonListener("Bottom/ForceQuitTest", this.OnForceExitTestClick);
        // this.ViewNode("TalkBox").SetActive(false);
        this.ViewNode("Waiting").SetActive(false);

        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void OnEventMaskClick(BaseEventData data) {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UITouchMap, data);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void OnForceExitTestClick()
    {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIExitLogicServer, (int)QuitReason.ForceQuit);
    }

    private void OnExitTestClick()
    {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIExitLogicServer, (int)QuitReason.PlayerQuit);
    }

    private void OnUIEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.UserExitLogicServerReturn:
                Debug.Log($"ExitLogicServerReturn status: {(int)param}");
                break;
            case (int)GMEvent.FrameSyncDataReturn:
                ResFrameSyncData res = param as ResFrameSyncData;
                this.View<Text>("Tital").gameObject.SetActive(true);
                this.View<Text>("Tital").text =  res.frameId.ToString();
                break;
        }
    }

}
