using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{   
    public enum SoundType
    {
        SWING = 0 ,
        HIT = 1 ,
        DASH = 2 ,
        HURT = 3,
        FOOTSTEPS_GRASS = 4 ,
        BATTLEMUSIC = 5,
        SWORD_CLASH = 6 
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] soundList;
        private static SoundManager instance;
        private AudioSource audioSource;
        private Dictionary<SoundType, AudioClip> soundDict = new Dictionary<SoundType, AudioClip>();

        private void LoadSounds()
        {
            AudioClip[] loadedSounds = Resources.LoadAll<AudioClip>("SFX");

            if (loadedSounds == null || loadedSounds.Length == 0)
            {
                Debug.LogError("Keine Sounds gefunden! Stelle sicher, dass sie im 'Resources/SFX' Ordner sind.");
                return;
            }

            // Sound-Dictionary mit Namen der Audiodateien füllen
            foreach (AudioClip clip in loadedSounds)
            {
                foreach (SoundType sound in System.Enum.GetValues(typeof(SoundType)))
                {
                    if (clip.name.ToUpper() == sound.ToString().ToUpper()) // Name der Datei muss mit Enum übereinstimmen
                    {
                        soundDict[sound] = clip;
                        Debug.Log($"Sound geladen: {sound} -> {clip.name}");
                    }
                }
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                LoadSounds(); // Lade Sounds nach jedem Szenenwechsel
                audioSource = GetComponent<AudioSource>();
                

            }
            else
            {
                
                Debug.LogWarning("Es gibt bereits eine Instanz von SoundManager!");
                
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                Debug.LogError("AudioSource nicht gefunden!");
            }

            if (soundList == null || soundList.Length == 0)
            {
                Debug.LogError("soundList ist leer oder null! Überprüfe, ob es beim Szenenwechsel erhalten bleibt.");
            }
        }


        public static void PlaySound(SoundType sound, float volume = 0.5f)
        {
            if (instance == null || instance.audioSource == null)
            {
                Debug.LogError("SoundManager oder AudioSource ist nicht vorhanden!");
                return;
            }

            if (!instance.soundDict.ContainsKey(sound))
            {
                Debug.LogError($"AudioClip für {sound} wurde nicht gefunden! Überprüfe den Dateinamen.");
                return;
            }

            instance.audioSource.PlayOneShot(instance.soundDict[sound], volume);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
