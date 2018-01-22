using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Return to pool an object so can be reused.
    /// </summary>
    public class ReturnToPool : MonoBehaviour
    {
        #region Public attributes

        [SerializeField] float timeToReuse;

        #endregion

        void OnEnable()
        {
            if (timeToReuse != 0f)
                Invoke("Return", timeToReuse);
        }

        void OnDisable()
        {
            Return();
        }

        void Return()
        {
            // Cancel Return invoke call
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }

}
