using System.Collections.Generic;
using UnityEngine;
/**
 * 物体层
 */
class EntityLayer : MonoBehaviour {

    private List<Transform> children = new List<Transform>();

    public void Update()  {
       this.sortZindex(); 
    }

    private void sortZindex()
    {
        this.children.Clear();

        for (int i = 0; i < this.transform.childCount; i++) {
            children.Add(this.transform.GetChild(i));
        }

        // 针对我们的y来进行排序，y大的排在前面啊，先渲染，y小的排在后面，后渲染
        children.Sort((lhs, rhs)=> {
            if (lhs.localPosition.y > rhs.localPosition.y)
            {
                return -1;
            }
            else if (lhs.localPosition.y < rhs.localPosition.y) {
                return 1;
            }
            else if (lhs.localPosition.x > rhs.localPosition.x) { 
                return 1;
            }
            return 0;
        });

        // 排在数组前面的就先绘制，排在数组后面的就绘制;
        // 一次调用这个渲染层级顺序的API，让前面的为小的值，后面的为大的值;
        for (int i = 0; i < children.Count; i++) {
            children[i].SetSiblingIndex(i);
        }
    }
}
