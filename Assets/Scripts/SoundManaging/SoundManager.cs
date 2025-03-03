using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public enum SoundType
    {
        SWING,
        SWING2,
        HIT,
        DASH,
        HURT,
        FOOTSTEPS_GRASS,
        BATTLEMUSIC,
        BOW,
        SWORD_CLASH
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] soundList;
        private static SoundManager instance;
        private AudioSource audioSource;  // Für normale Soundeffekte
        private AudioSource musicSource;  // Für Hintergrundmusik

        private float musicVolume = 1f; // Standard-Lautstärke für Musik

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // SoundManager bleibt erhalten
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            audioSource = GetComponent<AudioSource>();

            // Neue AudioSource für die Hintergrundmusik hinzufügen
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true; // Musik in Endlosschleife spielen
        }

        void Start()
        {
            // Falls BattleMusic existiert, sofort starten
            PlayMusic(SoundType.BATTLEMUSIC, musicVolume);

        }

        public static void PlaySound(SoundType sound, float volume = 0.5f)
        {
            if (instance == null || instance.soundList.Length <= (int)sound || instance.soundList[(int)sound] == null)
            {
                Debug.LogError("SoundManager: Sound " + sound + " ist NULL oder nicht gesetzt.");
                return;
            }

            instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
        }

        public static void PlayMusic(SoundType sound, float volume = 0.5f)
        {
            if (instance == null || instance.soundList.Length <= (int)sound || instance.soundList[(int)sound] == null)
            {
                Debug.LogError("SoundManager: Musik " + sound + " ist NULL oder nicht gesetzt.");
                return;
            }
        }

        public static void StopMusic()
        {
            if (instance != null)
            {
                instance.musicSource.Stop();
            }
        }

        public static void SetMusicVolume(float volume)
        {
            if (instance != null)
            {
                instance.musicVolume = Mathf.Clamp(volume, 0f, 1f); // Lautstärke auf 0 - 1 begrenzen
                instance.musicSource.volume = instance.musicVolume;
            }
        }

        public static float GetMusicVolume()
        {
            return instance != null ? instance.musicVolume : 0f;
        }
    }
}
