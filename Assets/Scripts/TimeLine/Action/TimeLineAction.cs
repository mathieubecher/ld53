using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineAction : MonoBehaviour
{
    [SerializeField] private ActionType m_type;
    [SerializeField] private Image m_colorImage;
    [SerializeField] private Image m_iconeImage;
    private float m_timePosition;
    private float m_duration;
    [HideInInspector] public bool played = false;
    [HideInInspector] public bool stop = false;

    private Sprite m_icone;
    private Color m_color;
    public ActionType type => m_type;
    public float timePosition => m_timePosition;
    public float duration => m_duration;


    public void SetActionType(ActionType _type)
    {
        m_type = _type;
    }
    
    public void SetIcone(Sprite _icone)
    {
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

    public void Break()
    {
        if (type != ActionType.GUARD) return;
        SetColor(GameManager.GetColor(ActionType.HIT));
    }

}
