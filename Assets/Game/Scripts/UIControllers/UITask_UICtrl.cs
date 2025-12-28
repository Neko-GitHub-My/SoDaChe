using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UITask_UICtrl : UICtrl {

	void Start() {
		this.ViewNode("Waiting").SetActive(true);
		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIPullingTaskData);
		this.AddButtonListener("Mask", this.OnCloseSelf);
		this.AddButtonListener("CollectDamond", ()=>{
			ReqTestGetGoods req = new ReqTestGetGoods();
			req.num = 1;
			req.typeId = 1;
			EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UITestGetGoods, req);
		});
		this.AddButtonListener("CollectBook", ()=> {
			ReqTestGetGoods req = new ReqTestGetGoods();
			req.num = 1;
			req.typeId = 2;
			EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UITestGetGoods, req);
		});
		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UITask");
	}

	private void AddItemWithUnOpended(TaskItem item, GameObject itemPrefab)
	{
		GameObject itemNode = GameObject.Instantiate(itemPrefab);
		itemNode.transform.Find("TaskDesic").GetComponent<Text>().text = item.taskDesic;
		itemNode.transform.Find("TaskBtn/src").GetComponent<Text>().text = "未开启";

		itemNode.transform.SetParent(this.View<ScrollRect>("TaskList").content, false);

	}

	private void AddItemWithStarted(TaskItem item, GameObject itemPrefab)
	{
		GameObject itemNode = GameObject.Instantiate(itemPrefab);
		itemNode.transform.Find("TaskDesic").GetComponent<Text>().text = item.taskDesic;
		itemNode.transform.Find("TaskBtn/src").GetComponent<Text>().text = "进行中";

		itemNode.transform.SetParent(this.View<ScrollRect>("TaskList").content, false);

	}

	private async void ShowTaskDataList(TaskItem[] tasks) {
		if (tasks == null || tasks.Length <= 0) {
			return;
		}

		GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/TaskItem");
		for (int i = 0; i < tasks.Length; i++) {
			if (tasks[i].status == 0)
			{
				this.AddItemWithUnOpended(tasks[i], itemPrefab);
			}
			else if(tasks[i].status == 1)
			{
				this.AddItemWithStarted(tasks[i], itemPrefab);
			}
		}
	}

	private void OnShowTaskListInfo(ResPullingTaskList res) {
		this.ViewNode("Waiting").SetActive(false);
		if (res.status != (int)Respones.OK) {
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"错误状态码:{res.status}";
		}
		else if (res.tasks == null ||
				res.tasks.Length <= 0) {
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = "当前没有任务!";
		}
		else {
			this.ShowTaskDataList(res.tasks);
		}
	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowTaskList:
				this.OnShowTaskListInfo((ResPullingTaskList)param);
				break;
		}
	}
}
