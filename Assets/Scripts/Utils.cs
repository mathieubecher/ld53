using System;

[Serializable] public enum ActionType
{
    NULL,
    ATTACK,
    GUARD,
    BUFF,
    HIT,
    REINFORCED_ATTACK,
    REINFORCED_GUARD,
    PARRY,
    INVULNERABLE_ATTACK
}

[Serializable] public enum ActionEffect
{
    MELEE_ATTACK,
    MAGIC_ATTACK,
    TAUNT,
    START_GUARD,
    BUFF,
}
