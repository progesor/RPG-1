using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Atributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction,ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxNavPathLength = 40f;

        private NavMeshAgent _navMeshAgent;
        private Health _health;
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
        private static readonly int SidewardSpeed = Animator.StringToHash("sidewardSpeed");

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
        }

        void Start()
        {
            
        }


        void Update()
        {
            _navMeshAgent.enabled = !_health.IsDead();

            UpdateAnimator();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            float verticalSpeed = localVelocity.x;
            GetComponent<Animator>().SetFloat(ForwardSpeed, speed);
            GetComponent<Animator>().SetFloat(SidewardSpeed, verticalSpeed);
        }
        
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length-1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination,speedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }
        
        
        public void MoveTo(Vector3 destination,float speedFraction)
        {
            _navMeshAgent.destination = destination;
            _navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            _navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            _navMeshAgent.enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            _navMeshAgent.enabled = true;
            
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
