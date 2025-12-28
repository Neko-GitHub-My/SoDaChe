using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Datas.Messages;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class UITeamUp_UICtrl : UICtrl {

    public int selfSeatId = -1;
    public int selfInViewId = -1;

    void Start() {
        this.transform.Find("PlayerA").gameObject.SetActive(false);
        this.transform.Find("PlayerB").gameObject.SetActive(false);
        this.transform.Find("RoomInfo").gameObject.SetActive(false);
        // this.transform.Find("Waiting").gameObject.SetActive(false);
        this.transform.Find("TipInfo").gameObject.SetActive(false);

        this.transform.Find("Waiting").gameObject.SetActive(true);

        this.selfSeatId = -1;
        this.selfInViewId = -1;

        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.EnterLogicServer, null);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void ShowTipInfo(string content)
    {
        this.transform.Find("TipInfo").gameObject.SetActive(true);
        this.transform.Find("TipInfo").GetComponent<Text>().text = content;
    }

    private void RemoveRoom()
    {
        UIMgr.Instance.RemoveUIView("UITeamUp");
    }

    private void ShowRoomInfo(int roomId)
    {
        this.transform.Find("RoomInfo").gameObject.SetActive(true);
        this.transform.Find("RoomInfo").GetComponent<Text>().text = $"房间号:{roomId}";
    }

    private void OnEnterRoom(ResEnterRoom res)
    {

        this.transform.Find("Waiting").gameObject.SetActive(false);

        this.selfInViewId = res.roomInViewId;
        this.ShowRoomInfo(res.roomId);
        this.transform.Find("TipInfo").gameObject.SetActive(false);
        this.transform.Find("PlayerA").gameObject.SetActive(true);
        this.transform.Find("PlayerB").gameObject.SetActive(true);
        this.transform.Find("PlayerA/User").gameObject.SetActive(false);
        this.transform.Find("PlayerA/SitdownBtn").gameObject.SetActive(true);

        this.transform.Find("PlayerB/User").gameObject.SetActive(false);
        this.transform.Find("PlayerB/SitdownBtn").gameObject.SetActive(true);

        this.ShowTipInfo($"玩家旁观ID: {res.roomInViewId}进入房间了!");

        // 自动坐下;  
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.UserSitdown, -1);
    }

    private async Task OnOtherPlayerArrivedSeat(ResUserArrivedSeat res)
    {
        Transform userInfo = null;

        if (res.seatId == 0)
        {
            userInfo = this.transform.Find("PlayerA/User");
            this.transform.Find("PlayerA/User").gameObject.SetActive(true);
            this.transform.Find("PlayerA/SitdownBtn").gameObject.SetActive(false);

        }
        else if (res.seatId == 1)
        {
            userInfo = this.transform.Find("PlayerB/User");
            this.transform.Find("PlayerB/User").gameObject.SetActive(true);
            this.transform.Find("PlayerB/SitdownBtn").gameObject.SetActive(false);
        }

        // 同步作为的昵称，头像, 
        var unick = res.unick;
        // var unick = GM_DataMgr.Instance.account.unick;
        unick = (unick.Length > 5) ? (unick.Substring(0, 5) + "...") : unick;
        userInfo.Find("unick").GetComponent<Text>().text = unick;

        int usex = res.usex;
        string usexStr = (usex == 0) ? "男" : "女";
        userInfo.Find("usex").GetComponent<Text>().text = usexStr;

        int uface = res.uface;
        string ufaceString = $"GUI/Faces/Male/face{uface}";
        Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(ufaceString);

        Image image = userInfo.Find("HeadBk/uface").GetComponent<Image>();
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);

        userInfo.Find("uchip").gameObject.SetActive(false);

        this.ShowTipInfo($"玩家座位号: {res.seatId}坐下了!");

        // 帧同步，我们不要准备，自动就准备好了
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerIsReady, null);
    }

    private void OnOtherPlayerEscape(ResPlayerEscape res)
    {
        this.OnOtherPlayerExitSeat(res.seatId);
        this.ShowTipInfo($"[玩家座位:{res.seatId}]逃跑了!");
    }

    private void OnOtherPlayerExitSeat(int seatId)
    {
        if (seatId == 0)
        {
            this.transform.Find("PlayerA/User").gameObject.SetActive(false);
            this.transform.Find("PlayerA/SitdownBtn").gameObject.SetActive(true);
        }
        else if (seatId == 1)
        {
            this.transform.Find("PlayerB/User").gameObject.SetActive(false);
            this.transform.Find("PlayerB/SitdownBtn").gameObject.SetActive(true);
        }

        this.ShowTipInfo($"玩家座位号: {seatId}离开座位了!");
    }


    private async Task OnSelfPlayerSitdownAt(int seatId)
    {
        this.transform.Find("Waiting").gameObject.SetActive(false);

        this.selfSeatId = seatId;

        Transform userInfo = null;

        if (seatId == 0)
        {
            userInfo = this.transform.Find("PlayerA/User");
            this.transform.Find("PlayerA/User").gameObject.SetActive(true);
            this.transform.Find("PlayerA/SitdownBtn").gameObject.SetActive(false);

        }
        else if (seatId == 1)
        {
            userInfo = this.transform.Find("PlayerB/User");
            this.transform.Find("PlayerB/User").gameObject.SetActive(true);
            this.transform.Find("PlayerB/SitdownBtn").gameObject.SetActive(false);
        }

        // 同步作为的昵称，头像, 
        var unick = GM_DataMgr.Instance.playerData.unick;
        // var unick = GM_DataMgr.Instance.account.unick;
        unick = (unick.Length > 5) ? (unick.Substring(0, 5) + "...") : unick;
        userInfo.Find("unick").GetComponent<Text>().text = unick;

        int usex = GM_DataMgr.Instance.playerData.usex;
        string usexStr = (usex == 0) ? "男" : "女";
        userInfo.Find("usex").GetComponent<Text>().text = usexStr;

        int uface = GM_DataMgr.Instance.account.uface;
        string ufaceString = $"GUI/Faces/Male/face{uface}";
        Texture2D tex = await ResMgr.Instance.AwaitGetAsset<Texture2D>(ufaceString);

        Image image = userInfo.Find("HeadBk/uface").GetComponent<Image>();
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), image.sprite.pivot);

        userInfo.Find("uchip").gameObject.SetActive(false);

        this.ShowTipInfo($"玩家自己座位号: {seatId}坐下了!");

        // 自动准备好，无需要点击;
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.PlayerIsReady, null);
    }

    private void OnPlayerStandup(int status)
    {
        if (status != (int)Respones.OK)
        {
            this.ShowTipInfo($"eror status: {status}");
            return;
        }

        if (this.selfSeatId == 0)
        {
            this.transform.Find("PlayerA/User").gameObject.SetActive(false);
            this.transform.Find("PlayerA/SitdownBtn").gameObject.SetActive(true);
        }
        else if (this.selfSeatId == 1)
        {
            this.transform.Find("PlayerB/User").gameObject.SetActive(false);
            this.transform.Find("PlayerB/SitdownBtn").gameObject.SetActive(true);
        }

        this.ShowTipInfo($"玩家自己座位号: {this.selfSeatId}站起了!");
    }

    private void OnPLayerIsReady(ResPlayerIsReady res)
    {
        if (res.status != (int)Respones.OK)
        {
            this.ShowTipInfo($"找逻辑服实例返回 status: {(int)res.status}");
            return;
        }

        this.ShowTipInfo($"在座位号:{res.seatId}的玩家准备号了!");
    }

    private void OnRoundIsReady(ResRoundReady res)
    {
        this.ShowTipInfo($"游戏马上要开始了，不能离开了，本局时长为:{res.roundTime}!");
    }

    private void OnRoundIsStarted(ResRoundStarted res)
    {
        this.ShowTipInfo($"游戏正式开始了!");
        UIMgr.Instance.RemoveUIView("UITeamUp");
    }

    private void OnRoundCheckOut(ResRoundCheckOut res)
    {
        this.ShowTipInfo($"游戏结束，正在结算!");
    }

    private void OnRoundClear(ResRoundClear res)
    {
        this.ShowTipInfo($"游戏结束，进入本局清理状态，可以开始下一局!");
    }

    private async void OnReconnRoomGame(ResReconnRoom res)
    {

        // 进入房间
        this.OnEnterRoom(res.room);
        // end

        // 玩家自己数据与坐下
        await this.OnSelfPlayerSitdownAt(res.selfSitdown.seatId);
        // end

        // 其它玩家坐下的流程
        for (int i = 0; i < res.seats.Length; i++)
        {
            ResUserArrivedSeat arraivedData = res.seats[i];
            if (arraivedData == null)
            {
                continue;
            }

            await this.OnOtherPlayerArrivedSeat(arraivedData);
        }
        // end

        // 处理自己这个玩家所在的一些数据;
        // end
        // ...
        this.ShowTipInfo("玩家断线重连回到游戏!");
    }

    private void OnPlayerOptAction(ResPlayerOpt opt)
    {
        if (opt.status != (int)Respones.OK)
        {
            this.ShowTipInfo($"玩家操作出错: [{opt.status}]");
            return;
        }

        // do some thing
        this.ShowTipInfo($"[玩家座位:{opt.seatId}]操作动作:[{opt.optType}]");
    }

    private void OnUIEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.EnterLogicServerReturn:
                this.ShowTipInfo($"找逻辑服实例返回 status: {(int)param}");
                break;
            case (int)GMEvent.ShowLogicEchoInfo:
                this.ShowTipInfo((string)param);
                break;
            case (int)GMEvent.UserExitLogicServerReturn:
                this.ShowTipInfo($"ExitLogicServerReturn status: {(int)param}");
                this.RemoveRoom();
                break;
            case (int)GMEvent.UserEnterRoomReturn:
                this.OnEnterRoom((ResEnterRoom)param);
                break;
            case (int)GMEvent.UserSitdownReturn:
                ResSitdown res = (ResSitdown)param;
                if (res.status != (int)Respones.OK)
                {
                    this.transform.Find("Waiting").gameObject.SetActive(false);
                    this.ShowTipInfo($"玩家坐下出错: [{res.status}]");
                    return;
                }
                this.OnSelfPlayerSitdownAt(res.seatId);
                break;
            case (int)GMEvent.UserStandupReturn:
                this.OnPlayerStandup((int)param);
                break;
            case (int)GMEvent.UserArrivedSeatReturn:
                this.OnOtherPlayerArrivedSeat((ResUserArrivedSeat)param);
                break;
            case (int)GMEvent.UserExitSeatReturn:
                this.OnOtherPlayerExitSeat(((ResUserExitSeat)param).seatId);
                break;
            case (int)GMEvent.PlayerIsReadyReturn:
                this.OnPLayerIsReady((ResPlayerIsReady)param);
                break;
            case (int)GMEvent.RoundIsReadyReturn:
                this.OnRoundIsReady((ResRoundReady)param);
                break;
            case (int)GMEvent.RoundIsStartedReturn:
                this.OnRoundIsStarted((ResRoundStarted)param);
                break;
            case (int)GMEvent.RoundCheckOutReturn:
                this.OnRoundCheckOut((ResRoundCheckOut)param);
                break;
            case (int)GMEvent.RoundClearReturn:
                this.OnRoundClear((ResRoundClear)param);
                break;
            case (int)GMEvent.ReconnRoomReturn:
                this.OnReconnRoomGame((ResReconnRoom)param);
                break;
            case (int)GMEvent.PlayerOptReturn:
                this.OnPlayerOptAction((ResPlayerOpt)param);
                break;
            case (int)GMEvent.PlayerEscapeReturn:
                this.OnOtherPlayerEscape((ResPlayerEscape)param);
                break;
        }
    }
}
