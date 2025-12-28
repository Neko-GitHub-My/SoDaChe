enum RoomState
{
    Invalid = -1,
    Waiting = 0, // 等待玩家加入，等待玩家游戏Ready准备;
    Ready, // 已经达到可以开局的游戏要求，所有玩家都已经Ready; 随时可以开始;  
    Started, // 游戏正式开始了，大家进入到了游戏环节;
    CheckOut, // 游戏进入结算状态;
    RoundClear, // 结算完成，进入清理状态;
}

enum PlayerInRoomState
{
    Invalid = -1,
    InView = 0, // 玩家正在旁观
    Ready,  //  玩家做小已经准备好了;
    Playing, // 玩家正在游戏;
    CheckOut, // 玩家目前处于结算状态;
    RoundClear, // 玩家目前处于清理状态;
}

