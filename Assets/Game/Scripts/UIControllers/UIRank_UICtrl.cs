using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UIRank_UICtrl : UICtrl {

	void Start() {
		this.ViewNode("Waiting").SetActive(true);

		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIPullingRank);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
		this.AddButtonListener("Mask", this.OnCloseSelf);
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIRank");
	}

	private async void ShowRankList(ResPullingRank respones) {
		GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/RankItem");
		for (int i = 0; i < respones.ranks.Length; i++) {
			RankItem item = respones.ranks[i];
			GameObject itemNode = GameObject.Instantiate(itemPrefab);
			itemNode.transform.SetParent(this.View<ScrollRect>("RankList").content, false);

			itemNode.transform.Find("unick").GetComponent<Text>().text = item.unick;
			itemNode.transform.Find("value").GetComponent<Text>().text = item.value.ToString();
			itemNode.transform.Find("rankIcon/src").GetComponent<Text>().text = (i + 1).ToString();

			string ufaceString = $"GUI/Faces/Male/face{item.uface}";
			Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(ufaceString);

			Image image = itemNode.transform.Find("HeadBk/uface").GetComponent<Image>();
			image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);


		}
	}

	private void OnUIShowRankList(ResPullingRank respones) {
		
		
		this.ViewNode("Waiting").SetActive(false);
		if (respones.status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"错误状态码:{respones.status}";
			return;
		}


		if (respones.ranks == null || respones.ranks.Length <= 0)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = "没有排行榜数据!";
		}
		else
		{
			this.ShowRankList(respones);
		}
	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowRankList:
				this.OnUIShowRankList((ResPullingRank)param);
				break;
		}
	}
}
