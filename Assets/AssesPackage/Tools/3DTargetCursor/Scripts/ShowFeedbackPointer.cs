using UnityEngine;
using UnityEngine.AI;

namespace Graphics.Feedback.Scripts

{
    public class ShowFeedbackPointer : MonoBehaviour
    {
        private readonly FeedbackPointer _feedbackPointer = new FeedbackPointer();

        [SerializeField]
        private float _feedbackPointerScale = 1f;

        [SerializeField]
        private GameObject _moveToIndicator;

        [SerializeField]
        private NavMeshAgent _navMeshAgent;

        [SerializeField]
        private int _walkableNavMeshMask = -1;

        public void InteractOnPerformed(Vector2 position)
        {
            if (_navMeshAgent.isOnOffMeshLink)
                return;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(position.x, position.y, Camera.main.nearClipPlane));

            const float maxDistance = 300f;
            if (!Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, maxDistance))
                return;

            NavMesh.SamplePosition(hitInfo.point, out NavMeshHit navHit, maxDistance, _walkableNavMeshMask);

            if (!navHit.hit)
                return;

            var path = new NavMeshPath();
            if (!_navMeshAgent.CalculatePath(navHit.position, path) || path.status == NavMeshPathStatus.PathInvalid)
                return;

            _feedbackPointer.ShowPointer(navHit.position);
            _navMeshAgent.SetPath(path);
        }

        private void Start()
        {
            _feedbackPointer.PreparePointer(_moveToIndicator, _feedbackPointerScale);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                InteractOnPerformed(Input.mousePosition);
        }
    }
}