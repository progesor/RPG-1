using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Sound
{
    public class SoundRandomiser : MonoBehaviour
    {
        [SerializeField]
        public AudioClip[] sounds;
        private AudioSource source;
        [SerializeField] public float maxVolume = 1;
        [SerializeField]
        [Range(0.1f, 0.9f)] public float volumeChangeMultiplier = 0.2f;
        [SerializeField]
        [Range(0.1f, 0.5f)] public float pitchChangeMultiplier = 0.2f;

        private void Start()
        {
            source = GetComponent<AudioSource>();
        }

        public void PlayOnce()
        {
            source.clip = sounds[Random.Range(0, sounds.Length)];
            source.volume = Random.Range(maxVolume - volumeChangeMultiplier, maxVolume);
            source.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
            source.PlayOneShot(source.clip);
        }
    }
}