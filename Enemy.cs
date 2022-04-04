using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    Material mat;
    ThirdPersonMovement player;
    Collider enemyCollider;
    NavMeshAgent agent;
    [SerializeField] float viewRadius;
    [Range(0, 360)] [SerializeField] float viewAngle;
    [SerializeField] GameObject playerObject;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;
    public bool canSeePlayer;
    Vector3 startPosition;


    void Awake()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
        enemyCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
    }


    // Start is called before the first frame update
    void Start()
    {
        player = ThirdPersonMovement.instance;
        StartCoroutine(FOVRoutine());
        startPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        mat.SetVector("PlayerPosition", player.transform.position);
        ChasePlayer();
        if (canSeePlayer)
            AttackPlayer();

    }

    void GetHit()
    {
        Debug.Log(this.name + "just got hit.");
    }

    void ChasePlayer()
    {
        if (canSeePlayer)
            agent.SetDestination(playerObject.transform.position);
        else
            agent.SetDestination(startPosition);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        
            GetHit();
        
    }

    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f); //put it as variable to save time so that its not called to be 0.2 every routine 
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask)) //Sending a ray to player and if its not hit by any obstacles on the way, the player can be seen
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, playerObject.transform.position) <= viewRadius)
        {
            float speed = agent.speed;
            agent.speed = 0;
            Swing();
            agent.speed = speed;
        }
    }

    void Swing()
    {
        Debug.Log("Swing sword");
    }
   
}
