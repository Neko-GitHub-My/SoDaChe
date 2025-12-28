using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;
using Game.Datas.Messages;

public class UIRoomFight_UICtrl : UICtrl {

	void Start() {

        this.ViewNode("RoomDesic").SetActive(false);
        this.ViewNode("LostHpTip").SetActive(false);
        this.ViewNode("top/SkillTip").SetActive(false);

        this.AddButtonListener("top/ForceQuit", this.OnPlayerForceQuit);
        this.AddButtonListener("SkillOptRoot/SkillA", () => { this.OnStartSkill(1000001); });
        this.AddButtonListener("SkillOptRoot/Buff1", () => { this.OnStartBuff(100001); });

        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void OnDestroy() {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
    }

    private void OnPlayerForceQuit() {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIExitLogicServer, (int)QuitReason.ForceQuit);
    }

    private void OnStartSkill(int skillId) {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.StartSkill, skillId);
    }

    private void OnStartBuff(int buffId) {
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.StartBuff, buffId);
    }

    private void ShowBuffTip(string str) {
        this.ViewNode("top/SkillTip").SetActive(true);
        this.View<Text>("top/SkillTip").text = str;

        /*TimerMgr.Instance.ScheduleOnce((object udata) => {
            this.ViewNode("top/SkillTip").SetActive(false);
        }, 2.5f);*/
    }

    private void ShowTipDesic(string str)
    {
        this.ViewNode("RoomDesic").SetActive(true);
        this.View<Text>("RoomDesic").text = str;

        /*TimerMgr.Instance.ScheduleOnce((object udata) => {
            this.ViewNode("RoomDesic").SetActive(false);
        }, 2.5f);*/
    }

    private void ShowLostHpTipDesic(string str)
    {
        this.ViewNode("LostHpTip").SetActive(true);
        this.View<Text>("LostHpTip").text = str;

        /*TimerMgr.Instance.ScheduleOnce((object udata) => {
            this.ViewNode("LostHpTip").SetActive(false);
        }, 2.5f);*/
    }

    private void OnRoundCheckOut(ResRoundCheckOut res) {
        if (res.reserve == -1) {
            this.ShowTipDesic($"游戏平局");
        }
        else {
            this.ShowTipDesic($"游戏结束,[{res.reserve}]号玩家赢了");
        }
        
    }

    private void OnStartSkillReturn(ResStartSkill res)  {
        this.ShowTipDesic("玩家[" + res.seatOrWorldId + "]放了技能: " + res.skillId);
    }

    private void OnStartBuffReturn(ResStartBuff res) {
        this.ShowTipDesic("玩家[" + res.seatOrWorldId + "]放了Buff: " + res.buffId);
    }

    private void OnLostHpReturn(ResLostHp res) {
        this.ShowLostHpTipDesic("玩家[" + res.seatOrWorldId + "]掉血: " + res.lostHp);
    }

    private void OnUIEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.RoundIsReadyReturn:
                this.ShowTipDesic("游戏马上开始!");
                break;
            case (int)GMEvent.RoundIsStartedReturn:
                // this.ShowTipDesic("游戏正式开始!");
                break;
            case (int)GMEvent.PlayerEscapeReturn:
                this.ShowTipDesic("玩家逃跑了");
                break;
            case (int)GMEvent.UIBuffStarted:
                this.ShowBuffTip("Buff开启了");
                break;
            case (int)GMEvent.UIBuffReady:
                this.ShowBuffTip("Buff可用了");
                break;
            case (int)GMEvent.UIBuffFreezed:
                this.ShowBuffTip("Buff冷却中");
              break;
            case (int)GMEvent.RoundCheckOutReturn:
                this.OnRoundCheckOut((ResRoundCheckOut) param);
                break;
            case (int)GMEvent.StartSkillReturn:
                this.OnStartSkillReturn((ResStartSkill)param);
                break;
            case (int)GMEvent.StartBuffReturn:
                this.OnStartBuffReturn((ResStartBuff)param);
                break;
            case (int)GMEvent.LostHpReturn:
                this.OnLostHpReturn((ResLostHp)param);
                break;
        }
    }
}
