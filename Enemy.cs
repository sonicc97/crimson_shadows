using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    public float range = 30;
    public float health = 100;
    public float damage = 20;

    public bool dead = false;

    public GM gm;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GM>();
    }

    // Update is called once per frame
    void Update()
    {
        //// This code checks if the agent is on the NavMesh and every 30 frames sets a new random destination using the NavMeshAgent, allowing the NPC to move around the scene avoiding obstacles.
        if (!dead && agent.isOnNavMesh && Time.frameCount % 30 == 0)
        {
            /* If the distance between the object and the player is less than the given range,
               set the agent's destination to the player's position. */
            if ((transform.position - player.position).magnitude < range)
            {
                agent.SetDestination(player.position);
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if(!dead)
        {
            health -= dmg;
            if (health <= 0)
            {
                gm.EnemyDied();
                dead = true;
                agent.enabled = false;
                GetComponent<Rigidbody>().isKinematic = false;
                StartCoroutine(KillEnemy());
            }
        }
    }

    IEnumerator KillEnemy()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
