

public enum EventType { 
    Invalid = -1,
    Internal = 0,  // 内部事件[0 ~ 9999)
    UI = 10000,
    Net = 20000,
    AI = 30000,
    Logic = 40000,
    // ...
}

public enum GMEvent {
    Invalid = -1,

    UIGuestLogin = 1,
    UISelectRole,
    UIProgress,
    UILoadingEnd,

    UIRegisterUser,
    UIShowRegUserInfo,
    UIUserLogin,
    UIShowUserLoginInfo,
    UIGuestUpgrade,
    UIShowGuestUpgradeInfo,
    UISyncAccountInfo,
    UISyncPlayerInfo,

    UIRecvLoginBonues,
    UIShowLoginBonuesInfo,
    

    UIPullingBonuesData,
    UIShowBonuesList,
    UIRecvBonues,
    UIFlushBonuesList,

    UIPullingTaskData,
    UIShowTaskList,

    UIPullingMailMsg,
    UIShowMailMsgList,
    UIUpdateMailMsgStatus,
    UIFlushMailMsgList,

    UIPullingRank,
    UIShowRankList,

    UIPullingPackData,
    UIShowBackpackList,

    UIExchangeProduct,
    UIShowExchangeProduct,

    UIEnterGameScene,
    UIExitLogicServer,

    UITestGetGoods,
    UITestUpdateGoods,
    UITestEchoLogicServer,

    UIBuffStarted,
    UIBuffFreezed,
    UIBuffReady,
    UITouchMap,
    UIJoyStick,

    EnterLogicServer,
    ShowLogicEchoInfo,
    EnterLogicServerReturn,

    UserExitLogicServerReturn,
    
    UserSitdown,
    UserSitdownReturn,

    UserStandup,
    UserStandupReturn,

    UserEnterRoomReturn,
    UserArrivedSeatReturn,
    UserExitSeatReturn,


    UserTalkMsg,
    UserTalkMsgReturn,

    PlayerIsReady,
    PlayerIsReadyReturn,

    PlayerOpt,
    PlayerOptReturn,

    PlayerSpawn,
    PlayerSpawnReturn,

    NavToDst,
    NavToDstReturn,

    NavInDir,
    NavInDirReturn,

    RoundIsReadyReturn,
    RoundIsStartedReturn,
    RoundCheckOutReturn,
    RoundClearReturn,
    ReconnRoomReturn,
    PlayerEscapeReturn,
    FrameSyncDataReturn,

    PlayerEnterAOIReturn,
    PlayerLeaveAOIReturn,

    SyncCharactorStatusReturn,
    LostHpReturn,

    StartSkill,
    StartSkillReturn,


    StartBuff,
    StartBuffReturn,
    
}

public enum SceneId {
    Invalid = 0,
    HomeScene = 1,
    WorldScene = 7,
}
enum Module
{
    /*...*/

    /** 登录 */
    AUTH = 101,
    /** 玩家 */
    PLAYER = 102,
    /** 场景 */
    SCENE = 103,
    /** 活动 */
    ACTIVITY = 104,
    /** 技能 */
    SKILL = 105,
    /** 聊天 */
    CHAT = 106,

    // ------------------跨服业务功能模块（107开始）---------------------
    /** 跨服天梯 */
    LADDER = 107,
}

public enum Cmd
{
    // 游客登录
    eGuestLoginReq = 1,
    eGuestLoginRes,

    // 用户名注册,
    eRegisterUserReq,
    eRegisterUserRes,

    // 用户登录
    eUserLoginReq,
    eUserLoginRes,

    // 游客账号升级
    eGuestUpgradeReq,
    eGuestUpgradeRes,

    // 拉取玩家游戏数据
    ePullingPlayerDataReq,
    ePullingPlayerDataRes,

    // 领取每日登录奖励
    eRecvLoginBonuesReq,
    eRecvLoginBonuesRes,
    // end

    // 拉取玩家的奖励数据
    ePullingBonuesListReq,
    ePullingBonuesListRes,

    // 领取玩家的奖励
    eRecvBonuesReq,
    eRecvBonuesRes,

