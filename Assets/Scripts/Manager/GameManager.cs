using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Serializable] private class DataType
    {
        public ActionType type;
        public Color color;
        public Sprite icone;
    }
    [Serializable] private class CombineType
    {
        public ActionType typeA;
        public ActionType typeB;
        public ActionType result;
    }
    
    #region Singleton
    private static GameManager m_instance;
    private TimelineManager m_timelineManager;
    private CharacterSpriteManager m_characterSpriteManager;
    private ActionSpellsManager m_actionSpellsManager;
    public static GameManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    public static TimelineManager timelineManager
    {
        get => instance.m_timelineManager;
    }
    
    public static CharacterSpriteManager characterSpriteManager => instance.m_characterSpriteManager;
    
    public static ActionSpellsManager actionSpellsManager => instance.m_actionSpellsManager;
    
    public static bool CanCombine(ActionType _typeA, ActionType _typeB)
    {
        return instance.m_combineTypes.Exists(x => x.typeA == _typeA && x.typeB == _typeB);
    }
    
    public static ActionType GetCombinedType(ActionType _typeA, ActionType _typeB)
    {
        return instance.m_combineTypes.Find(x => x.typeA == _typeA && x.typeB == _typeB).result;
    }
    #endregion

    public delegate void EndSceneEvent();
    public static event EndSceneEvent OnWin;
    public static event EndSceneEvent OnLoose;
    
    [SerializeField] private bool m_startFightAtStart = false;
    [SerializeField] private Player m_player;
    [SerializeField] private List<NPC> m_npcs;
    
    [SerializeField] private CharacterData m_playerToSpawn;
    [SerializeField] private List<CharacterData> m_NPCToSpawn;
    [SerializeField] private List<CombineType> m_combineTypes;
    [SerializeField] private Transform m_arrow;
    [SerializeField] private TextMeshProUGUI m_descriptionText;
    [SerializeField] private List<Transform> m_auraEffectIcones;
    [SerializeField] private GameObject m_overlayPrefab;

    public Player player => m_player;
    public List<NPC> npcs => m_npcs;

    private bool m_startFight = false;
    private bool m_isPause = false;
    private Transform m_currentOverlay;
    
    public bool isPlaying => m_startFight;
    public void Awake()
    {
        m_timelineManager = GetComponent<TimelineManager>();
        m_characterSpriteManager = GetComponent<CharacterSpriteManager>();
        m_actionSpellsManager = GetComponent<ActionSpellsManager>();
        m_npcs = new List<NPC>();
    }

    public void OnEnable()
    {
        m_timelineManager.ResetWidth(m_actionSpellsManager.width);

        foreach (var NPCData in m_NPCToSpawn)
        {
            NPC npc = new NPC(NPCData);
            m_npcs.Add(npc);
        }

        m_timelineManager.SpawnVersus();
        m_player = new Player(m_playerToSpawn);
        
        m_actionSpellsManager.Init(-5.0f);

        NPC.OnNPCDead += OnNPCDead;
        Player.OnPlayerDead += OnPlayerDead;
        
        CreateOverlay();

        if(m_startFightAtStart) StartFight();
    }


    public void OnDisable()
    {
        m_actionSpellsManager.Reset();
        for (int i = 0; i < m_npcs.Count; ++i)
        {
            m_npcs[i].OnDestroy();
        }
        m_npcs = new List<NPC>();
        
        NPC.OnNPCDead -= OnNPCDead;
        Player.OnPlayerDead -= OnPlayerDead;
        
        DestroyOverlay();
        
        m_player.OnDestroy();
        m_player = null;
    }

    public void StartFight()
    {
        m_startFight = true;
        foreach (NPC npc in m_npcs)
        {
            npc.StartFight();
        }

        m_player.StartFight();

        m_actionSpellsManager.SartFight();
    }

    private void StopFight()
    {
        m_startFight = false;
        if (!m_isPause)
        {
            foreach (NPC npc in m_npcs)
            {
                npc.StopFight();
            }

            m_player.StopFight();
        
            m_actionSpellsManager.StopFight();
        }
    }
    
    private void PauseFight()
    {
        foreach (NPC npc in m_npcs)
        {
            npc.StopFight();
        }

        m_player.StopFight();
        
        m_actionSpellsManager.StopFight();
    }
    private void ResumeFight()
    {
        foreach (NPC npc in m_npcs)
        {
            npc.ResumeFight();
        }

        m_player.ResumeFight();
        
        m_actionSpellsManager.ResumeFight();
    }

    public void Update()
    {
        if(m_startFight) m_player.Update();
        if(!ControlsManager.selectedActionSpellButton)
        {
            m_currentOverlay.gameObject.SetActive(false);
            m_arrow.gameObject.SetActive(false);
        }
        
        ManageDescription();
    }

    private void OnPlayerDead()
    {
        OnWin?.Invoke();
        StopFight();
    }

    private void OnNPCDead()
    {
        foreach (NPC npc in m_npcs)
        {
            if (!npc.isDead) return;
        }
        OnLoose?.Invoke();
        StopFight();
    }
    
    private void CreateOverlay()
    {
        m_currentOverlay = Instantiate(m_overlayPrefab, m_npcs[0].timeline.transform).transform;
        m_currentOverlay.gameObject.SetActive(false);
        m_arrow.gameObject.SetActive(false);
    }
    private void DestroyOverlay()
    {
        Destroy(m_currentOverlay.gameObject);
    }

    public void DrawHoverAction(ActionSpell _actionSpell)
    {
        foreach (var npc in m_npcs)
        {
            if (npc.TryDrawAction(_actionSpell.type, m_arrow, m_currentOverlay as RectTransform))
            {
                m_currentOverlay.gameObject.SetActive(true);
                m_arrow.gameObject.SetActive(true);
                return;
            }
        }
        m_currentOverlay.gameObject.SetActive(false);
        m_arrow.gameObject.SetActive(false);
    }
    
    public void HealFaction(string _faction, float _value)
    {
        if (m_player.faction == _faction)
        {
            m_player.Heal(_value);   
        }
        foreach (var npc in m_npcs)
        {
            if (npc.faction == _faction)
            {
                npc.Heal(_value);
            }   
        }
    }

    public void AttackBuffFaction(string _faction, float _value, float _duration)
    {
        if (m_player.faction == _faction && !m_player.isDead)
        {
            m_player.timeline.AddAura(m_player.timeline.elapsedTime, _duration, _value, false, false);   
        }
        foreach (var npc in m_npcs)
        {
            if (npc.faction == _faction && !m_player.isDead)
            {
                npc.timeline.AddAura(m_player.timeline.elapsedTime, _duration, _value, false, false);  
            }   
        }
    }

    public void AttackDeBuffFaction(string _otherFaction, float _value, float _duration)
    {
        if (m_player.faction != _otherFaction)
        {
            m_player.timeline.AddAura(m_player.timeline.elapsedTime, _duration, _value, false, false);   
        }
        foreach (var npc in m_npcs)
        {
            if (npc.faction != _otherFaction)
            {
                npc.timeline.AddAura(m_player.timeline.elapsedTime, _duration, _value, false, false);  
            }   
        }
    }
    
    
    
    private void ManageDescription()
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_descriptionText.transform.parent.parent as RectTransform, Mouse.current.position.value,
                FindObjectOfType<Canvas>().worldCamera, out Vector2 localPoint))
        {
            var parent = m_descriptionText.transform.parent;
            parent.localPosition = localPoint + Vector2.left * 20.0f;
            string description = "";

            description = m_actionSpellsManager.GetDescription();
            if (description == "" && !m_actionSpellsManager.IsSelected())
            {
                foreach (var npc in m_npcs)
                {
                    description = npc.GetHoverActionDescription();
                    if (description != "") break;
                }
            }

            if (description == "" && !m_actionSpellsManager.IsSelected())
            {
                description = m_player.GetHoverActionDescription();
            }

            m_descriptionText.text = description;
            
            float timePos;
            bool isOnTimeline = false;
            if (m_player.isMouseInTimeline(out timePos))
            {
                ManageAuraDescription(m_player, description == "" ? -20.0f : 5.0f, timePos);
                isOnTimeline = true;
            }
            else
            {
                foreach (var character in m_npcs)
                {
                    if (character.isMouseInTimeline(out timePos))
                    {
                        ManageAuraDescription(character, description == "" ? -20.0f : 5.0f, timePos);
                        isOnTimeline = true;
                        break;
                    }
                }
            }

            if (!isOnTimeline)
            {
                foreach (var auraDescrion in m_auraEffectIcones)
                {
                    auraDescrion.gameObject.SetActive(false);
                }
            }
                
            m_descriptionText.ForceMeshUpdate();
            float textWidth =  30.0f + description.Length > 10 ? 220 : 60;
            float textHeight = description == "" ? 0f : m_descriptionText.textBounds.size.y + 30.0f;
            
            ((RectTransform)parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);
            ((RectTransform)parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);
        }
    }

    private void ManageAuraDescription(Character _character, float _offset, float _timePos)
    {
        AuraEffect effect = _character.timeline.GetAuraAtTimePos(_timePos);
        float auraOffset = _offset;
        if (effect.attackMultiplier > 1.0f)
        {
            m_auraEffectIcones[0].gameObject.SetActive(true);
            m_auraEffectIcones[0].localPosition = Vector3.up * auraOffset;
            auraOffset += 35f;
        }
        else m_auraEffectIcones[0].gameObject.SetActive(false);
        
        if (effect.attackDeMultiplier < 1.0f)
        {
            m_auraEffectIcones[1].gameObject.SetActive(true);
            m_auraEffectIcones[1].localPosition = Vector3.up * auraOffset;
            auraOffset += 35f;
        }
        else m_auraEffectIcones[1].gameObject.SetActive(false);

        if (effect.taunt)
        {
            m_auraEffectIcones[2].gameObject.SetActive(true);
            m_auraEffectIcones[2].localPosition = Vector3.up * auraOffset;
            auraOffset += 35f;
        }
        else m_auraEffectIcones[2].gameObject.SetActive(false);

        if (effect.invulnerability)
        {
            m_auraEffectIcones[3].gameObject.SetActive(true);
            m_auraEffectIcones[3].localPosition = Vector3.up * auraOffset;
        }
        else m_auraEffectIcones[3].gameObject.SetActive(false);
    }
    public string GetSelectedSpellDescription(ActionSpell _actionSpell)
    {
        foreach (var npc in m_npcs)
        {
            string description = npc.GetSelectedSpellDescription(_actionSpell);
            if (description != "") return description;
        }

        return "";
    }

    public void PauseGame()
    {
        if (!m_startFight) return;
        if (!m_isPause)
        {
            PauseFight();
            m_isPause = true;
        }
    }

    public void ResumeGame()
    {
        if (!m_startFight) return;

        if (m_isPause)
        {
            ResumeFight();
            m_isPause = false;
        }
    }
}
