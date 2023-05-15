using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineAction : MonoBehaviour
{
    [SerializeField] private CharacterActionData m_data;
    [SerializeField] private Character m_character;
    [SerializeField] private Image m_colorImage;
    [SerializeField] private Image m_iconeImage;
    private ActionType m_actionType;
    private float m_timePosition;
    private float m_duration;
    private TimeLine m_parent;
    private string m_description;
    [HideInInspector] public bool played = false;
    [HideInInspector] public bool stop = false;

    private Sprite m_icone;
    private Color m_color;
    public ActionType type => m_actionType;
    public float timePosition => m_timePosition;
    public float duration => m_duration;

    public string description => m_description;

    private int m_currentStep = -1;

    public void SetActionData(CharacterActionData _data)
    {
        m_actionType = _data.actionType;
        m_description = _data.description;
        if (_data.actions.Count == 0)
        {
            if(!m_data ) Debug.LogError(_data.actionType + " has no action steps!");
            return;
        }
        m_data = _data;
        
    }
    
    public void SetTimeline(TimeLine _timeline)
    {
        m_parent = _timeline;
        
    }

    public void SetIcone(Sprite _icone)
    {
        if (duration <= 0.5f)
        {
            m_iconeImage.gameObject.SetActive(false);
            return;
        }
        m_icone = _icone;
        m_iconeImage.gameObject.SetActive(m_icone);
        m_iconeImage.sprite = m_icone;
    }
    
    public void SetColor(Color _color)
    {
        m_color = _color;
        m_colorImage.color = m_color;
    }
    public void SetSize(float _size)
    {
        RectTransform rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, _size);
    }
    
    public void SetTimePosition(float _timePosition)
    {
        m_timePosition = _timePosition;
    }
    public void SetDuration(float _duration)
    {
        m_duration = _duration;
    }

    public void PlayAction(Character _character)
    {
        m_character = _character;
        m_currentStep = 0;
        m_character.PlayActionStep(m_data.actions[m_currentStep].steps, m_data.actions[m_currentStep].numberOfCells / (float)m_parent.cellsPerUnit);
    }

    private void Update()
    {
        if (m_data && m_currentStep >= 0 && m_currentStep < m_data.actions.Count)
        {
            float time = m_timePosition;
            for (int i = 0; i <= m_currentStep; ++i)
            {
                time += m_data.actions[i].numberOfCells / (float)m_parent.cellsPerUnit;
            }

            if (m_parent.elapsedTime >= time)
            {
                ++m_currentStep;
                if (m_currentStep < m_data.actions.Count)
                {
                    var nextStep = m_data.actions[m_currentStep];
                    m_character.PlayActionStep(nextStep.steps, nextStep.numberOfCells / (float)m_parent.cellsPerUnit);
                }
            }
        }
        
    }

    public void Break(Color _color)
    {
        if (type == ActionType.GUARD || type == ActionType.GUARD_GUARD || type == ActionType.GUARD_ATTACK) 
            SetColor(_color);
    }

}
