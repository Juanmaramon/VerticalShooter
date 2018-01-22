using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Spawn area. 2D area to spawn magic creatures! :)
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Collider2D))]
    public class SpawnArea : MonoBehaviour
    {

        BoxCollider2D areaCollider;

        [HideInInspector]
        public ScreenBounds area;

        // Use this for initialization
        void Start()
        {
            areaCollider = GetComponent<BoxCollider2D>();

            // Create area of spawing
            area.xMin = (transform.position.x + areaCollider.offset.x) - areaCollider.size.x / 2;
            area.xMax = (transform.position.x + areaCollider.offset.x) + areaCollider.size.x / 2;
            area.yMin = (transform.position.y + areaCollider.offset.y) - areaCollider.size.y / 2;
            area.yMax = (transform.position.y + areaCollider.offset.y) + areaCollider.size.y / 2;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(
                transform.position + new Vector3(areaCollider.offset.x, areaCollider.offset.y, 0f),
                new Vector3(areaCollider.size.x, areaCollider.size.y, 0f)
            );
        }
    }
}