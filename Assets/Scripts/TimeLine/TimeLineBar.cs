using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeLineBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;

    public void SetText(String _text)
    {
        m_text.text = _text;
    }
}
