using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILoginBonues_UICtrl : UICtrl {

	void Start() {
		int bonues = GM_DataMgr.Instance.playerData.loginBonues; // 根据你配置文件的奖励的数目来就可以了，days ---> bonues;
		this.View<Text>("loginBonuesDesic").text = $"连续登录{GM_DataMgr.Instance.playerData.days}天,\n获取{bonues}";

		this.AddButtonListener("RecvBonues", this.OnRecvBonuesClick);
		this.AddButtonListener("Mask", this.OnCloseSelf);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowLoginBonuesInfo:
				int status = (int)param;
				this.OnSyncLoginBonuesInfo(status);
				break;
		}
	}

	private void OnSyncLoginBonuesInfo(int status) {
		if (status == (int)Respones.OK) {  // 播放奖励动画
			int bonues = GM_DataMgr.Instance.playerData.loginBonues; // 根据你配置文件的奖励的数目来就可以了，days ---> bonues;
			this.ViewNode("RecvBonues").SetActive(false);
			this.View<Text>("loginBonuesDesic").text = $"连续登录{GM_DataMgr.Instance.playerData.days}天,\n已得:{bonues}";
		}
		else {
			this.View<Text>("errorDesic").text = $"领取奖励错误号:{status}";
		}
	}

	private void OnCloseSelf() {
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UILoginBonues");
	}

	private void OnRecvBonuesClick() {
		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIRecvLoginBonues);
		Debug.Log("OnRecvBonuesClick ####");
	}
}
