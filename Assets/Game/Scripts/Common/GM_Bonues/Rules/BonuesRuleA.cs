
using Game.Datas.Messages;
using UnityEngine;

[BonuesRule(100000)]
public static class BonuesRuleA {


    [BonuesApplay(-1)]
    public static void CoinBonuesApplayToPlayer(ResRecvBonues bonues) {
        GM_DataMgr.Instance.playerData.ucion += bonues.b1;       
    }

    [BonuesApplay(2)]
    public static void MonneyBonuesApplayToPlayer(ResRecvBonues bonues) {
        GM_DataMgr.Instance.playerData.umoney += bonues.b1;
    }
}



