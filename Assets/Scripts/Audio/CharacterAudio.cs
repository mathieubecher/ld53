using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CharacterAudio : MonoBehaviour
{
    public CharacterSpriteEvent charEvents;
    private SoundComponent m_soundComponent;
    public bool isPlayer;

    [Header("Sfx")]
    public EventReference attackSfx;
    public EventReference guardSfx;
    public EventReference buffSfx;
    public EventReference buffAnticipSfx;
    public EventReference damageReceivedSfx;
    public EventReference deathSfx;
    public EventReference guardBreakSfx;
    public EventReference stunSfx;
    public EventReference footstepSfx;
    public EventReference blockSfx;

    [Header("Voices")]
    public EventReference attackVo;
    public EventReference attackStartVo;
    public EventReference damageReceivedVo;
    public EventReference damageInflictedVo;
    public EventReference guardVo;
    public EventReference buffVo;
    public EventReference actionReceivedVo;
    public EventReference specialVo;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (charEvents)
        {
            charEvents.OnActionStep += OnActionStep;
            charEvents.OnActionStart += OnActionStart;
            charEvents.OnActionEnd += OnActionEnd;
            charEvents.OnBlocked += OnBlocked;
            charEvents.OnBlockSuccess += OnBlockSuccess;
            charEvents.OnDamageInflicted += OnDamageInflicted;
            charEvents.OnDamageReceived += OnDamageReceived;
            charEvents.OnDie += OnDie;
            charEvents.OnGuardBreak += OnGuardBreak;
            charEvents.OnHealed += OnHealed;
            charEvents.OnStun += OnStun;
            charEvents.OnMoveEnd += OnMoveEnd;
            charEvents.OnMoveStart += OnMoveStart;
            charEvents.OnNewActionReceived += OnNewActionReceived;
            charEvents.OnFootstep += OnFootstep;
        }
        else
        {
            Debug.LogError($"Event component not referenced on {gameObject.name} audio component");
        }
    }

    // Update is called once per frame
    void OnDisable()
    {
        if (charEvents)
        {
            charEvents.OnActionStep -= OnActionStep;
            charEvents.OnActionStart -= OnActionStart;
            charEvents.OnActionEnd -= OnActionEnd;
            charEvents.OnBlocked -= OnBlocked;
            charEvents.OnBlockSuccess -= OnBlockSuccess;
            charEvents.OnDamageInflicted -= OnDamageInflicted;
            charEvents.OnDamageReceived -= OnDamageReceived;
            charEvents.OnDie -= OnDie;
            charEvents.OnGuardBreak -= OnGuardBreak;
            charEvents.OnHealed -= OnHealed;
            charEvents.OnStun -= OnStun;
            charEvents.OnMoveEnd -= OnMoveEnd;
            charEvents.OnMoveStart -= OnMoveStart;
            charEvents.OnNewActionReceived -= OnNewActionReceived;
            charEvents.OnFootstep -= OnFootstep;
        }
        else
        {
            Debug.LogError($"Event component not referenced on {gameObject.name} audio component");
        }
    }

    private void Awake()
    {
        if (!TryGetComponent<SoundComponent>(out m_soundComponent))
            m_soundComponent = gameObject.AddComponent<SoundComponent>();
    }

    public void OnFootstep()
    {
        m_soundComponent.PlaySound(footstepSfx);
    }

    private void OnActionStep(ActionStep _step)
    {
        switch (_step)
        {
            case ActionStep.IDLE:
                break;
            case ActionStep.START_GUARDING:
                m_soundComponent.PlayMultipleSounds(new EventReference[] { guardSfx, guardVo });
                break;
            case ActionStep.STOP_GUARDING:
                break;
            case ActionStep.ATTACK:
                m_soundComponent.PlayMultipleSounds(new EventReference[] { attackSfx, attackVo });
                break;
            case ActionStep.SPECIAL:
                m_soundComponent.PlayMultipleSounds(new EventReference[] { buffSfx, buffVo });
                m_soundComponent.StopSound(buffAnticipSfx);
                break;
            case ActionStep.REACH_TARGET:
                break;
            case ActionStep.RETURN_TO_POSITION:
                break;
            case ActionStep.SPECIAL_ANTICIPATION:
                m_soundComponent.PlaySound(buffAnticipSfx);
                break;
            case ActionStep.HIT:
                break;
            default:
                return;
        }
    }

    private void OnNewActionReceived(ActionType _type)
    {
        if (!isPlayer)
            m_soundComponent.PlaySound(actionReceivedVo);
    }

    private void OnMoveStart()
    {
        //m_soundComponent.PlayMutlipleSounds(new EventReference[] { onAttackSfx, onAttackVo });
        m_soundComponent.PlaySound(attackStartVo);
    }

    private void OnMoveEnd()
    {
        //m_soundComponent.PlayMutlipleSounds(new EventReference[] { onAttackSfx, onAttackVo });
    }

    private void OnStun()
    {
        m_soundComponent.PlaySound(stunSfx);
    }

    private void OnHealed()
    {
        //m_soundComponent.PlaySound(onHealedEvent);
    }

    private void OnGuardBreak()
    {
        m_soundComponent.PlaySound(guardBreakSfx);
    }

    private void OnDie()
    {
        m_soundComponent.PlayMultipleSounds(new EventReference[] { deathSfx, damageReceivedVo });
    }

    private void OnDamageReceived()
    {
        m_soundComponent.PlayMultipleSounds(new EventReference[] { damageReceivedSfx, damageReceivedVo });
        m_soundComponent.StopSound(buffAnticipSfx);
    }

    private void OnDamageInflicted()
    {
        //m_soundComponent.PlaySound(damageInflictedVo);
    }

    private void OnBlockSuccess()
    {
        m_soundComponent.PlaySound(blockSfx);
    }

    private void OnBlocked()
    {

    }

    private void OnActionEnd(ActionType _type)
    {

    }

    private void OnActionStart(ActionType _type)
    {
        /*
        switch (_type)
        {
            case ActionType.GUARD:
                m_soundComponent.PlayMutlipleSounds(new EventReference[] { onGuardSfx, onGuardVo });
                break;

            case ActionType.SPECIAL:
                m_soundComponent.PlayMutlipleSounds(new EventReference[] { onBuffSfx, onBuffVo });
                break;
        }
        */
    }
}
