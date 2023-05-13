using System;

[Serializable] public enum ActionType
{
    NULL,
    ATTACK,
    GUARD,
    SPECIAL,
    HIT,
    ATTACK_ATTACK,
    GUARD_GUARD,
    GUARD_ATTACK,
    ATTACK_GUARD,
    SPECIAL_SPECIAL,
    SPECIAL_ATTACK,
    SPECIAL_GUARD,
    GUARD_SPECIAL,
    IDLE,
}

[Serializable] public enum ActionEffect
{
    ATTACK,
    TAUNT,
    START_GUARD,
    STOP_GUARD,
    BUFF,
    ATTACK_MAGIC,
    INTERRUPT,
    POTION,
    

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
    INTERRUPT,
}
