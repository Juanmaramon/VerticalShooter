using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class AutoDestroy : MonoBehaviour
    {
        #region Public attributes

        [SerializeField] float timeToDestroy;

        #endregion

        // Destroy after time
        void Start()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}
