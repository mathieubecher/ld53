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

    public void PlayMultipleSounds(EventReference[] _eventsToPlay)
    {
        foreach (EventReference eventRef in _eventsToPlay)
        {
            StartSound(eventRef);
        }
    }

    public void PlayMutlipleSounds(EventReference[] _eventsToPlay, bool _oneInstanceOnly)
    {
        foreach (EventReference eventRef in _eventsToPlay)
        {
            if (_oneInstanceOnly)
            {
                StopSound(eventRef);
            }
            else
            {
                StartSound(eventRef);
            }
        }
    }

    private void StartSound(EventReference _ref)
    {
        if (!_ref.IsNull)
        {
            EventDescription eventDescription = RuntimeManager.GetEventDescription(_ref);

            EventInstance instance = RuntimeManager.CreateInstance(_ref);
            RuntimeManager.AttachInstanceToGameObject(instance, transform);
            instance.start();
            instance.release();
        }
    }

    public void StopSound(EventReference _ref)
    {
        if (!_ref.IsNull)
        {
            EventDescription eventDescription = RuntimeManager.GetEventDescription(_ref);
            EventInstance[] instances;
            eventDescription.getInstanceList(out instances);
            foreach (EventInstance instance in instances)
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }
}
