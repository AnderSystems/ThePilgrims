using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IK : MonoBehaviour
{
    public Ped Ped;
    public Transform LookAtCenter;


    [System.Serializable]
    public class sync
    {
        public float LookAtWeight = 1;
        public float LookAtLerp = 5;
        public Vector3 LookAtPos;
    }
    [SerializeField]
    public sync Sync;
   
    public void RPC_SyncIK(string JSON)
    {
        Sync = JsonUtility.FromJson<sync>(JSON);
    }


    private void FixedUpdate()
    {
        //Set
        if (Ped.freezePriority > 0)
        {
            Sync.LookAtWeight = 0;
        }
        else
        {
            Sync.LookAtWeight = 1;
            Vector3 pos = transform.InverseTransformPoint(LookAtCenter.position);

            Sync.LookAtPos = Vector3.Lerp(Sync.LookAtPos, pos, Sync.LookAtLerp * Time.deltaTime);
        }
    }
}
