using Game.Datas.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_DataMgr : Singleton<GM_DataMgr>
{
    public const string GUEST_KEY = "BYCW_SLG_GUEST_KEY";
    public string guestKey = null;

    public AccountInfo account = null;
    public PlayerInfo playerData = null;

    public BonuesItem[] bonuesArray = null;
    public TaskItem[] tasksArray = null;
    public MailMsgItem[] mailMsgArray = null;

    public int machineId = 0;
    public long OpenId = 0;

    public int zid = -1;
    public int stype = -1;
    public int useDirectConn = 0;

    public void Init() {
        //ExcelDataMgr.Instance.ReadConfigData<ClientMachineIdConfig>();
        // 下一次登录的时候，我们还是要之前的游客key;
        this.guestKey = PlayerPrefs.GetString(GUEST_KEY, null);

        
    }


    public void SaveGuestKey(string guestKey) {
        PlayerPrefs.SetString(GUEST_KEY, guestKey);
        this.guestKey = guestKey;
    }

    public void SetMachineIdWithLogicEntry(int entryId) {
        ClientMachineIdConfig config = ExcelDataMgr.Instance.GetConfigData<ClientMachineIdConfig>(entryId.ToString());
        if (config == null) {
            this.machineId = 0;
        }
        else {
            this.machineId = config.MachineId;
            if (this.useDirectConn == 1) {
                ServerZoneMgr.Instance.DirectConnectToMachineServer(this.machineId);
            }
        }
            
    }
}
