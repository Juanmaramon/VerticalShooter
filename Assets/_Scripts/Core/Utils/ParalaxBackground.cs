using UnityEngine;
using System.Collections;

namespace Core
{
    public class ParalaxBackground : MonoBehaviour
    {

        [SerializeField]
        private float speed = 0f;

        // Update is called once per frame
        void Update()
        {
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, Time.time * speed);
        }
    }
}