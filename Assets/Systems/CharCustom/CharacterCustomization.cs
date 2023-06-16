using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    [System.Serializable]
    public class objectClass
    {
        public string name;
        public int Selection;
        [Space]
        public List<Renderer> objects;
        public bool Search;
    }

    [System.Serializable]
    public class wardrobe
    {
        [SerializeField]
        public List<objectClass> items = new List<objectClass>();
    }
    [SerializeField]
    public wardrobe Wardrobe = new wardrobe();

    public bool Randomize;
    public bool RandomizeOnStart;


    [TextArea()] public string JSONdata;
    public bool Export;
    public bool Import;

    public void Apply()
    {
        foreach (var c in Wardrobe.items)
        {
            for (int i = 0; i < c.objects.Count; i++)
            {
                if (i == c.Selection)
                {
                    c.objects[i].gameObject.SetActive(true);
                }
                else
                {
                    Destroy(c.objects[i].gameObject);
                }
            }
        }
    }
    public void PerformRandomize()
    {
        foreach (var c in Wardrobe.items)
        {
            c.Selection = Random.Range(0, c.objects.Count);
        }
        UpdateSelection();
    }
    public void UpdateSelection()
    {
        foreach (var c in Wardrobe.items)
        {
            for (int i = 0; i < c.objects.Count; i++)
            {
                if (i == c.Selection)
                {
                    c.objects[i].gameObject.SetActive(true);
                } else
                {
                    c.objects[i].gameObject.SetActive(false);
                }
            }
        }
        JSONdata = JsonUtility.ToJson(Wardrobe, true);
    }
    public void SearchObjects()
    {
#if UNITY_EDITOR
        foreach (var c in Wardrobe.items)
        {
            if (c.Search)
            {
                if (UnityEditor.EditorUtility.DisplayDialogComplex("Search type", "Search mode", "Repalce", "Cancel", "Add") == 0)
                {
                    c.objects = new List<Renderer>();
                }

                Renderer[] rrr = GetComponentsInChildren<Renderer>(true);
                foreach (var r in rrr)
                {
                    if (r.name.Contains(c.name) && !c.objects.Contains(r))
                    {
                        c.objects.Add(r);
                    }
                }

                c.Search = false;
            }
        }
        UpdateSelection();
#endif
    }

    //Import-Export
    public void ExportData(string fullPath)
    {
        File.WriteAllText(fullPath, JSONdata);
    }
    public void ExportData(string path, string file)
    {
        ExportData(Path.Combine(path, file));
    }
    public void ImportDataJSON(string JSON)
    {
        wardrobe ww = JsonUtility.FromJson<wardrobe>(JSON);
        for (int i = 0; i < Wardrobe.items.Count; i++)
        {
            Wardrobe.items[i].Selection = ww.items[i].Selection;
        }

        UpdateSelection();
    }
    public void ImportData(string Path)
    {
        ImportDataJSON(File.ReadAllText(Path));
    }

    void OnEnable()
    {
        if (RandomizeOnStart)
        {
            PerformRandomize();
        }
    }
    public void OnValidate()
    {
        UpdateSelection();
        SearchObjects();

        if (Randomize)
        {
            PerformRandomize();
            Randomize = false;
        }
        if (Export)
        {
            string p = UnityEditor.EditorUtility.SaveFilePanelInProject("Export Character", "Character", "char", "Export this character");
            if (p != "")
            {
                ExportData(p);
            }
            Export= false;
        }
        if (Import)
        {
            string p = UnityEditor.EditorUtility.OpenFilePanel("Import Character", Application.dataPath, "char");

            if (p != "")
            {
                ImportData(p);
            }
            Import = false;
        }
    }
}
