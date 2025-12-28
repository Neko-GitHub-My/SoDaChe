using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Excels;

public class UITrading_UICtrl : UICtrl {

	async void Start() {
		this.AddButtonListener("Mask", this.OnCloseSelf);
		EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);

		List<ExchangeTrading> configs = ExcelDataMgr.Instance.GetConfigDatas<ExchangeTrading>("ExchangeTrading");
		if (configs == null || configs.Count <= 0) {
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = "没有背包数据数据!";
			return;
		}

		GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/TradingItem");

		for (int i = 0; i < configs.Count; i++) {
			ExchangeTrading config = configs[i];
			GameObject itemNode = GameObject.Instantiate(itemPrefab);
			itemNode.transform.SetParent(this.View<ScrollRect>("TradingList").content, false);

			itemNode.transform.Find("Desic").GetComponent<Text>().text = config.name;
			itemNode.transform.Find("Price").GetComponent<Text>().text = config.Price.ToString();
			
			string goodsIconPath = $"GUI/TradingIcons/{config.IconUrl}";
			Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(goodsIconPath);
			Image image = itemNode.transform.Find("icon/src").GetComponent<Image>();
			image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);

			itemNode.GetComponentInChildren<Button>().onClick.AddListener(() => {
				EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIExchangeProduct, config.ID);
			});
		}
	}

	private void OnDestroy()
	{
		EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
	}

	private void OnCloseSelf()
	{
		UIMgr.Instance.RemoveUIView("GUI/Prefabs/UITrading");
	}

	private void OnUIExchangeProduct(int status) {
		if (status != (int)Respones.OK)
		{
			this.ViewNode("errorDesic").SetActive(true);
			this.View<Text>("errorDesic").text = $"错误状态码:{status}";
			return;
		}

		this.ViewNode("errorDesic").SetActive(true);
		this.View<Text>("errorDesic").text = "兑换成功";
	}

	private void OnUIEventProcess(int eventType, object udata, object param)
	{
		switch (udata)
		{
			case (int)GMEvent.UIShowExchangeProduct:
				this.OnUIExchangeProduct((int)param);
				break;
		}
	}
}
