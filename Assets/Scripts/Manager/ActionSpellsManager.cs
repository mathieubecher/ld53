using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ActionSpellsManager : MonoBehaviour
{
    [Serializable] struct SpellForType
    {
        public ActionType type;
        public ActionSpell spell;
    }
    
    [SerializeField] private Transform m_actionSpellParent;
    [SerializeField] private Transform m_garbage;
    [SerializeField] private TextMeshProUGUI m_cooldownText;
    [SerializeField] private float m_buttonHeight = 80.0f;
    [SerializeField] private float m_margin = 10.0f;
    [SerializeField] private float m_animSpeed = 2.0f;
    [SerializeField] private int m_nbActions = 5;
    [SerializeField] private float m_spellCooldown = 1.0f;
    [SerializeField] private List<SpellForType> m_actionSpells;
    
    private float m_cumulHeight = 0.0f;
    private float m_spellCurrentCooldown = 0.0f;
    private List<ActionSpellButton> m_buttons;
    private ActionSpellButton m_previous;
    private bool m_isHover;
    private bool m_start;
    private int m_nextSpellNumber;
    
    public delegate void HoverEvent(bool _isHover);
    public static event HoverEvent OnHover;

    private void OnEnable()
    {
        //ControlsManager.OnSpellInput += OnSpellInput;
    }

    private void OnDisable()
    {
        //ControlsManager.OnSpellInput -= OnSpellInput;
    }
    public void SartFight()
    {
        m_start = true;
    }

    public void StopFight()
    {
        m_start = false;
    }

    public void Init(float _offset)
    {
        m_buttons = new List<ActionSpellButton>();
        m_actionSpellParent.localPosition = new Vector2(-_offset, m_actionSpellParent.localPosition.y);
        m_cumulHeight = 0.0f;
        m_cooldownText.transform.parent.localPosition = Vector3.down * (m_buttonHeight + m_margin) * (m_nbActions);
        var npcs = GameManager.instance.npcs;
        
        for (m_nextSpellNumber = 0; m_nextSpellNumber < m_nbActions; ++m_nextSpellNumber)
        {
            var button = AddButton(npcs[m_nextSpellNumber % npcs.Count].SelectActionSpell());
            m_buttons.Add(button);
        }
        m_spellCurrentCooldown += m_spellCooldown;
    }

    private ActionSpellButton AddButton(ActionType _type)
    {
        SpellForType spellForType = m_actionSpells.Find(x => x.type == _type);
        if (!spellForType.spell.buttonPrefab) return null;

        GameObject button = Instantiate(spellForType.spell.buttonPrefab, m_actionSpellParent);
        ActionSpellButton actionSpellButton = button.GetComponent<ActionSpellButton>();

        button.transform.localPosition += Vector3.down * m_cumulHeight;

        m_cumulHeight += m_buttonHeight + m_margin;
        actionSpellButton.SetActionSpell(spellForType.spell);
        //actionSpellButton.SetActionSpellNumber(number);
        return actionSpellButton;
    }

    private float m_spellPosTimer = 0.0f;
    private int m_upIndex = 0;
    private void Update()
    {            
        ManageSpellButtons();

        if(m_start) m_spellCurrentCooldown -= Time.deltaTime * GameManager.timelineManager.timelineScale;
        
        if (m_spellCurrentCooldown > 0.0f)
        {
            float floorCooldown = math.floor(m_spellCurrentCooldown);
            float cooldownMs = math.floor((m_spellCurrentCooldown - floorCooldown) * 10.0f);
            
            m_cooldownText.text = floorCooldown + "<size=15>."+ cooldownMs +"</size>";
        }
        
        if (m_spellCurrentCooldown <= 0.0f)
        {
            if (m_previous)
            {
                Destroy(m_previous.gameObject);
            }
            ++m_nextSpellNumber;
            var npcs = GameManager.instance.npcs;
            var button = AddButton(npcs[m_nextSpellNumber % npcs.Count].SelectActionSpell());
            button.Reset();
            m_buttons.Add(button);
            m_cumulHeight -= m_buttonHeight + m_margin;
            
            for (int i = m_buttons.Count - 1; i >= 2; --i)
            {
                if (m_buttons[i] == null)
                {
                    m_upIndex = i;
                    m_spellPosTimer = 1.0f;
                    m_buttons.RemoveAt(m_upIndex);
                    break;
                }
            }
            while (m_buttons.Count > m_nbActions)
            {
                m_upIndex = 0;
                m_spellPosTimer = 1.0f;
                if(m_buttons[0])
                {
                    m_previous = m_buttons[0];
                    m_previous.transform.SetParent(m_garbage);
                    
                    if(ControlsManager.selectedActionSpellButton == m_previous)
                    {
                        ControlsManager.instance.SetCurrentActionSpellButton(null);
                    }
                    m_previous.Remove();
                }
                m_buttons.RemoveAt(m_upIndex);
            }

            m_spellCurrentCooldown += m_spellCooldown;
        }
        else if (m_spellCooldown > 0.0f)
        {
            m_spellPosTimer -= Time.deltaTime * m_animSpeed;
            if (m_spellPosTimer < 0)
            {
                if(m_previous) Destroy(m_previous.gameObject);
                m_spellPosTimer = 0.0f;
            }
            if(m_previous) m_previous.transform.localPosition = Vector3.Lerp(Vector3.up * (m_buttonHeight + m_margin), Vector3.zero, m_spellPosTimer);
            for (int i = m_upIndex; i < m_buttons.Count; ++i)
            {
                Vector3 unitPos = Vector3.down * (m_buttonHeight + m_margin);
                if(m_buttons[i]) 
                    m_buttons[i].transform.localPosition = Vector3.Lerp(unitPos * i, unitPos * (i + 1), m_spellPosTimer);
            }
        }

    }


    private void ManageSpellButtons()
    {
        var buttons = m_buttons.OrderBy(x => x.priority).ToList();
        bool isHover = false;
        int i = 1;
        foreach (var button in buttons)
        {
            if (!button) continue;

            if (!isHover && button.IsHover())
            {
                isHover = true;
            }

            button.transform.SetSiblingIndex(i);
            ++i;
        }

        if (m_isHover != isHover)
        {
            OnHover?.Invoke(isHover);
            m_isHover = isHover;
        }
    }


    private void OnSpellInput(int _number)
    {
        if (_number-1 < m_buttons.Count)
        {
            m_buttons[_number-1].Select();
        }
    }

    public void Reset()
    {

        foreach (var button in m_buttons)
        {
            if (!button) continue;
            Destroy(button.gameObject);
        }
        m_buttons = new List<ActionSpellButton>();
        
    }
}

