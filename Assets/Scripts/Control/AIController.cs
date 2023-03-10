using RPG.Combat;
using GameDevTV.Utils;
using RPG.Control;
using UnityEngine;
using RPG.Atributes;
using RPG.Movement;
using UnityEditor;

namespace RPG.Core
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private float aggroCooldownTime = 5f;
        [SerializeField] private bool debug = false;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 3f;
        [SerializeField] private float shoutDistance = 5f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        
        public Color chaseColor = new Color(1, 1, 0, 1f);

        Fighter _fighter;
        Health _health;
        private Mover _mover;
        GameObject _player;

        private LazyValue<Vector3> _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;
        private int _currentWaypointIndex = 0;
        private bool isAggrevated = false;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");

            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }
        private void Update()
        {
            if (_health.IsDead()) return;

            if (IsAggravated() && _fighter.CanAttack(_player))
            {
                _timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer<suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0;
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition,patrolSpeedFraction);
            }
            
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNexIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _fighter.Attack(_player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if(ai==null)continue;
                
                ai.Aggrevate();
            }
        }

        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance || _timeSinceAggrevated < aggroCooldownTime;
        }

        // Called by Unity
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debug)
            {
                //Gizmos.color = Color.blue;
                //Gizmos.DrawWireSphere(transform.position, chaseDistance);
                chaseColor.a = .2f;
                Handles.color = chaseColor;
                Handles.DrawSolidDisc(transform.position, Vector3.up, chaseDistance);
            }
        }
#endif

    }

}