using Game.Datas.Messages;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldCityMgr : BaseWorldMgr
{
    public static WorldCityMgr Instance = null;



    public void Awake()
    {
        WorldCityMgr.Instance = this;
    }

    public override void Init(int mapId/*object config*/)
    {
        base.Init(mapId);

        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.AddListener((int)(EventType.Logic), this.OnLogicEventProcess);
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.EnterLogicServer, null);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.RemoveListener((int)(EventType.Logic), this.OnLogicEventProcess);
    }

    private void RemoveWorld() {
        GameObject.Destroy(this.gameObject);
    }

    private void OnUITouchMap(PointerEventData eventData) {
        Ray ray = this.gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            ReqNavToDst req = new ReqNavToDst();
            req.x = hitInfo.point.x;
            req.y = hitInfo.point.y;
            req.z = hitInfo.point.z;
            Debug.Log(hitInfo.point.ToString());
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.NavToDst, req);
        }
    }

    private void OnUIEventProcess(int eventType, object udata, object param) {
        switch (udata) {
            case (int)GMEvent.UITouchMap:
                this.OnUITouchMap(param as PointerEventData);
                break;
        }
    }

    private void OnLogicEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
            case (int)GMEvent.UserExitLogicServerReturn:
                this.RemoveWorld();
                break;
            case (int)GMEvent.EnterLogicServerReturn:
                Debug.Log($"找逻辑服实例返回: status: {(int)param}");
                this.OnEnterGameWorld((int)param);
                break;
            case (int)GMEvent.PlayerSpawnReturn:
                Debug.Log($"玩家出生返回: status: {(int)((ResPlayerSpawn)param).status}");
                this.OnPlayerSpawnAtMap((ResPlayerSpawn)param);
                break;
            case (int)GMEvent.PlayerEnterAOIReturn:
                this.OnCharactorEnterAOI((ResEnterAOI)param);
                break;
            case (int)GMEvent.PlayerLeaveAOIReturn:
                this.OnCharactorLeaveAOI((ResLeaveAOI)param);
                break;
            case (int)GMEvent.NavToDstReturn:
                this.OnPlayerNavToDst((ResNavToDst)param);
                break;
            case (int)GMEvent.NavInDirReturn:
                this.OnPlayerNavWithDir((ResNavInDir)param);
                break;
            case (int)GMEvent.SyncCharactorStatusReturn:
                this.OnSyncCharactorStatus((ResSyncCharactorStatus) param);
                break;
        }
    }
}
