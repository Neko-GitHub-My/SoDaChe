using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UIRegisterUser_UICtrl : UICtrl {

	void Start() {
		this.AddButtonListener("RegBtn", this.OnRegisterUserClick);
		this.AddButtonListener("Mask", this.OnCloseSelf);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIShowStatusInfo);
	}

	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIRegisterUser");
	}

	private void OnUIShowStatusInfo(int eventType, object udata, object param) {

		int subType = (int)udata;
		if (subType != (int)GMEvent.UIShowRegUserInfo) {
			return;
		}

		int status = (int)param;
		if (status == (int)Respones.OK)
		{  // 成功注册;
			Debug.Log("Register Success###");
			UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIRegisterUser");
			return;
		}
		else if (status == (int)Respones.UnameIsExist)
		{
			Debug.Log("UnameIsExist ###");
			this.View<Text>("errorDesic").text = "用户名已存在";
			return;
		}
		else {
			Debug.Log("SystemError ###");
			this.View<Text>("errorDesic").text = "系统错误";
			return;
		}
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIShowStatusInfo);
	}

	private void OnRegisterUserClick() {
		string uname = this.View<InputField>("UnameInput").text;
		string upwd = this.View<InputField>("UpwdInput").text;
		string upwdAgain = this.View<InputField>("UpwdInputAgain").text;

		if (uname.Length <= 5 || upwd.Length <= 0) {
			this.View<Text>("errorDesic").text = "输入正确的用户名or密码";
			return;
		}
		if (!upwd.Equals(upwdAgain)) {
			this.View<Text>("errorDesic").text = "两次密码不一致";
			return;
		}

		ReqRegisterUser req = new ReqRegisterUser();
		req.channal = 0;
		req.uname = uname;
		req.upwd = upwd;

		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIRegisterUser, req);
	}

}
