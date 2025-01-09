using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{   
    public enum SoundType
    {
        SWING,
        HIT,
        DASH,
        HURT,
        FOOTSTEPS_GRASS,
        BATTLEMUSIC
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] soundList;
        private static SoundManager instance;
        private AudioSource audioSource;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public static void PlaySound(SoundType sound, float volume = 0.5f)
        {
            instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
