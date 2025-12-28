using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UIGuestUpgrade_UICtrl : UICtrl {

	void Start() {
		this.View<Text>("errorDesic").text = "";
		this.View<InputField>("UnickInput").text = GM_DataMgr.Instance.account.unick;

		this.AddButtonListener("UgradeBtn", this.OnGuestUpgradeClick);
		this.AddButtonListener("Mask", this.OnCloseSelf);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnCloseSelf() {
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIGuestUpgrade");
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnShowGuestUpgradeInfo(int status) {
		this.ViewNode("Waiting").SetActive(false);

		if (status == (int)Respones.AccountIsNotLogin) {
			this.View<Text>("errorDesic").text = "游客账号没有登录";
			return;
		}

		if (status == (int)Respones.InvalidParams)
		{
			this.View<Text>("errorDesic").text = "账号升级参数错误";
			return;
		}

		if (status == (int)Respones.UnameIsExist)
		{
			this.View<Text>("errorDesic").text = "用户名已经存在";
			return;
		}

		if (status == (int)Respones.UserIsNotGuest)
		{
			this.View<Text>("errorDesic").text = "该账号不是游客账号";
			return;
		}

		this.View<Text>("errorDesic").text = $"未知错误: {status}";

	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{

		switch (udata)
		{
			case (int)GMEvent.UIShowGuestUpgradeInfo:
				this.OnShowGuestUpgradeInfo((int)param);
				break;
		}
	}

	private void OnGuestUpgradeClick()
	{
		this.View<Text>("errorDesic").text = "";

		string uname = this.View<InputField>("UnameInput").text;
		string upwd = this.View<InputField>("UpwdInput").text;
		string upwdAgain = this.View<InputField>("UpwdInputAgain").text;
		string unick = this.View<InputField>("UnickInput").text;

		if (uname.Length <= 5 || upwd.Length <= 0 || unick.Length <= 0)
		{
			this.View<Text>("errorDesic").text = "输入正确的用户名or密码or昵称";
			return;
		}
		if (!upwd.Equals(upwdAgain)) {
			this.View<Text>("errorDesic").text = "两次密码不一致";
			return;
		}


		this.ViewNode("Waiting").SetActive(true);
		ReqGuestUpgrade req = new ReqGuestUpgrade();
		
		req.uname = uname;
		req.upwd = upwd;
		req.unick = unick;

		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIGuestUpgrade, req);
	}

}
