using System;

[Serializable] public enum ActionType
{
    NULL,
    ATTACK,
    GUARD,
    BUFF,
    HIT,
}

[Serializable] public enum ActionEffect
{
    MELEE_ATTACK,
    MAGIC_ATTACK,
    DISTANCE_ATTACK,
    TAUNT,
    START_GUARD,
    BUFF,
    END_GUARD,
    HEAL
}
