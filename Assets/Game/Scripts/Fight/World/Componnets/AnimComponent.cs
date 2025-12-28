using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimComponent
{
    public GameObject unityObject;

    public int animState;

    public Animation animPlayer;
}

public struct Anim2DComponent
{
    public GameObject unityObject;

    public int animState;

    public int direction; // 2D角色的方向;
    public MovieClip movieClip; // 地图上角色动画的播放组件;
}
