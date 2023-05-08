using System;

[Serializable] public enum ActionType
{
    NULL,
    ATTACK,
    GUARD,
    SPECIAL,
    HIT,
    REINFORCED_ATTACK,
    REINFORCED_GUARD,
    PARRY,
    INVULNERABLE_ATTACK
}

[Serializable] public enum ActionEffect
{
    ATTACK,
    TAUNT,
    START_GUARD,
    STOP_GUARD,
    BUFF,
}
[Serializable] public enum ActionStep
{
    ATTACK,
    SPECIAL,
    GUARD,
    REACH_TARGET,
    RETURN_TO_POSITION,
    SPECIAL_ANTICIPATION,
    IDLE,
    START_GUARDING,
    STOP_GUARDING,
    HIT,
}
