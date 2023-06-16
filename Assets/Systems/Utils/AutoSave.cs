#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEditor;

[ExecuteAlways()]
[FilePath("AnderSystems/AutoSave", FilePathAttribute.Location.PreferencesFolder)]
public class AutoSaveAndBackup : ScriptableSingleton<AutoSaveAndBackup>
{
    [Tooltip("Use timed auto save?")] public bool _UseAutoSave = true;
    [Tooltip("Time to save (in seconds)")] public int _AutoSaveTime = 300;
    [Tooltip("Max number of files")] public int _MaxNumberOfFiles = 2;
    [Tooltip("Save on editor reompile")] public bool _SaveOnRecompile = true;
    public static int saveTime { get { return AutoSaveAndBackup.instance._AutoSaveTime; } }
    public static int maxNumberOfFiles { get { return AutoSaveAndBackup.instance._MaxNumberOfFiles; } }



    /// <summary>
    /// Save scene as copy on same folder but with name "SceneName_backup(x)"
    /// </summary>
    /// <param name="Save">Scene to save</param>
    /// <returns></returns>
    public static string SaveAsBackup(UnityEngine.SceneManagement.Scene Save)
    {
        int fileCount = 0;

        string startPath = Save.path;
        string finalPath = Save.path.Replace(".unity", "_backup.unity");

        for (int i = 0; i < AutoSaveAndBackup.instance._MaxNumberOfFiles - 1; i++)
        {
            string generatedPath = finalPath.Replace(".unity", "(" +  fileCount + ").unity");
            string searchPath = (Application.dataPath + generatedPath.Replace("Assets/","/")).Replace("backup(0).unity","backup.unity");
            Debug.Log(searchPath);
            if (File.Exists(searchPath))
            {
                fileCount = i + 1;
            }
        }

        if (fileCount > maxNumberOfFiles)
        {
            fileCount = 0;
        }

        if (fileCount != 0)
        {
            finalPath = finalPath.Replace(".unity", "(" + fileCount + ").unity");
        }


        bool r = EditorSceneManager.SaveScene(Save, finalPath, false);
        EditorSceneManager.OpenScene(startPath);

        if (r)
        {
            return finalPath;
        } else
        {
            return "<ERROR>" + finalPath;
        }


    }
    public void Modify()
    {
        Save(true);
    }
    public void Save()
    {
        Save(true);
    }
}


class AutoSaveProvider : SettingsProvider
{
    SerializedObject m_SerializedObject;

    SerializedProperty m_UseAutoSave;
    SerializedProperty m_AutoSaveTime;
    SerializedProperty m_MaxNumberOfFiles;
    SerializedProperty m_SaveOnRecompile;
    public static float nextSave = 0;



    public AutoSaveProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        AutoSaveAndBackup.instance.Save();
        m_SerializedObject = new SerializedObject(AutoSaveAndBackup.instance);
        m_UseAutoSave = m_SerializedObject.FindProperty("_UseAutoSave");
        m_AutoSaveTime = m_SerializedObject.FindProperty("_AutoSaveTime");
        m_MaxNumberOfFiles = m_SerializedObject.FindProperty("_MaxNumberOfFiles");
        m_SaveOnRecompile = m_SerializedObject.FindProperty("_SaveOnRecompile");
    }

    public override void OnGUI(string searchContext)
    {
        float timeToSave = nextSave - (float)EditorApplication.timeSinceStartup;

        if (EditorApplication.timeSinceStartup > nextSave || GUILayout.Button("Create backup!"))
        {
            string saved = AutoSaveAndBackup.SaveAsBackup(EditorSceneManager.GetActiveScene());

            if (!saved.Contains("<ERROR>"))
            {
                Debug.Log("Scene saved on: ''" + saved + "''.");
            } else
            {
                Debug.LogError("Fail to save scene on ''" + saved.Replace("<ERROR>", "") + "''.");
            }

            nextSave = (float)EditorApplication.timeSinceStartup + AutoSaveAndBackup.saveTime;
        }

            using (CreateSettingsWindowGUIScope())
        {
            m_SerializedObject.Update();
            EditorGUI.BeginChangeCheck();

            //m_Number.floatValue = EditorGUILayout.FloatField(Styles.NumberLabel, m_Number.floatValue);
            //EditorGUILayout.PropertyField(m_Strings, Styles.StringsLabel);

            m_UseAutoSave.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Auto Save", "Use timed auto save?"), m_UseAutoSave.boolValue); GUI.enabled = m_UseAutoSave.boolValue;
            m_AutoSaveTime.intValue = EditorGUILayout.IntField(new GUIContent("Save Time (in Seconds)", "Time to save (in seconds)"), m_AutoSaveTime.intValue); GUI.enabled = true;
            m_SaveOnRecompile.boolValue = EditorGUILayout.Toggle(new GUIContent("Save on Recompile", "Save on editor reompile"), m_SaveOnRecompile.boolValue);
            m_MaxNumberOfFiles.intValue = EditorGUILayout.IntField(new GUIContent("Max number of save files", "Max number of files"), m_MaxNumberOfFiles.intValue);
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Next Save:", timeToSave.ToString() + " Sec");

            if (EditorGUI.EndChangeCheck())
            {
                m_SerializedObject.ApplyModifiedProperties();
                AutoSaveAndBackup.instance.Save();
            }
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateMySingletonProvider()
    {
        var provider = new AutoSaveProvider("AnderSystems/AutoSave", SettingsScope.Project);
        return provider;
    }

    private IDisposable CreateSettingsWindowGUIScope()
    {
        var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
        var type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");
        return Activator.CreateInstance(type) as IDisposable;
    }
}

#endif