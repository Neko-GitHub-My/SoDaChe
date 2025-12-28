using Game.Datas.Messages;
using LitJson;
using ProtoBuf.Meta;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RVOWorldMgr : BaseWorldMgr
{
    public static RVOWorldMgr Instance = null;

    protected MapData mapData = null;

    public void Awake()
    {
        RVOWorldMgr.Instance = this;
    }

    public override async void Init(int mapId/*object config*/)
    {
        base.Init(mapId);

        EventMgr.Instance.AddListener((int)(EventType.Logic), this.OnLogicEventProcess);
        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.EnterLogicServer, null);

        // 加载我们的地图数据;
#if false
        TextAsset textAsset = await ResMgr.Instance.AwaitGetAsset<TextAsset>("Maps/Datas/" + mapId + ".json");
        if (!textAsset)
        {
            return;
        }

        this.mapData = JsonMapper.ToObject<MapData>(textAsset.text);

        RectTransform t = GameObject.Find("MapCanvas").transform as RectTransform;
        float width = (mapData.mapWidth < t.sizeDelta.x) ? this.mapData.mapWidth : t.sizeDelta.x;
        float height = (mapData.mapHeight < t.sizeDelta.y) ? this.mapData.mapHeight : t.sizeDelta.y;
        this.transform.localPosition = new Vector3(-width * 0.5f, -height * 0.5f);
#endif

    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.RemoveListener((int)(EventType.Logic), this.OnLogicEventProcess);
    }

    private void RemoveWorld()
    {
        RvoNavSystem.ClearRVO();
        GameObject.Destroy(this.gameObject);
    }

    private void OnUITouchMap(PointerEventData eventData)
    {
        Ray ray = this.gameCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            ReqNavToDst req = new ReqNavToDst();
            req.x = hitInfo.point.x;
            req.y = 0.0f;
            req.z = hitInfo.point.z;
            Debug.Log(hitInfo.point.ToString());
            // Debug.Log(hitInfo.collider.gameObject.name);
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.NavToDst, req);
        }
    }

    private void OnUIEventProcess(int eventType, object udata, object param)
    {
        switch (udata)
        {
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
            case (int)GMEvent.UserEnterRoomReturn:
                // this.OnEnterGameWorldIndep((ResEnterRoom)param);
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
                this.OnPlayerNavWithDir((ResNavInDir) param);
                break;
            case (int)GMEvent.SyncCharactorStatusReturn:
                this.OnSyncCharactorStatus((ResSyncCharactorStatus)param);
                break;
        }
    }
}
