
using Framework.Core.Serializer;
using Framework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

class MessageFactory
{
    public static MessageFactory Instance = new MessageFactory();

    private Dictionary<int, Type> keyTypeDic = new();
    // private Dictionary<Type, int> TypeKeyDic = new();

    public void InitMeesagePool() {
        List<Type> messages = TypeScanner.ListAllSubTypes(typeof(Message)).ToList();

        foreach (Type message in messages) {
            MessageMeta meta = message.GetCustomAttribute<MessageMeta>();
            if (meta == null) {
                Debug.Log($"[致命错误]:没有找到[{message.Name}]的MessageMeta");
            }

            int key = BuildKey(meta.module, meta.cmd);
            if (keyTypeDic.ContainsKey(key)) {
                Debug.Log($"[致命错误]:[{key}]重复注册");
            }

            keyTypeDic.Add(key, message);
            // TypeKeyDic.Add(message, key);
        }
    }

    public int BuildKey(short module, short cmd) {
        return module * (10000) + cmd;
    }

    public bool GetMessage(short module, short cmd, out Type msgType)
    {
        return keyTypeDic.TryGetValue(BuildKey(module, cmd), out msgType);
    }
        
}

