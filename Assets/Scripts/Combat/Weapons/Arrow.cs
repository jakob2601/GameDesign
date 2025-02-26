using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] protected GameObject hitEffect;
        [SerializeField] protected float effectTime = 0.8f;
        void OnCollisionEnter2D(Collision collision)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
            Destroy(gameObject);
        }
    }
}
