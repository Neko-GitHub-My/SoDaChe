using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIHome_UICtrl : UICtrl {

	async void Start() {

		if (GM_DataMgr.Instance.account.isGuest == 0)
		{
			this.ViewNode("Upgrade").SetActive(false);
		}
		else {
			this.ViewNode("Upgrade").SetActive(true);
		}

		// 同步我们的昵称
		var unick = GM_DataMgr.Instance.playerData.unick;
		// var unick = GM_DataMgr.Instance.account.unick;
		unick = (unick.Length > 5) ? (unick.Substring(0, 5) + "...") : unick;
		this.View<Text>("UserInfo/unick").text = unick;

		int usex = GM_DataMgr.Instance.playerData.usex;
		string usexStr = (usex == 0) ? "男" : "女";
		this.View<Text>("UserInfo/usex").text = usexStr;


		string ucoinStr = $"金币:{GM_DataMgr.Instance.playerData.ucion}";
		this.View<Text>("UserInfo/uchip").text = ucoinStr;

		int uface = GM_DataMgr.Instance.account.uface;
		string ufaceString = $"GUI/Faces/Male/face{uface}";
		Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(ufaceString);
		
		Image image = this.View<Image>("UserInfo/HeadBk/uface");
		image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);

		this.AddButtonListener("Upgrade", this.OnGuestUpgradeClick);

		if (GM_DataMgr.Instance.playerData.hasBonues == 1) {
			UIMgr.Instance.ShowUIView("GUI/Prefabs/UILoginBonues");
		}

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);

		this.AddButtonListener("UIBottom/reward", this.OnPullingPlayerBonuesClick);
		this.AddButtonListener("UIBottom/task", this.OnPullingPlayerTaskClick);
		this.AddButtonListener("UIBottom/email", this.OnPullingMailMsgClick);
		this.AddButtonListener("UIBottom/rank", this.OnPullingRankClick);
		this.AddButtonListener("UIBottom/bpack", this.OnPullingBackpackClick);
		this.AddButtonListener("UIBottom/mall", this.OnPullingTradingClick);

        this.AddButtonListener("Scene1", () => { this.OnEnterSceneClick((int)ServerType.RoomWithRule, 10001); });
        this.AddButtonListener("Scene2", () => { this.OnEnterSceneClick((int)ServerType.OpenWithMapWorld, 20001); });
        this.AddButtonListener("Scene3", () => { this.OnEnterSceneClick((int)ServerType.OpenWithMapWorld, 20002); });
        this.AddButtonListener("Scene5", () => { this.OnEnterSceneClick((int)ServerType.RoomWithRule, 10002); });
        this.AddButtonListener("Scene6", () => { this.OnEnterSceneClick((int)ServerType.IndepOrRoomWithMapWorld, 200020); }); 
        this.AddButtonListener("Scene7", () => { this.OnEnterSceneClick((int)ServerType.IndepOrRoomWithMapWorld, 200030); });
        this.AddButtonListener("Scene4", () => { this.OnEnterSceneClick((int)ServerType.OpenWithMapWorld, 20003); }); // 开放RVO   
        this.AddButtonListener("Scene8", () => { this.OnEnterSceneClick((int)ServerType.RoomWithRule, 10004); });    
    }

	private void OnEnterSceneClick(int stype, int zid) {
		GM_DataMgr.Instance.stype = stype;
		GM_DataMgr.Instance.zid = zid;
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIEnterGameScene, zid);
    }


    private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnPullingTradingClick() {
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UITrading");
	}


	private void OnPullingBackpackClick() {
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UIBackpack");
	}

	private void OnPullingPlayerBonuesClick() {
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UIPlayerBonues");
	}

	private void OnPullingRankClick() {
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UIRank");
	}

	private void OnPullingMailMsgClick()
	{
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UIMailMsg");
	}

	private void OnPullingPlayerTaskClick()
	{
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UITask");
	}

	private void OnSyncPlayerInfo() {
		string ucoinStr = $"金币:{GM_DataMgr.Instance.playerData.ucion}";
		this.View<Text>("UserInfo/uchip").text = ucoinStr;
	}

	private async void OnSyncAccountInfo() {
		int uface = GM_DataMgr.Instance.account.uface;
		string ufaceString = $"GUI/Faces/Male/face{uface}";
		Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(ufaceString);

		Image image = this.View<Image>("UserInfo/HeadBk/uface");
		image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);

		if (GM_DataMgr.Instance.account.isGuest == 0) {
			this.ViewNode("Upgrade").SetActive(false);
		}
		else {
			this.ViewNode("Upgrade").SetActive(true);
		}
	}

	private void OnUIEventProcess(int eventType, object udata, object param) {
		switch (udata)
		{
			case (int)GMEvent.UISyncAccountInfo:
				this.OnSyncAccountInfo();
				break;
			case (int)GMEvent.UISyncPlayerInfo:
				this.OnSyncPlayerInfo();
				break;
		}
	}

	private void OnGuestUpgradeClick() {
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UIGuestUpgrade");
	}
}
