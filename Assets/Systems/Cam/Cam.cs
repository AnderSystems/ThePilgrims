using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class Cam_Editor : Editor
{
    [MenuItem("GameObject/Game/Cam")]
    public static void CreateCam()
    {
        Camera cam = UnityEditor.SceneView.lastActiveSceneView.camera;
        Cam.CreateNew(cam.transform.transform.position + (cam.transform.forward * 5), Quaternion.identity, 60);
    }

    [MenuItem("GameObject/Game/OrbitalCam")]
    public static void CreateOrbitalCam()
    {
        Camera cam = UnityEditor.SceneView.lastActiveSceneView.camera;
        OrbitalCam.CreateNewOrbital(cam.transform.transform.position + (cam.transform.forward * 5), Quaternion.identity, 60);
    }
}

#endif


public class Cam : MonoBehaviour
{
    [Tooltip("All tags of camera (use ; to separate each one)")] [SerializeField] [InspectorName("CameraTags")] string m_camTags = "Cam";
    public string[] camTags { get { return m_camTags.Split(";"); } }

    //Static
    public static void SendMessageToAll(string Message,object Contex)
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>())
        {
            item.SendMessage(Message, Contex, SendMessageOptions.DontRequireReceiver);
        }
    }
    public static Cam CurrentCam;
    public Orbit _orbit;
    public static Cam CreateNew(Vector3 pos, Quaternion rot, float FOV)
    {
        Cam r = new GameObject("Cam").AddComponent<Cam>();
        r.transform.position = pos;
        r.transform.rotation = rot;
        r.FieldOfView = FOV;

        return r;
    }

    //Params
    [Range(0.001f, 179f)]
    public float FieldOfView = 60;
    public float DefaultLerp = 5;

    //CameraCollider
    public Transform CameraCollisionEndPoint;
    public LayerMask CollisionLayers;


    public static void Set(Cam cam)
    {
        Set(cam, cam.DefaultLerp);
    }
    public static void Set(Cam cam, float lerp)
    {
        cam.Set(lerp);
    }
    public void Set()
    {
        Set(this);
    }

    public virtual void Set(float Lerp)
    {
        CurrentCam = this;
        renderCam.CurrentLerp = Lerp;
        SendMessageToAll("OnCameraChanged", this);
    }
    public virtual void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(renderCam.UnityCam.aspect, 1.0f, 1.0f));
        Gizmos.DrawFrustum(Vector3.zero, FieldOfView, 0, 3, 1.0f);

        if (_orbit)
        {
            Gizmos.DrawLine(transform.position, _orbit.transform.position);
        }

        Orbit.DrawAllOrbitGizmos(UnityEditor.Selection.activeTransform);

        if (Application.isPlaying)
            return;
        renderCam.PreviewCam(this);
#endif
    }
    public void Blend(Cam camA, Cam camB, float blend, float applyLerp = 0)
    {


        if (applyLerp > 0)
        {
            Vector3 storePos = Vector3.Lerp(camA.transform.position, camB.transform.position, blend);
            Quaternion storeQuat = Quaternion.Lerp(camA.transform.rotation, camB.transform.rotation, blend);
            float StoreFov = Mathf.Lerp(camA.FieldOfView, camB.FieldOfView, blend);

            transform.position = Vector3.Lerp(transform.position, storePos, applyLerp * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, storeQuat, applyLerp * Time.deltaTime);
            FieldOfView = Mathf.Lerp(FieldOfView, StoreFov, 4 * Time.deltaTime);

        } else
        {
            transform.position = Vector3.Lerp(camA.transform.position, camB.transform.position, blend);
            transform.rotation = Quaternion.Lerp(camA.transform.rotation, camB.transform.rotation, blend);
            FieldOfView = Mathf.Lerp(camA.FieldOfView, camB.FieldOfView, blend);
        }
    }
    public void Blend(List<Cam> Cams, float blend, float applyLerp = 0)
    {
        int currentCamIndex = (int)blend;
        float currentBlend = blend - currentCamIndex;
        Cam CamA = Cams[Mathf.Clamp(currentCamIndex - 1,0,9999)];
        Cam CamB = Cams[Mathf.Clamp(currentCamIndex, 0, 9999)];

        Blend(CamA, CamB, currentBlend, applyLerp);
    }


    private void OnValidate()
    {
        if (!_orbit)
        {
            _orbit = GetComponentInParent<Orbit>(); 
        }
    }

}
