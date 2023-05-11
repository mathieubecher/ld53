using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CharacterAudio : MonoBehaviour
{
    public CharacterSpriteEvent charEvents;
    private SoundComponent m_soundComponent;

    public EventReference onAttackSfx;
    public EventReference onAttackVo;
    public EventReference onGuardSfx;
    public EventReference onGuardVo;
    public EventReference onBuffSfx;
    public EventReference onBuffVo;
    public EventReference onBuffAnticipSfx;
    public EventReference onActionEndEvent;
    public EventReference onBlockedEvent;
    public EventReference onBlockSuccessEvent;
    public EventReference onDamageInflictedEvent;
    public EventReference onDamageReceivedEvent;
    public EventReference onDieEvent;
    public EventReference onGuardBreakSfx;
    public EventReference onGuardBreakVo;
    public EventReference onHealedEvent;
    public EventReference onHitEvent;
    public EventReference onMoveStartEvent;
    public EventReference onMoveEndEvent;
    public EventReference onNewActionReceived;
    public EventReference onFootstepFoley;

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
        m_soundComponent.PlaySound(onFootstepFoley);
    }

    private void OnActionStep(ActionStep _step)
    {
        switch (_step)
        {
            case ActionStep.IDLE:
                break;
            case ActionStep.START_GUARDING:
                m_soundComponent.PlayMultipleSounds(new EventReference[] { onGuardSfx, onGuardVo });
                break;
            case ActionStep.STOP_GUARDING:
                break;
            case ActionStep.ATTACK:
                m_soundComponent.PlayMultipleSounds(new EventReference[] { onAttackSfx, onAttackVo });
                break;
            case ActionStep.SPECIAL:
                m_soundComponent.PlayMultipleSounds(new EventReference[] { onBuffSfx, onBuffVo });
                m_soundComponent.StopSound(onBuffAnticipSfx);
                break;
            case ActionStep.REACH_TARGET:
                break;
            case ActionStep.RETURN_TO_POSITION:
                break;
            case ActionStep.SPECIAL_ANTICIPATION:
                m_soundComponent.PlaySound(onBuffAnticipSfx);
                break;
            case ActionStep.HIT:
                break;
            default:
                return;
        }
    }

    private void OnNewActionReceived(ActionType _type)
    {
        m_soundComponent.PlaySound(onNewActionReceived);
    }

    private void OnMoveStart()
    {
        //m_soundComponent.PlayMutlipleSounds(new EventReference[] { onAttackSfx, onAttackVo });
    }

    private void OnMoveEnd()
    {
        //m_soundComponent.PlayMutlipleSounds(new EventReference[] { onAttackSfx, onAttackVo });
    }

    private void OnStun()
    {
        m_soundComponent.PlaySound(onHitEvent);
    }

    private void OnHealed()
    {
        m_soundComponent.PlaySound(onHealedEvent);
    }

    private void OnGuardBreak()
    {
        m_soundComponent.PlaySound(onGuardBreakSfx);
    }

    private void OnDie()
    {
        m_soundComponent.PlaySound(onDieEvent);
    }

    private void OnDamageReceived()
    {
        m_soundComponent.PlaySound(onDamageReceivedEvent);
        m_soundComponent.StopSound(onBuffAnticipSfx);
    }

    private void OnDamageInflicted()
    {
        m_soundComponent.PlaySound(onDamageInflictedEvent);
    }

    private void OnBlockSuccess()
    {
        m_soundComponent.PlaySound(onBlockSuccessEvent);
    }

    private void OnBlocked()
    {
        m_soundComponent.PlaySound(onBlockedEvent);
    }

    private void OnActionEnd(ActionType _type)
    {
        m_soundComponent.PlaySound(onActionEndEvent);
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
