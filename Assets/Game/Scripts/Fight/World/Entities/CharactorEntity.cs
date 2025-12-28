public class BaseEntity
{
    public int worldId;

    public TransformComponent uTransform;

    public CharactorInfoComponent uInfo;

    public NavComponent uNav;

    public StatusComponent uStatus;

    public SkillAndBuffComponent uSkillAndBuff;

    public AStarComponent uAStarNav;

    public RVOComponent uRvo;

    public FrameSyncComponent uFrameSync;

    public PropsComponent uProps; // 单机，帧同步才需要的角色属性组件

    // .... 其它的一些数据

}


public class CharactorEntity : BaseEntity
{
    public AnimComponent uAnim;
    // public PropsComponent uProps;
    // .... 其它的一些数据

}


public class CharactorEntity2D : BaseEntity
{

    public Anim2DComponent uAnim;
    // public PropsComponent uProps;
    // .... 其它的一些数据
}
