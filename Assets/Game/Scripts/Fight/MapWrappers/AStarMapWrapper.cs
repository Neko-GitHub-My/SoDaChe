
using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Analytics;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine.Events;


public class MapSpawnPoint
{
    public float x; // 出生点的位置
    public float y; // 出生点的位置

    public int spawnId; // 当前我们对应的spawnId;
    public bool defaultSpawn; // 是否为默认的Spawn;
}

public class AStarMapWrapper : IMapWrapper
{
    private int mapId;

    public MapData mapData = null;

    private Dictionary<int, MapSpawnPoint> spawnPoints = null;

    private void InitSpawnPointInMap(JsonData item)
    {
        string itemType = (string)item["type"];

        if (itemType.Equals("spawnPoint"))
        { // 出生点,不是entity,所以这里到时候特殊处理一下
            MapSpawnPoint entity = new MapSpawnPoint();
            entity.spawnId = (int)((item["spawnId"].IsInt) ? (int)item["spawnId"] : (double)(item["spawnId"]));
            entity.x = (float)((item["x"].IsInt) ? (int)item["x"] : (double)(item["x"]));
            entity.y = (float)((item["y"].IsInt) ? (int)item["y"] : (double)(item["y"]));
            entity.defaultSpawn = (bool)item["defaultSpawn"];
            if (entity != null) {
                this.spawnPoints.Add(entity.spawnId, entity);
            }
        }
    }
    private void InitMapSpwanPoint(JsonData mapItems)
    {
        this.spawnPoints = new Dictionary<int, MapSpawnPoint>();
        for (int i = 0; i < mapItems.Count; i++)
        {
            JsonData item = mapItems[i];
            this.InitSpawnPointInMap(item);
        }
    }

    public async void LoadMapData(int mapId, Transform mapRoot = null, UnityAction onEnd = null)
    {
        this.mapId = mapId;
        // string path = "Configs/Jsons/MapDatas/" + this.mapId + ".json";
        // string mapJsonText = File.ReadAllText(path);
        TextAsset textAsset = await ResMgr.Instance.AwaitGetAsset<TextAsset>("Maps/Datas/" + mapId + ".json");
        this.mapData = JsonMapper.ToObject<MapData>(textAsset.text);
        JsonData mapDataJson = JsonMapper.ToObject(textAsset.text);

        // 读取地图里面所有玩家的出生点，到我们的内存里面来;
        this.InitMapSpwanPoint(mapDataJson["mapItems"]);
        // end

        // 读取其它的;
        // end

        // 初始化寻路系统，把寻路关联地图数据;  [0, 0, 0, ....]
        PathFindingAgent.instance.init(this.mapData);
        // end
    }

    

    public void WalkToDst(CharactorEntity2D e, float speed, float x, float z)
    {
        List<RoadNode> roadNodeArr = PathFindingAgent.instance.seekPath2(e.uTransform.pos.x, e.uTransform.pos.z, x, z);
        if (roadNodeArr == null || roadNodeArr.Count < 2)
        {
            // e.uStatus.status = (int)CharactorStatus.Idle;
            return;
        }

        // e.uProps.speed = speed;
        e.uAStarNav.speed = speed;
        AStarNavSystem.StartRoadNavAction(ref e.uAStarNav, roadNodeArr);

        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Run);


    }

    public void WalkToDst(CharactorEntity e, float speed, float x, float y, float z)
    {
        throw new System.NotImplementedException();
    }

    public void SyncEntityAlphaWithMap(CharactorEntity2D e)
    {
        if (e.uAnim.movieClip == null)
        {
            return;
        }

        Image movieImg = e.uAnim.movieClip.GetComponent<Image>();
        RoadNode roadNode = PathFindingAgent.instance.getRoadNodeByPixel(e.uTransform.pos.x, e.uTransform.pos.z);
        if (roadNode == null)
        {
            return;
        }

        switch (roadNode.value)
        {
            case 2: // 半透的这种遮挡路点
                if (movieImg.color.a != 0.4f)
                {
                    Color color = movieImg.color;
                    color.a = 0.4f;
                    movieImg.color = color;
                }
                break;
            case 3: // 隐藏路点,全部透明;
                if (movieImg.color.a > 0.0f)
                {
                    Color color = movieImg.color;
                    color.a = 0.0f;
                    movieImg.color = color;
                }
                break;
            default: // 不透明的
                if (movieImg.color.a < 1.0f)
                {
                    Color color = movieImg.color;
                    color.a = 1.0f;
                    movieImg.color = color;
                }
                break;
        }
    }

    public void OnNavUpdate(CharactorEntity2D e, float dt)
    {
        AStarNavSystem.NavRoadUpdate(ref e.uAStarNav, ref e.uStatus, ref e.uTransform, dt);
        EntityWrapper.SyncToUnityTransform2D(e);
    }

    public void OnNavUpdate(CharactorEntity e, float dt)
    {
        AStarNavSystem.NavRoadUpdate(ref e.uAStarNav, ref e.uStatus, ref e.uTransform, dt);
        EntityWrapper.SyncToUnityTransform(e);
    }

    public int MapWidth()
    {
        return this.mapData.mapWidth;
    }

    public int MapHeight()
    {
        return this.mapData.mapHeight;
    }

    public void OnNavLateUpdate(CharactorEntity e, float dt)
    {
        // throw new System.NotImplementedException();
    }

    public void WalkInDir(CharactorEntity e, float speed, int dirx, int diry)
    {
        throw new System.NotImplementedException();
    }
}

