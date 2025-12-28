

public struct SkillAndBuffComponent
{

    public SkillTimeLine skillTimeLine;
    public BuffTimeLine buffTimeLine;


    public static void Init(CharactorEntity entity)
    {
        entity.uSkillAndBuff.buffTimeLine.Init();
        entity.uSkillAndBuff.skillTimeLine.Init();
    }
}
