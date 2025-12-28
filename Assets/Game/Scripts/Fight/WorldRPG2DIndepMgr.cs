using Game.Datas.Messages;
using LitJson;
using ProtoBuf.Meta;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldRPG2DIndepMgr : BaseWorld2DMgr
{
    public static WorldRPG2DIndepMgr Instance = null;

    protected MapData mapData = null;

    public void Awake()
    {
        WorldRPG2DIndepMgr.Instance = this;
    }

    public override async void Init(int mapId/*object config*/)
    {
        base.Init(mapId);

        EventMgr.Instance.AddListener((int)(EventType.Logic), this.OnLogicEventProcess);
        EventMgr.Instance.AddListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.Emit((int)EventType.Logic, (int)GMEvent.EnterLogicServer, null);

        // 加载我们的地图数据;
        TextAsset textAsset = await ResMgr.Instance.AwaitGetAsset<TextAsset>("Maps/Datas/" + mapId + ".json");
        if (!textAsset) {
            return;
        }

        this.mapData = JsonMapper.ToObject<MapData>(textAsset.text);

        RectTransform t = GameObject.Find("MapCanvas").transform as RectTransform;
        float width = (mapData.mapWidth < t.sizeDelta.x) ? this.mapData.mapWidth : t.sizeDelta.x;
        float height = (mapData.mapHeight < t.sizeDelta.y) ? this.mapData.mapHeight : t.sizeDelta.y;
        this.transform.localPosition = new Vector3(-width * 0.5f, -height * 0.5f);
    }

    private void OnDestroy()
    {
        EventMgr.Instance.RemoveListener((int)(EventType.UI), this.OnUIEventProcess);
        EventMgr.Instance.RemoveListener((int)(EventType.Logic), this.OnLogicEventProcess);
    }

    private void RemoveWorld() {
        this.cameraCtrl.BindFollowTarget(null);
        GameObject.Destroy(this.gameObject);
    }

    private void OnUITouchMap(PointerEventData eventData) {
        // 把这个UI坐标，转成地图坐标，然后发送给服务器
        Vector2 mapPostion;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)this.transform, eventData.position, this.gameCamera, out mapPostion);
        ReqNavToDst req = new ReqNavToDst();
        req.x = mapPostion.x;
        req.y = 0;
        req.z = mapPostion.y;
        
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.NavToDst, req);
        // end
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
                // this.OnEnterGameWorld((int)param);
                break;
            case (int)GMEvent.UserEnterRoomReturn:
                this.OnEnterGameWorldIndep((ResEnterRoom)param);
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
            case (int)GMEvent.SyncCharactorStatusReturn:
                this.OnSyncCharactorStatus((ResSyncCharactorStatus) param);
                break;
        }
    }
}
