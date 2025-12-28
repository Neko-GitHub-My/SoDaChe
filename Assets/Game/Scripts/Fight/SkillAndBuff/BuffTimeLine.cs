
using System.Collections.Generic;
using UnityEngine;

public enum BuffState
    {
        Invalid,
        Ready, // Buff准备好了，可以使用了;
        Started, // Buff开启了;
        Freezed, // Buff无法开启;
    }

    public class BuffNode
    {
        public int buffId;

        public BuffState state;

        public float freezeTime;
        public float durationTime;

        public float passedTime;

        public BuffNode(int buffId)
        {
            this.buffId = buffId;
            this.state = BuffState.Ready;

            this.freezeTime = -1f;
            this.durationTime = -1f;
            this.passedTime = 0;
        }
    }

    public struct BuffTimeLine
    {
        private Dictionary<int, BuffNode> buffNodeSet;
        

        public void Init()
        {
            this.buffNodeSet = new Dictionary<int, BuffNode>();
        }

        public bool HasBuff(int buffId)
        {
            if (!this.buffNodeSet.ContainsKey(buffId))
            {
                return false;
            }

            BuffNode node = this.buffNodeSet[buffId];
            if (node.state != BuffState.Started)
            {
                return false;
            }

            return true;
        }

        public BuffNode AddBuffNode(int buffId)
        {
            if (this.buffNodeSet.ContainsKey(buffId))
            {
                return this.buffNodeSet[buffId];
            }
            BuffNode node = GM_BuffMgr.Instance.CreateBuffNode(buffId);
            if (node == null)
            {
                // Debug.LogError("Can not find BuffNode");
                return null;
            }
            this.buffNodeSet.Add(buffId, node);

            return node;
        }

        public bool StartBuff(int buffId)
        {
            Debug.Log("StartBuff called!!###");
            BuffNode node = this.AddBuffNode(buffId);
            if (node.state != BuffState.Ready)
            {
                return false;
            }
            Debug.Log("StartBuff called!! &&&&");
            node.state = BuffState.Started;
            node.passedTime = 0;
            
            // test
            EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIBuffStarted);
            return true;
        }

        public void RemoveBuff(int buffId)
        {
            if (this.buffNodeSet.ContainsKey(buffId))
            {
                BuffNode node = this.buffNodeSet[buffId];
                node.state = BuffState.Invalid;

                this.buffNodeSet.Remove(buffId);
            }
        }

        public void CalcAllBuffsWithProp(GM_BuffMgr mgr, string propName, FightCalcResult ret)
        {
            foreach (var buf in this.buffNodeSet)
            {
                BuffNode node = buf.Value;
                if (node.state != BuffState.Started)
                {
                    continue;
                }

                mgr.CalcFightBuffWithProp(node.buffId, propName, ret);
            }
        }

        public void OnUpdate(float dt)
        {
            foreach (var key in this.buffNodeSet)
            {
                BuffNode node = key.Value;
                if (node.state == BuffState.Started)
                {
                    node.passedTime += dt;
                    if (node.passedTime >= node.durationTime)
                    {
                        node.state = BuffState.Freezed;
                        node.passedTime = 0;

                        // test
                        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIBuffFreezed);
                    }
                }
                else if (node.state == BuffState.Freezed)
                {
                    node.passedTime += dt;
                    if (node.passedTime >= node.freezeTime)
                    {
                        node.state = BuffState.Ready;
                        node.passedTime = 0;
                        // test
                        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIBuffReady);
                    }
                }
            }
        }
    }



