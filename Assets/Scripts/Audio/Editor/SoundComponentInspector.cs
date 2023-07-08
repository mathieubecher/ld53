using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(SoundComponent))]
public class SoundComponentInspector : Editor
{
    MonoBehaviour eventScript;
    List<string> methodsList;
    SoundComponent soundComponent;

    private void OnEnable()
    {
        soundComponent = target as SoundComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorUtility.IsDirty(target))
        {
            if (soundComponent.eventScript != null)
            {

                eventScript = soundComponent.eventScript;

                FieldInfo[] fields = eventScript.GetType().GetFields(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance);

                Debug.Log($"Found {fields.Length} fields");

                foreach (FieldInfo field in fields)
                {
                    Debug.Log($"Field = {field}");
                }

                int selected = 0;
                string[] options = new string[]
                {
                    "Option1", "Option2", "Option3",
                };
                selected = EditorGUILayout.Popup("Label", selected, options);
            }
        }
    }
}
