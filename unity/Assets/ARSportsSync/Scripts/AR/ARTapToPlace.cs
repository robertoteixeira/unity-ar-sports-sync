using System.Collections.Generic;
using ARSportsSync.Networking;
using ARSportsSync.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARSportsSync.AR
{
    public sealed class ARTapToPlace : MonoBehaviour
    {
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private RealtimePoseClient poseClient;

        private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

        private Transform targetRoot;
        private GameObject spawned;

        public RealtimePoseTarget SpawnedPoseTarget
        {
            get
            {
                return spawned == null ? null : spawned.GetComponentInChildren<RealtimePoseTarget>();
            }
        }

        private void Update()
        {
            if (Input.touchCount == 0)
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began || IsPointerOverUi(touch.fingerId))
            {
                return;
            }

            if (!raycastManager.Raycast(touch.position, Hits, TrackableType.PlaneWithinPolygon))
            {
                return;
            }

            Pose hitPose = Hits[0].pose;

            if (targetRoot == null)
            {
                GameObject rootObject = new GameObject("RealtimeTargetRoot");
                targetRoot = rootObject.transform;
            }

            targetRoot.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            if (spawned == null)
            {
                spawned = Instantiate(targetPrefab, targetRoot);
                spawned.transform.localPosition = UnityEngine.Vector3.zero;
                spawned.transform.localRotation = Quaternion.identity;

                RealtimePoseTarget poseTarget = SpawnedPoseTarget;
                if (poseClient != null && poseTarget != null)
                {
                    poseClient.SetTarget(poseTarget);
                }
            }
        }

        private static bool IsPointerOverUi(int fingerId)
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId);
        }
    }
}