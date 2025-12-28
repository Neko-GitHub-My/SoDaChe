using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class RVOMapWrapper : IMapWrapper
{
    private int mapId = -1;

    private RVOMapEditor.LevelConfig mapData;
    public async void LoadMapData(int mapId, Transform mapRoot = null, UnityAction onEnd = null)
    {
        this.mapId = mapId;

        // 加载我们的Map数据
        TextAsset textAsset = await ResMgr.Instance.AwaitGetAsset<TextAsset>("Maps/Datas/" + mapId + ".json");
        this.mapData = JsonMapper.ToObject<RVOMapEditor.LevelConfig>(textAsset.text);
        // end

        if (this.mapData == null) {
            Debug.LogWarning("LevelConfig == null : " + mapId);
            return;
        }

        // 构建RVO数据
        RvoNavSystem.RVOAddObstacles(this.mapData.rvoObstacles);

        // end

        for (int i = 0; this.mapData.maps != null && i < this.mapData.maps.Length; i++) {
            int typeId = this.mapData.maps[i].typeId;
            MapItemBase itemBase = ExcelDataMgr.Instance.GetConfigData<MapItemBase>(typeId.ToString());
            if (itemBase == null) {
                Debug.LogWarning("itemBase == null : " + typeId);
                continue;
            }

            var mapPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>(itemBase.assetUrl);
            var mapItem = GameObject.Instantiate(mapPrefab);
            mapItem.name = mapPrefab.name;
            mapItem.transform.SetParent(mapRoot, false);
            mapItem.transform.position = this.mapData.maps[i].pos;
            mapItem.transform.eulerAngles = this.mapData.maps[i].rot;
        }

        // 生成障碍物
        for (int i = 0; this.mapData.obses != null && i < this.mapData.obses.Length; i++)
        {
            int typeId = this.mapData.obses[i].typeId;
            MapItemBase itemBase = ExcelDataMgr.Instance.GetConfigData<MapItemBase>(typeId.ToString());
            if (itemBase == null)
            {
                Debug.LogWarning("itemBase == null : " + typeId);
                continue;
            }
            var obsPrefab = await ResMgr.Instance.AwaitGetAsset<GameObject>(itemBase.assetUrl);
            var obs = GameObject.Instantiate(obsPrefab);
            obs.name = obsPrefab.name;
            obs.transform.SetParent(mapRoot, false);
            obs.transform.position = this.mapData.obses[i].pos;
            obs.transform.eulerAngles = this.mapData.obses[i].rot;
        }
        // end

        if (onEnd != null) {
            onEnd();
        }
    }

    public int MapHeight()
    {
        throw new System.NotImplementedException();
    }

    public int MapWidth()
    {
        throw new System.NotImplementedException();
    }

    public void OnNavLateUpdate(CharactorEntity e, float dt)
    {
        // 同步Agent到 uTransform
        RvoNavSystem.NavAgentLateUpdate(e, dt);
        EntityWrapper.SyncToUnityTransform(e);
    }

    public void OnNavUpdate(CharactorEntity e, float dt)
    {
        RvoNavSystem.NavAgentUpdate(e, dt);
    }

    public void OnNavUpdate(CharactorEntity2D e, float dt)
    {
        // throw new System.NotImplementedException();
    }

    public void SyncEntityAlphaWithMap(CharactorEntity2D e)
    {
        // throw new System.NotImplementedException();
    }

    public void WalkInDir(CharactorEntity e, float speed, int dirx, int diry)
    {
        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Run);
        RvoNavSystem.StartRvoNavAction(e, speed, dirx, diry);
    }

    public void WalkToDst(CharactorEntity e, float speed, float x, float y, float z)
    {
        EntityWrapper.SetEntityStatus(ref e.uStatus, (int)CharactorStatus.Run);
        RvoNavSystem.StartRvoNavAction(e, speed, x, y, z);
    }

    public void WalkToDst(CharactorEntity2D e, float speed, float x, float z)
    {
        // throw new System.NotImplementedException();
    }
}
