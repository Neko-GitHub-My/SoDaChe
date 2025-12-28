using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UIPlayerBonues_UICtrl : UICtrl
{
	private GameObject curOptItem = null;
	private BonuesItem curOptBonuesDataItem = null;


	void Start()
	{
		this.ViewNode("Waiting").SetActive(true);

		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIPullingBonuesData);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);

		this.AddButtonListener("Mask", this.OnCloseSelf);
	}

	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIPlayerBonues");
	}

	private void OnDestroy() {
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}


	private void AddItemCanRecv(BonuesItem item, GameObject itemPrefab) {
		GameObject itemNode = GameObject.Instantiate(itemPrefab);
		itemNode.transform.Find("BonuesDesic").GetComponent<Text>().text = item.bonuesDesic;

		itemNode.transform.SetParent(this.View<ScrollRect>("BonuesList").content, false);

		itemNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
			if (item.status != 0) {
				return;
			}

			this.curOptItem = itemNode;
			this.curOptBonuesDataItem = item;
			EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIRecvBonues, item.bonuesId);
		});
	}

	private void AddItemCannotRecv(BonuesItem item, GameObject itemPrefab) {
		GameObject itemNode = GameObject.Instantiate(itemPrefab);
		itemNode.transform.Find("BonuesDesic").GetComponent<Text>().text = item.bonuesDesic;
		itemNode.transform.Find("RecvBtn/src").GetComponent<Text>().text = "已领取";

		itemNode.transform.SetParent(this.View<ScrollRect>("BonuesList").content, false);

	}

	private void OnFlushBonuesListItem(ResRecvBonues res) {
		if (res.status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"错误状态码:{res.status}";
			return;
		}
		else {
			this.curOptBonuesDataItem.status = 1;
			
			this.curOptItem.transform.Find("RecvBtn/src").GetComponent<Text>().text = "已领取";
		}
	}

	private void ShowBonuesDataList(GameObject itemPrefab) {
		
		for (int i = 0; i < GM_DataMgr.Instance.bonuesArray.Length; i++) {
			if (GM_DataMgr.Instance.bonuesArray[i].status == 0) {
				this.AddItemCanRecv(GM_DataMgr.Instance.bonuesArray[i], itemPrefab);
			}
			else {
				this.AddItemCannotRecv(GM_DataMgr.Instance.bonuesArray[i], itemPrefab);
			}
		}
	}

	private async void OnShowBonuesListInfo(int status) {
		GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/BonuesItem");
		
		this.ViewNode("Waiting").SetActive(false);

		if (status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"错误状态码:{status}";
		}
		else {
			if (GM_DataMgr.Instance.bonuesArray == null ||
				GM_DataMgr.Instance.bonuesArray.Length <= 0) {
				this.ViewNode("errorDesic").SetActive(true);
				this.View<Text>("errorDesic").text = "没有可领取的奖励!";
			}
			else {
				this.ShowBonuesDataList(itemPrefab);
			}
		}
	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowBonuesList:
				this.OnShowBonuesListInfo((int)param);
				break;
			case (int)GMEvent.UIFlushBonuesList:
				this.OnFlushBonuesListItem((ResRecvBonues)param);
				break;
		}
	}
}