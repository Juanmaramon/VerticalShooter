using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Fixed movement. Vertical movement in one direction
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class FixedMovement : MonoBehaviour
    {

        #region Private attributes

        Vector2 force;

        #endregion

        #region Public attributes

        [SerializeField] public Common.Direction direction;
        public Vector2 customAmount;

        #endregion

        // Use this for initialization
        public void Init()
        {
            // Apply constant force up or down the screen
            switch (direction)
            {
                case Common.Direction.UP:
                    force = new Vector2(0, customAmount.y);
                    break;
                case Common.Direction.DOWN:
                    force = new Vector2(0, -customAmount.y);
                    break;
                case Common.Direction.CUSTOM:
                    force = customAmount;
                    break;
            }

            GetComponent<Rigidbody2D>().velocity = force;
        }
    }
}