using RPG.Atributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private bool isHoming = true;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 2;
        [SerializeField] private UnityEvent onLaunch;
        [SerializeField] private UnityEvent onHit;
        
        private Health target = null;
        private GameObject instigator = null;
        private float damage = 0;
        private bool isLaunched = false;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (!isLaunched)
            {
                isLaunched = true;
                onLaunch.Invoke();
            }
            
            if (target == null) return;
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }

        public void SetTarget(Health target,GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            
            Destroy(gameObject,maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targerCapsule = target.GetComponent<CapsuleCollider>();
            if (targerCapsule == null)
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * targerCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target)return;
            if(target.IsDead()) return;

            target.TakeDamage(instigator, damage);

            speed = 0;

            onHit.Invoke();
            
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
