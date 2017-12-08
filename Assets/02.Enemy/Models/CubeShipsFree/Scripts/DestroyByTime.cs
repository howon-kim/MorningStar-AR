using UnityEngine;
using System.Collections;

namespace CubeSpaceFree
{
    // Destroy object after lifetime has passed.
    public class DestroyByTime : MonoBehaviour
    {
        public float lifetime;

        // Use this for initialization
        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}