using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr : Singleton<EventMgr>
{
    public delegate void OnEventAction(int type, object udata, object udata2 = null);
    private Dictionary<int, OnEventAction> eventActions = null;

    public void Init() {
        this.eventActions = new Dictionary<int, OnEventAction>();
    }

    public void AddListener(int eventType, OnEventAction onEvent) {
        if (this.eventActions.ContainsKey(eventType)) {
            this.eventActions[eventType] += onEvent;
        }
        else {
            this.eventActions[eventType] = onEvent;
        }
    }

    public void RemoveListener(int eventType, OnEventAction onEvent) {
        if (this.eventActions.ContainsKey(eventType)) {
            this.eventActions[eventType] -= onEvent;
        }

        /*if (this.eventActions[eventName] == null) {
            this.eventActions.Remove(eventName);
        }*/
    }

    public void RemoveAllListeners(object target) {
        foreach (var eventType in this.eventActions.Keys) {
            var v = this.eventActions[eventType];
            Delegate[] all = v.GetInvocationList();
            for (int i = 0; i < all.Length; i++) {
                if (all[i].Target == target) {
                    Delegate.Remove(v, all[i]);
                }
            }
        }
    }

    public void Emit(int eventType, object udata, object udata2 = null) {
        if (this.eventActions.ContainsKey(eventType)) {
            if (this.eventActions[eventType] != null) {
                this.eventActions[eventType](eventType, udata, udata2);
            }
        }
    }
}
