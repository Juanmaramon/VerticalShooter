using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{ 
    /// <summary>
    /// Destroy by collision gameObject.
    /// </summary>
    public class DestroyCollision : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(gameObject);
        }
    }
}