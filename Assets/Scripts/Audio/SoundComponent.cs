using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundComponent : MonoBehaviour
{
    public Dictionary<EventReference, List<EventInstance>> refInstanceDict = new Dictionary<EventReference, List<EventInstance>>();

    public void PlaySound(EventReference _eventToPlay)
    {
        EventInstance instance = RuntimeManager.CreateInstance(_eventToPlay);
        RuntimeManager.AttachInstanceToGameObject(instance, transform);
        instance.start();

        List<EventInstance> curInstances;
        if (!refInstanceDict.TryGetValue(_eventToPlay, out curInstances))
        {
            curInstances = new List<EventInstance>();
        }
        curInstances.Add(instance);
    }

    public void PlaySound(EventReference _eventToPlay, bool _oneInstanceOnly)
    {
        if (_oneInstanceOnly)
        {
            StopSound(_eventToPlay);
        }
        else
        {
            EventInstance instance = RuntimeManager.CreateInstance(_eventToPlay);
            instance.start();
        }
    }

    public void StopSound(EventReference _eventToStop)
    {

    }
}
