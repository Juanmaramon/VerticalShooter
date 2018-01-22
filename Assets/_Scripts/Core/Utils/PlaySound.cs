using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

    /// <summary>
    /// Play sound.
    /// </summary>
    public class PlaySound : MonoBehaviour
    {

        #region Private attributes

        AudioSource audioSource;

        #endregion

        #region Public attributes

        [SerializeField] AudioClip sound;

        #endregion

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();            
        }

        void OnEnable()
        {
            // Play sound
            audioSource.clip = sound;
            audioSource.Play();
        }
    }
}
