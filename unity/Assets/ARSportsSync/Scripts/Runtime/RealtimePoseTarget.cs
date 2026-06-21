using UnityEngine;

namespace ARSportsSync.Runtime
{
    public sealed class RealtimePoseTarget: MonoBehaviour
    {
        [SerializeField] private float positionLerpSpeed = 12f;
        [SerializeField] private float rotationLerpSpeed = 14f;
        [SerializeField] private float snapDistance = 1.25f;
        [SerializeField] private bool useLocalSpace = true;

        private bool hasSnapshot;
        private Vector3 targetPosition;
        private Quaternion targetRotation = Quaternion.identity;
        private int lastSeq = -1;

        public int LastSeq => lastSeq;
        public bool HasSnapshot => hasSnapshot;

        public bool ApplySnapshot(PoseSnapshot snapshot)
        {
            if (snapshot == null || snapshot.type != "pose")
            {
                return false;
            }

            if (snapshot.seq <= lastSeq)
            {
                return false;
            }
            
            lastSeq = snapshot.seq;
            targetPosition = snapshot.Position;
            targetRotation = snapshot.Rotation;
            hasSnapshot = true;
            return true;
        }

        private void Update()
        {
            if (!hasSnapshot)
            {
                return;
            }

            Vector3 currentPosition = useLocalSpace
                ? transform.localPosition
                : transform.position;

            float distance = Vector3.Distance(currentPosition, targetPosition);

            if (distance > snapDistance)
            {
                SetPosition(targetPosition);
                SetRotation(targetRotation);
                return;
            }

            float posT = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
            float rotT = 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime);

            SetPosition(Vector3.Lerp(currentPosition, targetPosition, posT));
            SetRotation(Quaternion.Slerp(GetRotation(), targetRotation, rotT));
        }

        private void SetPosition(Vector3 position)
        {
            if (useLocalSpace)
            {
                transform.localPosition = position;
            }
            else
            {
                transform.position = position;
            }
        }

        private Quaternion GetRotation()
        {
            return useLocalSpace ? transform.localRotation : transform.rotation;
        }

        private void SetRotation(Quaternion rotation)
        {
            if (useLocalSpace)
            {
                transform.localRotation = rotation;
            }
            else
            {
                transform.rotation = rotation;
            }
        }                        
    }
}