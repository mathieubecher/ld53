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
    [SerializeField] private GameObject m_overlayPrefab;

    public Player player => m_player;
    public List<NPC> npcs => m_npcs;

    private bool m_startFight = false;
    private Transform m_currentOverlay;
    public void Awake()
    {
        m_timelineManager = GetComponent<TimelineManager>();
        m_characterSpriteManager = GetComponent<CharacterSpriteManager>();
        m_actionSpellsManager = GetComponent<ActionSpellsManager>();
        m_npcs = new List<NPC>();
    }

    public void OnEnable()
    {
        m_timelineManager.ResetWidth();

        foreach (var NPCData in m_NPCToSpawn)
        {
            NPC npc = new NPC(NPCData);
            m_npcs.Add(npc);
        }
        
        m_player = new Player(m_playerToSpawn);
        m_actionSpellsManager.Init(m_timelineManager.width - 5.0f);

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
        foreach (NPC npc in m_npcs)
        {
            npc.StopFight();
        }

        m_player.StopFight();
        
        m_actionSpellsManager.StopFight();
    }

    public void Update()
    {
        if(m_startFight) m_player.Update();
        if(!ControlsManager.selectedActionSpellButton)
        {
            m_currentOverlay.gameObject.SetActive(false);
            m_arrow.gameObject.SetActive(false);
        }
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_descriptionText.transform.parent.parent as RectTransform, Mouse.current.position.value, FindObjectOfType<Canvas>().worldCamera, out localPoint))
        {
            var parent = m_descriptionText.transform.parent;
            
            parent.localPosition = localPoint;
            string description = "";

            description = m_actionSpellsManager.GetDescription();
            if (description == "")
            {
                foreach (var npc in m_npcs)
                {
                    description = npc.GetHoverActionDescription();
                    if (description != "") break;
                }
            }

            if (description == "")
            {
                description = m_player.GetHoverActionDescription();
            }
            m_descriptionText.text = description;
            parent.gameObject.SetActive(description != "");

            m_descriptionText.ForceMeshUpdate();
            float textHeight = m_descriptionText.textBounds.size.y;
            ((RectTransform)m_descriptionText.transform.parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight + 20.0f);
        }
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
    
    public string GetSelectedSpellDescription(ActionSpell _actionSpell)
    {
        foreach (var npc in m_npcs)
        {
            string description = npc.GetSelectedSpellDescription(_actionSpell);
            if (description != "") return description;
        }

        return "";
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
        if (m_player.faction == _faction)
        {
            m_player.timeline.AddAura(new Aura(m_player.timeline.elapsedTime, _duration, _value, 1.0f, false));   
        }
        foreach (var npc in m_npcs)
        {
            if (npc.faction == _faction)
            {
                npc.timeline.AddAura(new Aura(m_player.timeline.elapsedTime, _duration, _value, 1.0f, false));  
            }   
        }
    }

    public void AttackDeBuffFaction(string _otherFaction, float _value, float _duration)
    {
        if (m_player.faction != _otherFaction)
        {
            m_player.timeline.AddAura(new Aura(m_player.timeline.elapsedTime, _duration, 1.0f, _value, false));   
        }
        foreach (var npc in m_npcs)
        {
            if (npc.faction != _otherFaction)
            {
                npc.timeline.AddAura(new Aura(m_player.timeline.elapsedTime, _duration, 1.0f, _value, false));  
            }   
        }
    }
}
