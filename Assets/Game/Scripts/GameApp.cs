using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using Game.Datas.Excels;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Text;
using Game.Datas.Messages;
using UnityEditor.Experimental.GraphView;


public class GameApp : UnitySingleton<GameApp>
{
    public void Init() {
        
        MessageFactory.Instance.InitMeesagePool();
        MessageDispatcher.Instance.Init();
        
        GMEventProcesserCenter.Instance.Init();


        GM_BuffMgr.Instance.Init(); 
        GM_SkillMgr.Instance.Init();

        GM_DataMgr.Instance.Init();
        GM_EffectMgr.Instance.Init();
    }

    public IEnumerator EnterGame() { // 进入游戏
        Debug.Log("########   EnterGame   #########");
        // SoundMgr.Instance.PlayMusic("Sounds/bgm");
        // this.EnterLoginScene();
        yield break;
    }


    public void EnterLoginScene() {
        GM_DataMgr.Instance.machineId = 0;
        UIMgr.Instance.RemoveAllViews();
        UIMgr.Instance.ShowUIView("GUI/Prefabs/UILogin");
    }

    public void EnterHomeScene() {
        // 进入游戏场景以后，再回来，游戏地图场景没有被删除掉;   
        // end
        GM_DataMgr.Instance.machineId = 0;
        UIMgr.Instance.RemoveAllViews();
        UIMgr.Instance.ShowUIView("GUI/Prefabs/UIHome");
    }

    public void EnterLoadingScene(int nextSceneId) {
        UIMgr.Instance.RemoveAllViews();
        UILoading_UICtrl uiLoading = (UILoading_UICtrl)UIMgr.Instance.ShowUIView("GUI/Prefabs/UILoading");
        uiLoading.SetNextScene(nextSceneId);

        EventMgr.Instance.Emit((int)EventType.UI, (int)GMEvent.UIProgress, 1.0f);
    }

}
