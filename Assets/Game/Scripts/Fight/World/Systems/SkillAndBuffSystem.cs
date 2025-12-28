using Unity.VisualScripting;

public class SkillAndBuffSystem {
    public static void Update(BaseEntity e, float dt) { 
        e.uSkillAndBuff.skillTimeLine.OnUpdate(dt);
        e.uSkillAndBuff.buffTimeLine.OnUpdate(dt);
    }
}

