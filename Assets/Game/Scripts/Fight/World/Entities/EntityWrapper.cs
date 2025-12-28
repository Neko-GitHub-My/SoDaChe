
using Game.Datas.Messages;
using System.Threading.Tasks;
using UnityEngine;


/**
 * 设置单位方向
 * 
 * 方向值范围为 0-7，方向值设定如下，0是下，1是左下，2是左，3是左上，4是上，5是右上，6是右，7是右下
 * 
 *        4
 *      3   5
 *    2   *   6
 *      1   7
 *        0
 * 
 */

public class EntityWrapper {

    public static string[] animNames = {"idle", "run", "dance", "attack", "die"};

    public static CharactorEntity Create() {
        CharactorEntity e = new CharactorEntity();
        e.worldId = -1;

        // Transform
        e.uTransform.pos = Vector3.zero;
        e.uTransform.eulerAngles = new Vector3(0, 0, 0);
        // end

        // StatusComponent
        e.uStatus.status = (int)CharactorStatus.Invalid;
        // end

        // NavComponent
        e.uNav.vx = e.uNav.vz = 0.0f;
        e.uNav.passedTime = e.uNav.totalTime = 0.0f;
        // end

        // SkillAndBuff
        e.uSkillAndBuff.skillTimeLine.Init();
        e.uSkillAndBuff.buffTimeLine.Init();
        // end

        // AStarNav
        AStarComponent.Init(ref e.uAStarNav);
        // end

        // RVO Component
        RVOComponent.Init(ref e.uRvo);
        // end

        // PropsComponent
        PropsComponent.Init(ref e.uProps);
        // end

        return e;
    }

    public static CharactorEntity2D Create2D()
    {
        CharactorEntity2D e = new CharactorEntity2D();
        e.worldId = -1;

        // Transform
        e.uTransform.pos = Vector3.zero;
        e.uTransform.eulerAngles = new Vector3(0, 0, 0);
        // end

        // StatusComponent
        e.uStatus.status = (int)CharactorStatus.Invalid;
        // end

        // NavComponent
        e.uNav.vx = e.uNav.vz = 0.0f;
        e.uNav.passedTime = e.uNav.totalTime = 0.0f;
        // end

        // SkillAndBuff
        e.uSkillAndBuff.skillTimeLine.Init();
        e.uSkillAndBuff.buffTimeLine.Init();
        // end

        // AStarNav
        AStarComponent.Init(ref e.uAStarNav);
        // end

        // PropsComponent
        // PropsComponent.Init(ref e.uProps);
        // end

        return e;
    }

    public static async Task<CharactorEntity2D> Create2D(ArrivedCharactor ch, Transform parent, bool isSelf = false)
    {

        CharactorEntity2D e = EntityWrapper.Create2D();

        e.worldId = ch.worldId;

        // CharactorInfo
        e.uInfo.unick = ch.chInfo.unick;
        e.uInfo.job = ch.chInfo.job;
        e.uInfo.charactorId = ch.chInfo.charactorId; // -1
        e.uInfo.sex = ch.chInfo.sex;
        // end

        // Transform
        e.uTransform.pos.x = ch.transform.pos[0];
        e.uTransform.pos.y = ch.transform.pos[1];
        e.uTransform.pos.z = ch.transform.pos[2];

        // Debug.Log(ch.transform.pos.ToString());
        e.uTransform.eulerAngles.x = ch.transform.eulerAngles[0];
        e.uTransform.eulerAngles.y = ch.transform.eulerAngles[1];
        e.uTransform.eulerAngles.z = ch.transform.eulerAngles[2];
        // end


        // 创建Unity的相关节点, 正式项目，结合CharactorId,我们模板直接写死一个角色;
        GameObject prefab = null;
        prefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("Charactors/Prefabs/10003");
        e.uAnim.unityObject = GameObject.Instantiate(prefab);
        
        e.uAnim.unityObject.name = e.uInfo.unick;
        e.uAnim.unityObject.transform.SetParent(parent, false);
        // Debug.Log(e.uTransform.pos.ToString());
        // Debug.Log(ch.transform.pos[0] + "-" + ch.transform.pos[2]);
        /*Vector3 pos = e.uAnim.unityObject.transform.localPosition;
        pos.x = e.uTransform.pos.x;
        pos.y = e.uTransform.pos.z;
        e.uAnim.unityObject.transform.localPosition = pos;

        // e.uAnim.unityObject.transform.eulerAngles = e.uTransform.eulerAngles;
        float moveAngle = e.uTransform.eulerAngles.y;
        int dire = (int)Mathf.Round((-moveAngle + 180) / (180 / 4));
        e.uAnim.direction = dire > 5 ? dire - 6 : dire + 2;*/

        EntityWrapper.SyncToUnityTransform2D(e);

        e.uAnim.unityObject.transform.Find("Body/Skin_Walk").gameObject.SetActive(false);
        e.uAnim.unityObject.transform.Find("Body/Skin_Idle").gameObject.SetActive(false);

        e.uAnim.animState = (int)CharactorStatus.Invalid; // 动画状态;
        e.uAnim.movieClip = null;
        // end

        if (isSelf)
        {
            e.uAnim.unityObject.transform.Find("AOIMask").gameObject.SetActive(false);
        }
        else {
            e.uAnim.unityObject.transform.Find("AOIMask").gameObject.SetActive(false);
        }

        return e;
    }

