using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UILogin_UICtrl : UICtrl {

	void Start() {
		this.ViewNode("Waiting").SetActive(false);
		this.AddButtonListener("GuestLoginBtn", this.OnGuestLoginClick);
		this.AddButtonListener("RegBtn", this.OnRegisterUserClick);
		this.AddButtonListener("LoginBtn", this.OnUserLoginClick);

        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
	}

    private void OnDestroy()
    {
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}



    private void OnShowUserLoginInfo(int status) {
		this.ViewNode("Waiting").SetActive(false);
		if (status == (int)Respones.AccountIsNotExist) {
			this.View<Text>("errorDesic").text = "用户不存在";
			return;
		}

		if (status == (int)Respones.InvalidParams) {
			this.View<Text>("errorDesic").text = "非法的参数";
			return;
		}

		if (status == (int)Respones.UnameOrUpwdError)
		{
			this.View<Text>("errorDesic").text = "用户名or密码错误";
			return;
		}

		this.View<Text>("errorDesic").text = $"未知错误: {status}";
	}

	private void OnUIEventProcess(int eventType, object udata, object param) {
		
		switch (udata)
		{
			case (int)GMEvent.UIShowUserLoginInfo:
				this.OnShowUserLoginInfo((int)param);
			break;

        }
	}

	private void OnGuestLoginClick() {
		this.View<Text>("errorDesic").text = "";
		this.ViewNode("Waiting").SetActive(true);

		TimerMgr.Instance.ScheduleOnce((object param) => {
			EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIGuestLogin);
		}, null, 0.5f);
	}

	private void OnUserLoginClick() {
		this.View<Text>("errorDesic").text = "";
		// 检查数据, 用户名与密码不能为null;
		string uname = this.View<InputField>("UnameInput").text;
		string upwd = this.View<InputField>("UpwdInput").text;
		if (uname == null || uname.Length <= 0 || upwd == null || upwd.Length <= 0) {
			this.View<Text>("errorDesic").text = "用户名or密码为空";
			return;
		}
		// end


		this.ViewNode("Waiting").SetActive(true);



		ReqUserLogin req = new ReqUserLogin();
		req.uname = uname;
		req.upwd = upwd;

		TimerMgr.Instance.ScheduleOnce((object param) => {
			EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIUserLogin, req);
		}, null, 0.5f);
	}

	private void OnRegisterUserClick() {
		this.View<Text>("errorDesic").text = "";
		UIMgr.Instance.ShowUIView("GUI/Prefabs/UIRegisterUser");
	}
}
