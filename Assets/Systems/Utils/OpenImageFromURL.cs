using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OpenImageFromURL : MonoBehaviour
{
    public Image Output;
    public TextMeshProUGUI view;

    public void GetTextureFromURI(string URI)
    {
        view.text = URI;
        StartCoroutine(GetTexture(URI));
    }

    public static Sprite CreateSpriteFromTexture(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f));
    }

    IEnumerator GetTexture(string uri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Output.sprite = CreateSpriteFromTexture(((DownloadHandlerTexture)www.downloadHandler).texture);
        }
    }
}
