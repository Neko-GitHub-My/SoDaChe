

using Framework.Core.Serializer;
using Framework.Core.Utils;
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MessageDispatcher 
{
    public static MessageDispatcher Instance = new MessageDispatcher();

    private static Dictionary<string, CmdExecutor> MODULE_CMD_HANDLERS = new();

    private short[] GetMessageMeta(MethodInfo method)
    {
        foreach (ParameterInfo parameter in method.GetParameters())
        {
            if ((typeof(Message)).IsAssignableFrom(parameter.ParameterType))
            {
                MessageMeta msgMeta = parameter.ParameterType.GetCustomAttribute<MessageMeta>();
                if (msgMeta != null)
                {
                    short[] meta = { msgMeta.module, msgMeta.cmd };
                    return meta;
                }
            }
        }

        return null;
    }

    private string BuildKey(short module, short cmd)
    {
        return module + "_" + cmd;
    }

    public void Init() {
        
        IEnumerable<Type> controllers = TypeScanner.ListTypesWithAttribute(typeof(ResponesProcesser));
        foreach (Type controller in controllers) {
            try
            {
                object handler = Activator.CreateInstance(controller);
                MethodInfo[] methods = controller.GetMethods();

                foreach (MethodInfo method in methods) {
                    ResponesMapping mapperAttribute = method.GetCustomAttribute<ResponesMapping>();
                    if (mapperAttribute == null)
                    {
                        continue;
                    }

                    short[] meta = this.GetMessageMeta(method);
                    short module = meta[0];
                    short cmd = meta[1];

                    string key = this.BuildKey(module, cmd);
                    MODULE_CMD_HANDLERS.TryGetValue(key, out CmdExecutor cmdExecutor);
                    if (cmdExecutor != null)
                    {
                        Debug.LogWarning($"module[{module}] cmd[{cmd}] ÖØ¸´×¢²á´¦ÀíÆ÷");
                        return;
                    }

                    cmdExecutor = CmdExecutor.Create(method, method.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray(), handler);
                    MODULE_CMD_HANDLERS.Add(key, cmdExecutor);
                }
            }
            catch (Exception e) {
                Debug.LogWarning(e.ToString());
            }
        }

        // TcpSocketMgr.Instance.ConnectToServer();
        // SocketMgr.Instance.ConnectToServer();

        EventMgr.Instance.AddListener(SocketMgr.NetConnectedEvent, (int eventType, object udata, object udata2) => {
            Debug.Log("Socket Connect Success");

            
        });

        EventMgr.Instance.AddListener(SocketMgr.NetRecvCmdEvent, (int eventType, object udata, object udata2) => {
            this.OnRecvServerResponse((Message)udata);
        });

        EventMgr.Instance.AddListener(SocketMgr.NetDisconnectEvent, (int eventType, object udata, object udata2) => {
            Debug.Log("Sokcet Disconnect !");
        });

        EventMgr.Instance.AddListener(SocketMgr.NetConnectingEvent, (int eventType, object udata, object udata2) => {
            Debug.Log("Sokcet is Connecting ...");
        });
    }


    public void OnRecvServerResponse(Message msg) {
        short module = msg.GetModule();
        short cmd = msg.GetCmd();
        string key = this.BuildKey(module, cmd);

        MODULE_CMD_HANDLERS.TryGetValue(key, out CmdExecutor cmdExecutor);
        if (cmdExecutor == null)
        {
            Debug.LogWarning($"message executor missed, module={module},cmd={cmd}");
            return;
        }

        object[] @params = new object[] { msg };
        cmdExecutor.method.Invoke(cmdExecutor.handler, @params);
    }
}
