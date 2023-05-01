using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Transform m_bar;

    public void SetLifeRatio(float _lifeRatio)
    {
        m_bar.localScale = new Vector3(_lifeRatio, 1.0f, 1.0f);
    }
}
