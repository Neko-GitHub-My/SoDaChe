
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class BonuesRule : Attribute
{
    public int mainType;

    public BonuesRule(int mainType)
    {
        this.mainType = mainType;
    }
}


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class BonuesApplay : Attribute
{
    public int subType;

    public BonuesApplay(int subType)
    {
        this.subType = subType;
    }
}


public class GM_BonuesMgr : Singleton<GM_BonuesMgr>
{
    private Dictionary<int, MethodInfo> bonuesApplayFuncs = new Dictionary<int, MethodInfo>();

    public void Init() {
        IEnumerable<Type> BonuesRules = TypeScanner.ListTypesWithAttribute(typeof(BonuesRule));
        foreach (Type rule in BonuesRules)
        {
            try
            {
                BonuesRule ruleAttribute = rule.GetCustomAttribute<BonuesRule>();
                MethodInfo[] methods = rule.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (MethodInfo method in methods)
                {
                    BonuesApplay mapperAttribute = method.GetCustomAttribute<BonuesApplay>();
                    if (mapperAttribute != null)
                    {
                        int key = ruleAttribute.mainType + mapperAttribute.subType;
                        if (bonuesApplayFuncs.ContainsKey(key))
                        {
                            Debug.LogError($"dumplate BonuesApplay Key: {key}");
                            continue;
                        }

                        bonuesApplayFuncs.Add(key, method);
                    }

                }
            }
            catch { }

        }
    }

    public void ApplayBonuesToPlayer(ResRecvBonues bonues) {
        MethodInfo method = null;
        if (this.bonuesApplayFuncs.ContainsKey(bonues.typeId))
        {
            method = this.bonuesApplayFuncs[bonues.typeId];
        }
        else
        {
            int mainKey = ((bonues.typeId) / 100000) * 100000;
            int defaultKey = mainKey - 1;
            if (this.bonuesApplayFuncs.ContainsKey(defaultKey))
            {
                method = this.bonuesApplayFuncs[defaultKey];
            }
        }

        if (method == null) {
            Debug.LogWarning($"unkonw BonuesType: {bonues.typeId}");
            return;
        }

        method.Invoke(null, new object[] { bonues });
    }
}




