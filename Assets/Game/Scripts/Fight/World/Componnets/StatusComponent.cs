public enum CharactorStatus
{
    Invalid = -1,
    Idle = 0,
    Run,
    Dance,
    Attack,
    Died,
}

public struct StatusComponent
{
    public int status;
}

