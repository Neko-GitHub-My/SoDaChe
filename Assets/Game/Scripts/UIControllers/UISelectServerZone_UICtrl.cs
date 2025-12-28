using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;
using System;

public class UISelectServerZone_UICtrl : UICtrl {

	private void ShowServerZoneInfoList(ServerZoneInfo item, GameObject itemPrefab) {
        GameObject itemNode = GameObject.Instantiate(itemPrefab);
        string socketType = (item.socketType == 0) ? "Tcp" : "Ws";

        itemNode.transform.Find("MsgBody").GetComponent<Text>().text = item.serverName + $"\n{socketType}:{item.serverIp}:{item.port}";

        itemNode.transform.SetParent(this.View<ScrollRect>("ServerZoneList").content, false);

        itemNode.GetComponentInChildren<Button>().onClick.AddListener(async () => {
            this.ViewNode("Waiting").SetActive(true);
            await ServerZoneMgr.Instance.SetConnectServerZone(item);
            if (GM_DataMgr.Instance.useDirectConn == 1) {
                this.ViewNode("Waiting").SetActive(false);
                TimerMgr.Instance.ScheduleOnce((object udata) => {
                    GameApp.Instance.EnterLoginScene();
                }, 0.1f);
            }
            
        });
    }

    async void Start() {
        EventMgr.Instance.AddListener(SocketMgr.NetConnectedEvent, this.OnNetEventProcesser);
		List<ServerZoneInfo> servers = ServerZoneMgr.Instance.GetServerZones();
		if (servers == null || servers.Count <= 0) {
			return;
		}

        GameObject itemPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("GUI/Prefabs/ServerZoneItem");
        for (int i = 0; i < servers.Count; i++) {
			ServerZoneInfo item = servers[i];
			this.ShowServerZoneInfoList(item, itemPrefab);

        }
	}

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveAllListeners(this);
        // EventMgr.Instance.RemoveListener(SocketMgr.NetConnectedEvent, this.OnNetEventProcesser);
    }

    private void OnNetEventProcesser(int eventType, object udata, object param)
    {
        switch (eventType) {
            case (int)SocketMgr.NetConnectedEvent:
                // 这个地方不用定时器，会反复的进行连接Socket和创建,这个Bug要跟进一下
                TimerMgr.Instance.ScheduleOnce((object udata) => {
                    GameApp.Instance.EnterLoginScene();
                }, 0.1f);
                break;
        }
    }
}
