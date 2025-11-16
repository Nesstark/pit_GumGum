using UnityEngine;
using System.Collections.Generic;

public class RagdollPose : MonoBehaviour
{
    [System.Serializable]
    public class BoneData
    {
        public Transform bone;
        public Vector3 localPos;
        public Quaternion localRot;
    }

    public List<BoneData> bones = new List<BoneData>();

    void Awake()
    {
        SaveDefaultPose();
    }

    public void SaveDefaultPose()
    {
        bones.Clear();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            BoneData data = new BoneData();
            data.bone = t;
            data.localPos = t.localPosition;
            data.localRot = t.localRotation;
            bones.Add(data);
        }
    }

    public void ResetToDefaultPose()
    {
        foreach (var b in bones)
        {
            b.bone.localPosition = b.localPos;
            b.bone.localRotation = b.localRot;
        }
    }
}
