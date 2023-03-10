using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {

        enum DestinationIdentifier
        {
            A,B,C,D,E
        }
        
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination;
        [SerializeField] private float fadeOutTime = 1f;
        [SerializeField] private float fadeInTime = 2f;
        [SerializeField] private float fadeWaitTime = 0.5f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);
            
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            
            yield return fader.FadeOut(fadeOutTime);
            
            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            
            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
            
            Destroy(gameObject);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player= GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>() )
            {
                if(portal == this) continue;
                if(portal.destination !=destination) continue;
                return portal;
            }

            return null;
        }
    }

}