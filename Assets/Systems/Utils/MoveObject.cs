using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform target;
    public Transform posA;
    public Transform posB;

    [System.Serializable]public enum extrapolation
    {
        Default, PingPong
    }
    [SerializeField] public extrapolation Extrapolation;

    public float timeline { get; set; }
    public float defaultTransitionTime = 3;
    public float currentTransitionTime { get; set; }
    public bool playOnEnable = true;
    public bool Loop;
    public bool isPlaying { get; set; }

    public void Play(float transitionTime)
    {
        currentTransitionTime = transitionTime;
        isPlaying = true;
    }
    public void Play()
    {
        Play(defaultTransitionTime);
    }
    public void PlayReverse()
    {
        Play(-defaultTransitionTime);
    }
    public void Pause()
    {
        isPlaying = false;
    }
    public void Stop()
    {
        timeline = 0;
        isPlaying = false;
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Stop();
            Play();
        }
    }
    private void LateUpdate()
    {
        if (isPlaying)
        {
            timeline += (currentTransitionTime * Time.deltaTime);


            target.transform.position = Vector3.Lerp(posA.position, posB.position, timeline);
            target.transform.rotation = Quaternion.Lerp(posA.rotation, posB.rotation, timeline);


            if (timeline >= 1 || timeline <= 0)
            {
                if (Extrapolation == extrapolation.Default)
                {
                    timeline = 0;
                }

                if (Extrapolation == extrapolation.PingPong)
                {
                    currentTransitionTime *= -1;
                }

                if (!Loop)
                {
                    isPlaying = false;
                }
            }
        }
    }

    bool editor_perform;
    public void OnDrawGizmos()
    {
#if UNITY_EDITOR

        if (UnityEditor.Selection.activeGameObject == posA.gameObject)
        {
            target.transform.position = posA.transform.position;
            target.transform.rotation = posA.transform.rotation;
            editor_perform = true;
        } else
        {
            if (UnityEditor.Selection.activeGameObject == posB.gameObject)
            {
                target.transform.position = posB.transform.position;
                target.transform.rotation = posB.transform.rotation;
                editor_perform = true;
            } else
            {
                if (editor_perform)
                {
                    target.transform.position = posA.transform.position;
                    target.transform.rotation = posA.transform.rotation;
                    editor_perform = false;
                }
            }
        }

#endif
    }
    private void OnValidate()
    {

    }
}
