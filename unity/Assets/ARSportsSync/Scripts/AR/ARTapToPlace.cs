using System.Collections.Generic;
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
        [SerializeField] private Transform contentRoot;

        private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();
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

            if (spawned == null)
            {
                Transform parent = contentRoot == null ? null : contentRoot;
                spawned = Instantiate(targetPrefab, hitPose.position, hitPose.rotation, parent);
            }
            else
            {
                spawned.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
        }

        private static bool IsPointerOverUi(int fingerId)
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId);
        }
    }
}