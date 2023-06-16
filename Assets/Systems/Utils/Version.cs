using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class VersionEditor: IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        Version.ChangeVersion();
    }
}


#endif


public class Version : MonoBehaviour
{

    public static void ChangeVersion(bool onlyCheck = false)
    {
        //Get and set initial values
        Version.version = PlayerPrefs.GetString("version");
        Version.buildIndex = PlayerPrefs.GetInt("versionIndex");
        Version.lastUpdateBuild = DateTime.Now.ToString("ddMMyy");

        //Check if as new version on day
        if (Version.lastUpdateBuild != PlayerPrefs.GetString("versionDate"))
        {
            Version.buildIndex = 0;
            PlayerPrefs.SetString("versionDate", Version.lastUpdateBuild);
        }

        //Apply the version
        int Index = (int)Mathf.Repeat(Version.buildIndex, Version.buildIds.Length);
        int Index1 = (int)Mathf.Clamp(Version.buildIndex-Version.buildIds.Length, 0, Version.buildIds.Length);
        Version.version = Version.lastUpdateBuild + Version.buildIds[Index] + Version.buildIds[Index1];

        if (!onlyCheck) { Version.buildIndex += 1; }



        PlayerPrefs.SetString("version", Version.version);
        PlayerPrefs.SetString("versionDate", Version.lastUpdateBuild);
        PlayerPrefs.SetInt("versionIndex", Version.buildIndex);
    }

    public static string version = "--";
    public static int buildIndex;
    public static string lastUpdateBuild;

    public static char[] buildIds = new char[] { ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z' };

}
