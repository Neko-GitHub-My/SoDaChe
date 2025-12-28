using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyStickDir {
    public float x;
    public float y;

    public void Zero() {
        this.x = 0;
        this.y = 0;
    }
}

public class UIJoyStick : MonoBehaviour {
    private Canvas cs;
    public Transform stick;
    public float maxR = 80;

    private JoyStickDir touchDir = new JoyStickDir();

	// Use this for initialization
	void Awake () {
        this.cs = GameObject.Find("Canvas").GetComponent<Canvas>();
        this.stick.localPosition = Vector2.zero;
        this.touchDir.Zero();
	}

    public void OnStickDrag() {
        Vector2 pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, Input.mousePosition, this.cs.worldCamera, out pos);

        float len = pos.magnitude;
        if (len <= 0) {
            this.touchDir.Zero();
            return;
        }

        // 归一化, 
        this.touchDir.x = pos.x / len; // cos(r)
        this.touchDir.y = pos.y / len; // (sinr) cos^2 + sin ^ 2 = 1;

        if (len >= this.maxR) { // this.maxR / len = x` / x = y` / y;
            pos.x = pos.x * this.maxR / len;
            pos.y = pos.y * this.maxR / len;
        }

        this.stick.localPosition = pos;
        
        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIJoyStick, this.touchDir);
    }

    public void OnStickEndDrag() {
        this.stick.localPosition = Vector2.zero;
        this.touchDir.Zero();

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIJoyStick, this.touchDir);
    }

}
