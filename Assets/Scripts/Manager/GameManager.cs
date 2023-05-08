using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
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
    
    public static CharacterSpriteManager characterSpriteManager
    {
        get => instance.m_characterSpriteManager;
    }
    
    public static ActionSpellsManager ActionSpellsManager
    {
        get => instance.m_actionSpellsManager;
    }
    #endregion

    public delegate void EndSceneEvent();
    public static event EndSceneEvent OnWin;
    public static event EndSceneEvent OnLoose;
    
    [FormerlySerializedAs("m_startGameAtStart")] [SerializeField] private bool m_startFightAtStart = false;
    [SerializeField] private Player m_player;
    [SerializeField] private List<NPC> m_npcs;
    [SerializeField] private List<ActionSpell> m_actionSpells;
    
    [SerializeField] private CharacterData m_playerToSpawn;
    [SerializeField] private List<CharacterData> m_NPCToSpawn;
    [SerializeField] private Transform m_arrow;
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
        m_actionSpellsManager.Init(m_actionSpells, m_timelineManager.width);

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
    }

    private void StopFight()
    {
        m_startFight = false;
        foreach (NPC npc in m_npcs)
        {
            Debug.Log(npc);
            npc.StopFight();
        }

        Debug.Log(m_player);
        m_player.StopFight();
    }

    public void Update()
    {
        if(m_startFight) m_player.Update();
        if(!ControlsManager.selectedActionSpellButton)
        {
            m_currentOverlay.gameObject.SetActive(false);
            m_arrow.gameObject.SetActive(false);
        }
    }
    
    private void OnPlayerDead()
    {
        OnWin?.Invoke();
        StopFight();
    }

    private void OnNPCDead()
    {
        bool loose = false;
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
            if (npc.TryDrawAction(_actionSpell, m_arrow, m_currentOverlay as RectTransform))
            {
                m_currentOverlay.gameObject.SetActive(true);
                m_arrow.gameObject.SetActive(true);
                return;
            }
        }
        m_currentOverlay.gameObject.SetActive(false);
        m_arrow.gameObject.SetActive(false);
    }
}
