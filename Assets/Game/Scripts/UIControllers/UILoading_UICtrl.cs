using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILoading_UICtrl : UICtrl {

	private Image bar;
	private Text ProgressTxt = null;
	private float per = 0.0f;
	private float updateSpeed = 1.0f / 2.0f; // 2秒跑完
	private float nextPer = 0.0f;
	private int sceneId = 0;
	// private float nowTime = 0.0f; 

	void Awake() {
		this.bar = this.View<Image>("ProgressBar/Bar");
		this.ProgressTxt = this.View<Text>("ProgressBar/ProgressTxt");

		this.bar.fillAmount = 0.0f;
		this.per = 0.0f;
		this.ProgressTxt.text = ((int)(this.per * 100)) + "%";

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIProgress);
	}

	public void SetNextScene(int sceneId) {
		this.sceneId = sceneId;
	}


	private void OnDestroy()
    {
		EventMgr.Instance.RemoveAllListeners(this);
		// EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIProgress);
    }

    private void OnUIProgress(int eventType, object udata, object param)
	{
		switch (udata) {
			case (int)GMEvent.UIProgress:
			{
				this.nextPer = (float)param;
				if (this.nextPer <= this.per)
				{
					this.nextPer = this.per;
					// this.nowTime = 0;
					return;
				}

				// this.nowTime = (this.nextPer - this.per) / this.updateSpeed;
			}
			break;
		}
		
	}

	public void Update()
	{
		if (this.per >= this.nextPer)
		{
			return;
		}

		float dt = Time.deltaTime;

		this.per += (this.updateSpeed * dt);
		// Debug.Log(this.per);

		this.per = (this.per >= this.nextPer) ? this.nextPer : this.per;
		this.ProgressTxt.text = ((int)(this.per * 100)) + "%";
		this.bar.fillAmount = this.per;

		if (this.per >= 1.0)
		{  // 加载结束了
			TimerMgr.Instance.ScheduleOnce((object udata) =>
			{
				EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UILoadingEnd, this.sceneId);
			}, 0.1f);

		}
	}
}
