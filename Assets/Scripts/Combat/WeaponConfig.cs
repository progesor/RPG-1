using RPG.Atributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile = null;

        private const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand,leftHand);

            Weapon weapon = null;
                
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransfrom(rightHand, leftHand);

                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon==null)return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
            
        }

        private Transform GetTransfrom(Transform rightHand, Transform leftHand)
        {
            if (isRightHanded)
            {
                return rightHand;
            }
            else
            {
                return leftHand;
            }
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator,float calculatedDamage)
        {
            Projectile projectileInstance =
                Instantiate(projectile, GetTransfrom(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target,instigator, calculatedDamage);
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetRange()
        {
            return weaponRange;
        }
    }
}