    public static async Task<CharactorEntity> Create(ArrivedCharactor ch, Transform parent)
    {

        CharactorEntity e = EntityWrapper.Create();

        e.worldId = ch.worldId;

        // CharactorInfo
        e.uInfo.unick = ch.chInfo.unick;
        e.uInfo.job = ch.chInfo.job;
        e.uInfo.charactorId = ch.chInfo.charactorId;
        e.uInfo.sex = ch.chInfo.sex;
        // end

        // Transform
        e.uTransform.pos.x = ch.transform.pos[0];
        e.uTransform.pos.y = ch.transform.pos[1];
        e.uTransform.pos.z = ch.transform.pos[2];

        // Debug.Log(ch.transform.pos.ToString());
        e.uTransform.eulerAngles.x = ch.transform.eulerAngles[0];
        e.uTransform.eulerAngles.y = ch.transform.eulerAngles[1];
        e.uTransform.eulerAngles.z = ch.transform.eulerAngles[2];
        // end

        // 创建Unity的相关节点;
        GameObject prefab = null;
        if (GM_DataMgr.Instance.zid == 200030 || GM_DataMgr.Instance.zid == 20003) { // RVO
            prefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("Charactors/Prefabs/10004");
            e.uRvo.agentId = RvoNavSystem.CreateAgent(e.uTransform.pos, 0.5f);
        }
        else {
            prefab = await ResMgr.Instance.AwaitGetAsset<GameObject>("Charactors/Prefabs/10001");
        }
        
        e.uAnim.unityObject = GameObject.Instantiate(prefab);
        e.uAnim.unityObject.name = e.uInfo.unick;
        e.uAnim.unityObject.transform.SetParent(parent, false);
        // Debug.Log(e.uTransform.pos.ToString());
        // Debug.Log(ch.transform.pos[0] + "-" + ch.transform.pos[2]);


        // e.uAnim.unityObject.transform.position = e.uTransform.pos;
        // e.uAnim.unityObject.transform.eulerAngles = e.uTransform.eulerAngles;
        EntityWrapper.SyncToUnityTransform(e);

        e.uAnim.animState = (int)CharactorStatus.Invalid; // 动画状态;
        e.uAnim.animPlayer = e.uAnim.unityObject.GetComponentInChildren<Animation>();
        e.uAnim.animPlayer.Stop();
        // end


        return e;
    }

    public static async Task<CharactorEntity> Create(string unick, int usex,
        int seatId, string url, Transform parent)
    {
        CharactorEntity e = EntityWrapper.Create();

        e.worldId = seatId;

        // CharactorInfo
        e.uInfo.unick = unick;
        e.uInfo.job = -1;
        e.uInfo.charactorId = -1;
        e.uInfo.sex = usex;
        // end

        // ...

        // 创建Unity的相关节点;
        var prefab = await ResMgr.Instance.AwaitGetAsset<GameObject>(url);

        e.uAnim.unityObject = GameObject.Instantiate(prefab);
        e.uAnim.unityObject.name = e.uInfo.unick;
        e.uAnim.unityObject.transform.SetParent(parent, false);

        e.uAnim.unityObject.transform.position = e.uTransform.pos;
        e.uAnim.unityObject.transform.eulerAngles = e.uTransform.eulerAngles;
        e.uAnim.animState = (int)CharactorStatus.Invalid; // 动画状态;
        e.uAnim.animPlayer = e.uAnim.unityObject.GetComponentInChildren<Animation>();
        e.uAnim.animPlayer.Stop();

        return e;
    }

