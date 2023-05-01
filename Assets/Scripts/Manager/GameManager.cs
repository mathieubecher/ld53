using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [SerializeField] private Player m_player;
    [SerializeField] private List<NPC> m_npcs;
    [SerializeField] public List<string> m_npcNames;
    [SerializeField] public List<string> m_playerNames;
    [SerializeField] private List<ActionSpell> m_actionSpells;
    
    [SerializeField] private CharacterData m_playerToSpawn;
    [SerializeField] private List<CharacterData> m_NPCToSpawn;

    public Player player => m_player;
    public List<NPC> npcs => m_npcs;
    public List<string> npcNames => m_npcNames;
    public List<string> playerNames => m_playerNames;

    private bool m_startFight = false;
    public void Awake()
    {
        m_timelineManager = GetComponent<TimelineManager>();
        m_characterSpriteManager = GetComponent<CharacterSpriteManager>();
        m_actionSpellsManager = GetComponent<ActionSpellsManager>();
        m_npcs = new List<NPC>();
    }

    public void OnEnable()
    {
        NPC.NUMBER_NPC = 0;
        Player.NUMBER_PLAYER = 0;
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
        
        //StartFight();
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
        m_player.OnDestroy();
        m_player = null;
    }

    public void Update()
    {
        if(m_startFight) m_player.Update();
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
}
