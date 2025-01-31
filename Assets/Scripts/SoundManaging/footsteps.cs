using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepScript : MonoBehaviour
{
    public AudioSource footstepAudioSource;

    private void setFootStep(AudioSource footstepAudioSource)
    {
        this.footstepAudioSource = footstepAudioSource;
    }

    private AudioSource getFootStep()
    {
        return footstepAudioSource;
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform footStepTransform = transform.Find("Footsteps");

        if (footStepTransform == null)
        {
            Debug.LogError("Footstep object not found");
        }
        else
        {
            AudioSource audioSource = footStepTransform.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component not found on Footsteps object");
            }
            else
            {
                this.setFootStep(audioSource);
            }
        }

        if (footstepAudioSource != null)
        {
            footstepAudioSource.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool wPressed = Input.GetKey("w");
        bool sPressed = Input.GetKey("s");
        bool aPressed = Input.GetKey("a");
        bool dPressed = Input.GetKey("d");

        if (wPressed || sPressed || aPressed || dPressed)
        {
            if (footstepAudioSource != null && !footstepAudioSource.isPlaying)
            {
                footstepAudioSource.enabled = true;
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource != null && footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
                footstepAudioSource.enabled = false;
            }
        }
    }
}