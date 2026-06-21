using System;
using UnityEngine;

namespace ARSportsSync.Runtime
{
    [Serializable]
    public sealed class PoseSnapshot
    {
        public string type;
        public string id;
        public int seq;
        public double serverTimeMs;
        public Vector3Dto position;
        public Vector3Dto rotation;

        public Vector3 Position => position.ToUnityVector3();
        public Quaternion Rotation => Quaternion.Euler(rotation.ToUnityVector3());
    }

    [Serializable]
    public struct Vector3Dto
    {
        public float x;
        public float y;
        public float z;

        public Vector3 ToUnityVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public sealed class PongMessage
    {
        public string type;
        public double clientTimeMs;
        public double serverTimeMs;
    }
}