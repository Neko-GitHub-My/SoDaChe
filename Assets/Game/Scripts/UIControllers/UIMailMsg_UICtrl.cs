using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UIMailMsg_UICtrl : UICtrl {

	private GameObject curMsgItem = null;
	private MailMsgItem curOptMsgItem = null;

	void Start() {
		this.ViewNode("Waiting").SetActive(true);
		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIPullingMailMsg);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
		this.AddButtonListener("Mask", this.OnCloseSelf);

	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIMailMsg");
	}

	private void AddItemWithUnreaded(MailMsgItem item, GameObject itemPrefab)
	{
		GameObject itemNode = GameObject.Instantiate(itemPrefab);
		itemNode.transform.Find("MsgBody").GetComponent<Text>().text = item.msgBody;

		itemNode.transform.SetParent(this.View<ScrollRect>("MailMsgList").content, false);

		itemNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
			if (item.status != 0) {
				return;
			}

			this.curMsgItem = itemNode;
			this.curOptMsgItem = item;

			EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIUpdateMailMsgStatus, item.msgId);
		});
	}

	private void AddItemWithReaded(MailMsgItem item, GameObject itemPrefab)
	{
		GameObject itemNode = GameObject.Instantiate(itemPrefab);
		itemNode.transform.Find("BonuesDesic").GetComponent<Text>().text = item.msgBody;
		itemNode.transform.Find("RecvBtn/src").GetComponent<Text>().text = "ÒÑ¶Á";

		itemNode.transform.SetParent(this.View<ScrollRect>("MailMsgList").content, false);

	}

	private void ShowMailMsgList(GameObject itemPrefab)
	{

		for (int i = 0; i < GM_DataMgr.Instance.mailMsgArray.Length; i++)
		{
			if (GM_DataMgr.Instance.mailMsgArray[i].status == 0) // Î´¶Á
			{
				this.AddItemWithUnreaded(GM_DataMgr.Instance.mailMsgArray[i], itemPrefab);
			}
			else if(GM_DataMgr.Instance.mailMsgArray[i].status == 1) // ÒÑ¶Á
			{
				this.AddItemWithReaded(GM_DataMgr.Instance.mailMsgArray[i], itemPrefab);
			}
		}
	}

	private async void OnShowMailMsgList(int status)
	{
		GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/MailMsgItem");

		this.ViewNode("Waiting").SetActive(false);

		if (status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"´íÎó×´Ì¬Âë:{status}";
		}
		else
		{
			if (GM_DataMgr.Instance.mailMsgArray == null ||
				GM_DataMgr.Instance.mailMsgArray.Length <= 0)
			{
				this.ViewNode("errorDesic").SetActive(true);
				this.View<Text>("errorDesic").text = "Ã»ÓÐ¿É¶ÁÈ¡µÄÓÊ¼þ!";
			}
			else
			{
				this.ShowMailMsgList(itemPrefab);
			}
		}
	}

	private void OnFlushMailMsgListItem(ResUpdateMailMsg res)
	{
		if (res.status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"´íÎó×´Ì¬Âë:{res.status}";
			return;
		}
		else
		{
			this.curOptMsgItem.status = 1;

            this.curMsgItem.transform.Find("MsgBtn/src").GetComponent<Text>().text = "ÒÑ¶Á";
        }
    }

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowMailMsgList:
				this.OnShowMailMsgList((int)param);
				break;
			case (int)GMEvent.UIFlushMailMsgList:
				this.OnFlushMailMsgListItem((ResUpdateMailMsg)param);
				break;
		}
	}


}
