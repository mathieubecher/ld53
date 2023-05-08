using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundComponent : MonoBehaviour
{
    private Dictionary<EventReference, List<EventInstance>> m_refInstanceDict = new Dictionary<EventReference, List<EventInstance>>();

    public void PlaySound(EventReference _eventToPlay)
    {
        StartSound(_eventToPlay);
    }

    public void PlaySound(EventReference _eventToPlay, bool _oneInstanceOnly)
    {
        if (_oneInstanceOnly)
        {
            StopSound(_eventToPlay);
        }
        else
        {
            StartSound(_eventToPlay);
        }
    }

    private void StartSound(EventReference _ref)
    {
        EventInstance instance = RuntimeManager.CreateInstance(_ref);
        RuntimeManager.AttachInstanceToGameObject(instance, transform);
        instance.start();

        List<EventInstance> curInstances;
        if (!m_refInstanceDict.TryGetValue(_ref, out curInstances))
        {
            curInstances = new List<EventInstance>();
        }
        curInstances.Add(instance);
    }

    public void StopSound(EventReference _eventToStop)
    {
        List<EventInstance> _instances;
        if (m_refInstanceDict.TryGetValue(_eventToStop, out _instances))
        {
            foreach (EventInstance eventInstance in _instances)
            {
                eventInstance.release();
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}
