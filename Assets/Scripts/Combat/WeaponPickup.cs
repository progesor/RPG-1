using System.Collections;
using Control;
using RPG.Atributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float healthToRestore = 0;
        [SerializeField] private float respawnTime = 5;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shoulShow)
        {
            GetComponent<Collider>().enabled = shoulShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shoulShow);
            }
        }

        public CursorType getCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;
        }
    }

}