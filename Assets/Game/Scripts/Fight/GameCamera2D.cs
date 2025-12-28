using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCamera2D : MonoBehaviour
{
    public Camera mapCamera = null;
    private CharactorEntity2D target = null;
    private Vector3 offset;

    private float xmin;
    private float xmax;
    private float ymin;
    private float ymax;

    private void Start() {
        this.mapCamera = this.gameObject.GetComponent<Camera>();


    }

    // spawnPos 坐标相对于我们的mapRoot为原点坐标;
    public void ResetCamera(Vector2 spawnPos, RectTransform mapRoot, 
        float viewWidth, float viewHeight, float mapWidth, float mapHeight) {
        Vector3 localPosition = mapRoot.localPosition;
        localPosition.x += (spawnPos.x);
        localPosition.y += (spawnPos.y);
        localPosition.z = -100;

        this.transform.localPosition = localPosition;

        this.xmin = mapRoot.localPosition.x + viewWidth * 0.5f;
        this.xmax = mapRoot.localPosition.x + mapWidth - viewWidth * 0.5f;
        this.ymin = mapRoot.localPosition.y + viewHeight * 0.5f;
        this.ymax = mapRoot.localPosition.y + mapHeight - viewHeight * 0.5f;

        
        this.target = null;
    }

    public void BindFollowTarget(CharactorEntity2D entity) {
        
        if (entity == null) {
            this.target = null;
            return;
        }
        this.target = entity;

        // offset;  // 距离target offset是多少;, cam = target + offset;
        this.offset = this.transform.localPosition - this.target.uAnim.unityObject.transform.localPosition;
    }

    public void LateUpdate()
    {
        if (this.target != null) {
            Vector3 pos = this.target.uAnim.unityObject.transform.localPosition + this.offset;
            
            pos.x = (pos.x < this.xmin) ? this.xmin : pos.x;
            pos.x = (pos.x > this.xmax) ? this.xmax : pos.x;

            pos.y = (pos.y < this.ymin) ? this.ymin : pos.y;
            pos.y = (pos.y > this.ymax) ? this.ymax : pos.y;

            this.transform.localPosition = pos;

        }
    }
}
