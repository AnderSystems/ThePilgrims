using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameDebug : MonoBehaviour
{
    public bool Show = false;
    GameObject GetEditorSelectedObject()
    {
        GameObject r = null;
#if UNITY_EDITOR
        r = UnityEditor.Selection.activeGameObject;
#endif
        return r;
    }

    /// <summary>
    /// Get the main gameDebug
    /// </summary>
    public static gameDebug main { get; set; }

    [System.Serializable] public class debugVar
    {
        public string name;
        public string value;
        public MonoBehaviour scriptTarget;

        public debugVar(string _name, string _value, MonoBehaviour _target)
        {
            name = _name;
            value = _value;
            scriptTarget = _target;
        }
    }
    [System.Serializable] public class debugObject
    {
        public GameObject target;
        public List<debugVar> vars = new List<debugVar>();

        public debugObject(GameObject _target)
        {
            target = _target;
        }
        public debugObject(string name, string value, MonoBehaviour _target)
        {
            target = _target.gameObject;
            vars.Add(new debugVar(name, value, _target));
        }
        public debugVar getVar(string varName, MonoBehaviour _target)
        {
            debugVar r = null;

            foreach (var item in vars)
            {
                if (item.name == varName && item.scriptTarget == _target)
                {
                    r = item;
                }
            }

            return r;
        }
        public void debugVar(string varName, string _value, MonoBehaviour _target)
        {
            debugVar r = getVar(varName, _target);
            if (r == null)
            {
                vars.Add(new gameDebug.debugVar(varName, _value, _target));
            } else
            {
                r.value = _value;
            }

        }
    }
    [SerializeField] public static List<debugObject> DebugObjects = new List<debugObject>();
    [SerializeField] List<debugObject> debugObjectsMirror { get; set; }

    public static debugObject GetByObject(MonoBehaviour search)
    {
        debugObject r = null;

        foreach (var item in DebugObjects)
        {
            if (item.target == search.gameObject)
            {
                r = item;
            }
        }

        return r;
    }
    public static void Log(string var, object value, MonoBehaviour target)
    {
        //check and create Object
        debugObject r = GetByObject(target);

        if (r == null)
        {
            r = new debugObject(var, value.ToString(), target);
            DebugObjects.Add(r);
        }

        //Check and create var
        r.debugVar(var, value.ToString(), target);

        main.debugObjectsMirror = DebugObjects;
    }
    public string DebugText()
    {
        string debugText = "";

        foreach (var objs in DebugObjects)
        {
            if (!objs.target)
            {
                foreach (var Var in objs.vars)
                {
                    debugText += $"\n <b>[{Var.scriptTarget}]</b> {Var.name}: {Var.value}";
                }
            }
        }

        return debugText;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            Show = !Show;
        }
    }
    private void OnGUI()
    {
        if (!Show)
            return;

        string debugText = DebugText();
        GUI.color = Color.black; GUI.Label(new Rect(11, 11, 1920, 1080), debugText);
        GUI.color = Color.white; GUI.Label(new Rect(10,10,1920,1080), debugText);

        foreach (var item in DebugObjects)
        {
            if (item.target)
            {
                string text = "<b>" + item.target.gameObject.name + ":</b>\n";
                for (int i = 0; i < item.vars.Count; i++)
                {
                    text += $"<b>[{item.vars[i].scriptTarget}]: </b> {item.vars[i].name}: {item.vars[i].value}\n";
                }
                //Vector2 pos = RectTransformUtility.WorldToScreenPoint(Camera.current, item.target.transform.position);
                Vector2 pos = Camera.current.WorldToScreenPoint(item.target.transform.position + Vector3.up);
                pos.y = (Screen.height - pos.y) - (item.vars.Count * 16);

                GUI.color = Color.black; GUI.Label(new Rect(pos.x + 1, pos.y + 1, 800, 600), text);
                GUI.color = Color.white; GUI.Label(new Rect(pos.x, pos.y, 800, 600), text);
            }
        }
    }
    public void Awake()
    {
        main = FindObjectOfType<gameDebug>();
    }
}
