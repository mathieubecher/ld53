using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    static Dictionary<EventReference, Bank> m_eventBankDict;
    static HashSet<Bank> loadedBanks;

    public static bool TryLoadBankFromEventRef(EventReference _eventReference)
    {
        bool hasBank;
        if (m_eventBankDict.ContainsKey(_eventReference))
            hasBank = true;
        else
            hasBank = false;
        return hasBank;
    }
}