    public static void SyncToUnityTransform(CharactorEntity e) {
        if (e.uAnim.unityObject == null) {
            return;
        }

        e.uAnim.unityObject.transform.position = e.uTransform.pos;
        Vector3 animEuler = e.uTransform.eulerAngles;
        animEuler.y = 90 - animEuler.y;
        e.uAnim.unityObject.transform.localEulerAngles = animEuler;
    }

    public static void SyncToUnityTransform2D(CharactorEntity2D e)
    {
        if (e.uAnim.unityObject == null) {
            return;
        }
        // 同步位置
        Vector3 pos = e.uAnim.unityObject.transform.localPosition;
        pos.x = e.uTransform.pos.x;
        pos.y = e.uTransform.pos.z;
        e.uAnim.unityObject.transform.localPosition = pos;

        // 同步角色的方向
        float moveAngle = e.uTransform.eulerAngles.y;
        int dire = (int)Mathf.Round((-moveAngle + 180) / (180 / 4));
        int direction = dire > 5 ? dire - 6 : dire + 2;
        e.uAnim.direction = direction;
        EntityWrapper.SetAnim2DDirection(e);
        // end
    }

    private static void SetAnim2DDirection(CharactorEntity2D e)
    {
        if (e.uAnim.movieClip == null) {
            return;
        }

        switch (e.uAnim.direction)
        {
            case 0:
                e.uAnim.movieClip.rowIndex = 7 - 0;
                break;

            case 1:
                e.uAnim.movieClip.rowIndex = 7 - 4;
                break;

            case 2:
                e.uAnim.movieClip.rowIndex = 7 - 1;
                break;

            case 3:
                e.uAnim.movieClip.rowIndex = 7 - 6;
                break;

            case 4:
                e.uAnim.movieClip.rowIndex = 7 - 3;
                break;

            case 5:
                e.uAnim.movieClip.rowIndex = 7 - 7;
                break;

            case 6:
                e.uAnim.movieClip.rowIndex = 7 - 2;
                break;

            case 7:
                e.uAnim.movieClip.rowIndex = 7 - 5;
                break;
        }
    }


    public static void SetEntityStatus(ref StatusComponent uStatus, int status)
    {
        if (uStatus.status == status) {
            return;
        }

        uStatus.status = status;
        
    }

    public static void SyncEntityAnimStatus(CharactorEntity e) {
        if (e.uAnim.unityObject == null) {
            return;
        }
        // 切换动画的播放
        if (e.uAnim.animState != e.uStatus.status)
        {
            e.uAnim.animState = e.uStatus.status;
            Debug.Log(animNames[e.uAnim.animState]);
            e.uAnim.animPlayer.CrossFade(animNames[e.uAnim.animState]);
        }
        // end
    }

    public static void SyncEntityAnimStatus(CharactorEntity2D e)
    {
        if (e.uAnim.unityObject == null) {
            return;
        }
        // 切换动画的播放
        if (e.uAnim.animState != e.uStatus.status)
        {
            e.uAnim.animState = e.uStatus.status;
            if (e.uAnim.movieClip != null)
            {
                e.uAnim.movieClip.gameObject.SetActive(false);
            }

            if (e.uStatus.status == (int)CharactorStatus.Run)
            {
                e.uAnim.movieClip = e.uAnim.unityObject.transform.Find("Body/Skin_Walk").GetComponent<MovieClip>();
            }
            else
            {
                e.uAnim.movieClip = e.uAnim.unityObject.transform.Find("Body/Skin_Idle").GetComponent<MovieClip>();
            }

            e.uAnim.movieClip.gameObject.SetActive(true);
            EntityWrapper.SetAnim2DDirection(e);

            e.uAnim.movieClip.playIndex = 0;
            e.uAnim.movieClip.playAction();
        }
        // end
    }
}



