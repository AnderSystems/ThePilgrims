using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnderSystems
{
#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
    public class SceneContexMenuEditor : Editor
    {
        static SceneContexMenuEditor()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        public static bool MenuIsOpen { get; set; }
        public static GenericMenu menu = new GenericMenu();
        static void OnSceneGUI(SceneView sceneView)
        {
            SceneView sv = sceneView;
            Event e = Event.current;


            if (e.button == 1 && e.control && !e.shift)
            {
                if (e.type == EventType.MouseDown)
                {
                    menu.ShowAsContext();
                    MenuIsOpen = true;
                }
            } else
            {
                menu = new GenericMenu();
                MenuIsOpen = false;
            }
        }
    }
#endif
}
