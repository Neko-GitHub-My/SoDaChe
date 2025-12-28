using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class GM_EffectMgr
{
    public static GM_EffectMgr Instance = new GM_EffectMgr();
    public void Init()
    {
        GM_EffectMgr.Instance = this;
    }

    public GameObject PlayerSkillEffectAt(string SkillEffectName, Transform parent, Vector3 pos, bool isAutoDisponse = true)
    {
        Debug.Log("#####: " + SkillEffectName);
        string effectPath = "Skills/Prefabs/" + SkillEffectName;

        // 这里可以用节点池优化
        GameObject effectPrefab = ResMgr.Instance.LoadAssetSync<GameObject>(effectPath);
        GameObject effect = GameObject.Instantiate(effectPrefab);
        effect.name = SkillEffectName;
        effect.transform.position = pos;
        effect.transform.SetParent(parent, false);
        ParticleSystem pt = effect.GetComponentInChildren<ParticleSystem>();
        pt.Play();
        // end

        if (isAutoDisponse)
        {
            TimerMgr.Instance.ScheduleOnce((object param) => {
                GameObject.Destroy(effect);
            }, pt.main.duration);
        }
        return effect;
    }
}

