using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;

public class UIBackpack_UICtrl : UICtrl {

	void Start() {
		this.ViewNode("Waiting").SetActive(true);
		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIPullingPackData);

		this.AddButtonListener("Mask", this.OnCloseSelf);
		this.AddButtonListener("TestAddGoods", this.OnTestUpdateGoods);
		this.AddButtonListener("TestDecGoods", this.OnTestUpdateGoods);

		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnTestUpdateGoods() {
		string text = this.View<InputField>("TestGoodsIdInput").text;
		if (text == null || text.Length <= 0) {
			return;
		}

		// add: 10001=20  dec: 10001=-20
		EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UITestUpdateGoods, text);
	}


	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UIBackpack");
	}

	private async void ShowBackpackList(ResPullingPackData respones)
	{
		int count = 0;
		GameObject itemNode = null;

		GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/BackpackItem");
		foreach (var keyAndValue in respones.packGoods)
		{
			// List<GoodsItem> itemsPerType = keyAndValue.Value;
			GoodsItem[] itemsPerType = keyAndValue.Value;


            for (int i = 0; i < itemsPerType.Length; i++) {
				GoodsItem item = itemsPerType[i];
				int index = count % 4;
				if (index == 0) { // 4个了
					itemNode = GameObject.Instantiate(itemPrefab);
					itemNode.transform.SetParent(this.View<ScrollRect>("BackpackList").content, false);

					itemNode.transform.GetChild(0).gameObject.SetActive(false);
					itemNode.transform.GetChild(1).gameObject.SetActive(false);
					itemNode.transform.GetChild(2).gameObject.SetActive(false);
					itemNode.transform.GetChild(3).gameObject.SetActive(false);
				}

				Debug.Log(index);
				GameObject goodItemObject = itemNode.transform.GetChild(index).gameObject;
				goodItemObject.SetActive(true);

				goodItemObject.transform.Find("num").GetComponent<Text>().text = item.num.ToString();

				string goodsIconPath= $"GUI/GoodsIcons/{item.typeId}";
				Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(goodsIconPath);
				Image image = goodItemObject.transform.Find("icon").GetComponent<Image>();
				image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);

				count ++;
			}

		}
	}

	private void OnShowBackpackList(ResPullingPackData respones) {
		this.ViewNode("Waiting").SetActive(false);
		if (respones.status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"错误状态码:{respones.status}";
			return;
		}

		if (respones.packGoods == null || respones.packGoods.Length <= 0)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = "没有背包数据数据!";
			return;
		}

		this.ShowBackpackList(respones);
	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowBackpackList:
				this.OnShowBackpackList(param as ResPullingPackData);
				break;
		}
	}
}