    // 玩家选择角色
    eSelectPlayerReq,
    eSelectPlayerRes,

    // 拉取玩家的任务数据
    ePullingTaskListReq,
    ePullingTaskListRes,

    // 拉取玩家的邮件消息
    ePullingMailMsgReq,
    ePullingMailMsgRes,

    // 玩家更新邮件消息状态
    eUpdateMailMsgReq,
    eUpdategMailMsgRes,

    // 玩家拉取排行
    ePullingRankReq,
    ePullingRankRes,

    // 玩家拉取背包数据
    ePullingPackDataReq,
    ePullingPackDataRes,

    // 玩家交易兑换
    eExchangeProductReq,
    eExchangeProductRes,

    // 玩家获取逻辑服实例Id
    eEnterLogicServerReq,
    eEnterLogicServerRes,

    // 玩家离开逻辑服实例
    eExitLogicServerReq,
    eExitLogicServerRes,

    // 玩家的坐下
    eSitdownReq,
    eSitdownRes,

    // 玩家站起
    eStandupReq,
    eStandupRes,

    // 玩家发送聊天消息
    eTalkMsgReq,
    eTalkMsgRes,

    // 玩家准备好
    ePlayerIsReadyReq,
    ePlayerIsReadyRes,

    // 玩家的游戏操作
    ePlayerOptReq,
    ePlayerOptRes,

    // 服务器主动通知,玩家进入房间
    eEnterRoomRes,
    eUserArrivedSeatRes,
    eUserExitSeatRes,
    eRoundReadyRes,
    eRoundStartedRes,
    eRoundCheckOutRes,
    eRoundClearRes,
    eReconnRoomRes,
    ePlayerEscapeRes,
    eFrameSyncDataRes, // 帧同步，同步所有的玩家操作到客户端;

    // 开放世界的命令
    // 玩家出生的请求;
    ePlayerSpawnReq,
    ePlayerSpawnRes,

    // 游戏目的地导航
    eNavToDstReq,
    eNavToDstRes,

    // 游戏摇杆方向导航
    eNavInDirReq,
    eNavInDirRes,

    eStartSkillReq,
    eStartSkillRes,

    eStartBuffReq,
    eStartBuffRes,

    // 开放世界: 服务器主动通知
    eEnterAOIRes,
    eLeaveAOIRes,
    eSyncCharactorRes,
    eLostHpRes,

    // 测试
    eTestGetGoodReq,
    eTestGetGoodRes,

    // 测试更新背包物品,正式项目不要直接开放出来
    eTestUpdateGoodsReq,
    eTestUpdateGoddsRes,

    // 测试游戏逻辑服通讯命令
    eTestLogicCmdEchoReq,
    eTestLogicCmdEchoRes,

}

public enum QuitReason
{
    PlayerQuit = 1,
    ForceQuit = 2,
    DisconnectQuit = 3,

    // ...
}

public enum Respones
{
    OK = 1,


    SystemErr = -100,
    UserIsFreeze = -101,
    UserIsNotGuest = -102,
    InvalidParams = -103,
    UnameIsExist = -104,
    UnameOrUpwdError = -105,
    InvalidOpt = -106,
    PlayerIsNotExist = -107,
    AccountIsNotLogin = -108,
    PlayerIsFreeze = -109,
    AccountIsNotExist = -110,
    MoneyIsNotEnough = -111,
    LogicServerIsBusy = -112,
    InOtherLogicServerInst = -113,
    LogicServerInstIsNotExist = -114,
    UserIsPlaying = -115,
    // ...
}


public enum Channal
{
    InvalidChannal = -1,
    SelfChannal = 0,
    DouYin,
    IosAppStore,
    // ...
}

public enum RankType
{
    WorldCoin = 100001,

    // ...

}

public enum ServerType
{
    RoomWithRule = 1, // 房间模式+RoomMgr服务模式,代表有棋牌,卡牌,格斗，回合制等;
    OpenWithMapWorld = 2, // 开放世界+GameWorld服务; 代表有 RPG, ARPG, MMORPG
    IndepOrRoomWithMapWorld = 3, // 副本 + 房间模式 + GameWorld服务，代表有Moba，游戏副本等
}






