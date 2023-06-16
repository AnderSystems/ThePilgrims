using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CopyAssetPath : Editor
{
    [MenuItem("Assets/Copy full path", priority = 50)]
    public static void CopySelectedPath()
    {
        string path = Application.dataPath.Replace("Assets", AssetDatabase.GetAssetPath(Selection.activeObject));
        GUIUtility.systemCopyBuffer = path;
        //Debug.Log(path);
    }
}
