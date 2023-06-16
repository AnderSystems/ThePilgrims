using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{


    [System.Serializable]
    public class action
    {
        public string eventName;
        public InputField.EndEditEvent Event;
    }
    [Header("Use 'CallEvent(EventName)' to call some event")]
    [SerializeField] public List<action> actions = new List<action>();

    public action GetActionByName(string name)
    {
        action r = null;
        foreach (var item in actions)
        {
            if (item.eventName == name)
            {
                r = item;
            }
        }
        return r;
    }

    public void CallEvent(string name)
    {
        action r = GetActionByName(name);
        if (r != null)
        {
            r.Event.Invoke(name);
        } else
        {
            Debug.LogError($"Action with name '{name}' not found.");
        }

    }
}
