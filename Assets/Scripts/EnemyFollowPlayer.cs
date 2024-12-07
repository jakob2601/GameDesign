using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollowPlayer : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    //private NavMeshAgent navMeshAgent;
    public float speed = 2.0f;

    // Start is called before the first frame update
    /*
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }
        else if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not placed on a NavMesh on " + gameObject.name);
        }
    }
    */

    // Update is called once per frame
    void Update()
    {
        /*
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(player.position);
        }
        */
        if (player != null)
        {
            // Richtung zum Spieler berechnen
            Vector3 direction = (player.position - transform.position).normalized;

            // Bewegung in Richtung des Spielers
            transform.position += direction * speed * Time.deltaTime;
        }
    }
    /*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            navMeshAgent.isStopped = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            navMeshAgent.isStopped = false;
        }
    }
    */
}
