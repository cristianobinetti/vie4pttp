using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class EntityBehaviour : NetworkBehaviour {

//INST VAR

    //macros
    private Vector3 COLLIDER_TINY_SIZE;
    private Vector3 COLLIDER_AVARAGE_SIZE;
    private Vector3 COLLIDER_BIG_SIZE;

    private int TINY_AGENT_ID;
    private int AVARAGE_AGENT_ID;
    private int BIG_AGENT_ID;
    private int FLYING_AGENT_ID;

    private ObstacleAvoidanceType OBSTACLE_AVOIDANCE_LOW_QUALITY;
    private ObstacleAvoidanceType OBSTACLE_AVOIDANCE_MEDIUM_QUALITY;
    private ObstacleAvoidanceType OBSTACLE_AVOIDANCE_GOOD_QUALITY;

    //components
    private NavMeshAgent entityNavMeshAgent;
    private BoxCollider entityCollider;

    //class
    private EntityClass entityClass;

    private EntityCollisionDetector collisionManager;
    private Collider[] colliders;

    //variables
    [SerializeField]
    private int nature = 0;
    [SerializeField]
    private int dimension = 0;
    [SerializeField]
    private int movment = 0;
    [SerializeField]
    private int vision = 0;
    [SerializeField]
    private int flying = 0;

    private float canMove = 0;

//UNITY METH

    void Start() {

        if (this.isServer) {

            COLLIDER_TINY_SIZE = new Vector3(1, 1, 1);
            COLLIDER_AVARAGE_SIZE = new Vector3(2, 2, 2);
            COLLIDER_BIG_SIZE = new Vector3(3, 3, 3);

            TINY_AGENT_ID = GameObject.Find("Tiny Agent").GetComponent<NavMeshAgent>().agentTypeID;
            AVARAGE_AGENT_ID = GameObject.Find("Avarage Agent").GetComponent<NavMeshAgent>().agentTypeID;
            BIG_AGENT_ID = GameObject.Find("Big Agent").GetComponent<NavMeshAgent>().agentTypeID;
            FLYING_AGENT_ID = GameObject.Find("Flying Agent").GetComponent<NavMeshAgent>().agentTypeID;

            OBSTACLE_AVOIDANCE_LOW_QUALITY = GameObject.Find("Low Quality Agent").GetComponent<NavMeshAgent>().obstacleAvoidanceType;
            OBSTACLE_AVOIDANCE_MEDIUM_QUALITY = GameObject.Find("Medium Quality Agent").GetComponent<NavMeshAgent>().obstacleAvoidanceType;
            OBSTACLE_AVOIDANCE_GOOD_QUALITY = GameObject.Find("Good Quality Agent").GetComponent<NavMeshAgent>().obstacleAvoidanceType;

            entityNavMeshAgent = this.GetComponent<NavMeshAgent>();
            entityCollider = this.GetComponent<BoxCollider>();

            entityClass = this.GetComponent<EntityClass>();

            collisionManager = this.GetComponent<EntityCollisionDetector>();
        }
    }

    void FixedUpdate() {

        if (this.isServer) {

            this.nature = entityClass.nature;
            this.dimension = entityClass.dimension;
            this.movment = entityClass.movement;
            this.vision = entityClass.vision;
            this.flying = entityClass.isFly;

            //nature
            if (nature == 0) this.DefensiveNature();
            if (nature == 1) this.PassiveNature();
            if (nature == 2) this.AggressiveNature();

            //dimension
            if (dimension == 0) {

                entityCollider.size = COLLIDER_TINY_SIZE;

                entityNavMeshAgent.radius = 1;
                entityNavMeshAgent.height = 1;

                if (flying == 0) {

                    entityNavMeshAgent.agentTypeID = TINY_AGENT_ID;
                    entityNavMeshAgent.stoppingDistance = 3;
                }
            }

            if (dimension == 1) {

                entityCollider.size = COLLIDER_AVARAGE_SIZE;

                entityNavMeshAgent.radius = 2;
                entityNavMeshAgent.height = 2;

                if (flying == 0) {

                    entityNavMeshAgent.agentTypeID = AVARAGE_AGENT_ID;
                    entityNavMeshAgent.stoppingDistance = 4;
                }
            }

            if (dimension == 2) {

                entityCollider.size = COLLIDER_BIG_SIZE;

                entityNavMeshAgent.radius = 3;
                entityNavMeshAgent.height = 3;

                if (flying == 0) {

                    entityNavMeshAgent.agentTypeID = BIG_AGENT_ID;
                    entityNavMeshAgent.stoppingDistance = 5;
                }
            }

            //movment
            if (movment == 0) {

                entityNavMeshAgent.speed = 7.5f;
                entityNavMeshAgent.angularSpeed = 240;
                entityNavMeshAgent.acceleration = 15;
            }

            if (movment == 1) {

                entityNavMeshAgent.speed = 5;
                entityNavMeshAgent.angularSpeed = 180;
                entityNavMeshAgent.acceleration = 10;
            }

            if (movment == 2) {

                entityNavMeshAgent.speed = 2.5f;
                entityNavMeshAgent.angularSpeed = 120;
                entityNavMeshAgent.acceleration = 5;
            }

            //vision
            if (vision == 0) {

                collisionManager.radius = 10;
                entityNavMeshAgent.obstacleAvoidanceType = OBSTACLE_AVOIDANCE_LOW_QUALITY;
            }

            if (vision == 1) {

                collisionManager.radius = 20;
                entityNavMeshAgent.obstacleAvoidanceType = OBSTACLE_AVOIDANCE_MEDIUM_QUALITY;
            }

            if (vision == 2) {

                collisionManager.radius = 30;
                entityNavMeshAgent.obstacleAvoidanceType = OBSTACLE_AVOIDANCE_GOOD_QUALITY;
            }

            //flying
            if (flying == 0) {

                entityNavMeshAgent.baseOffset = 0;

                entityNavMeshAgent.autoBraking = true;
            }

            if (flying == 1) {

                entityNavMeshAgent.baseOffset = 5;

                entityNavMeshAgent.agentTypeID = FLYING_AGENT_ID;

                entityNavMeshAgent.stoppingDistance = 0;
                entityNavMeshAgent.autoBraking = false;
            }
        }
    }

//METH

    private void DefensiveNature() {

        //no player is detected
        if (collisionManager.colliders.Length <= 0) this.RandomMovement();

        //at least one player is detected
        else {

            this.colliders = collisionManager.colliders;
            Vector3 newEntityPosition;

            //one player is detected
            if (this.colliders.Length == 1) {

                newEntityPosition = this.transform.position + collisionManager.radius * (this.transform.position - this.colliders[0].transform.position).normalized;
                entityNavMeshAgent.SetDestination(newEntityPosition);
            }

            //more than one player is detected
            else {

                float minDistance = 0;
                int indexOfNearestPlayer = 0;

                for (int i = 0; i < this.colliders.Length; i++) {

                    if (i == 0) {

                        minDistance = Vector3.Distance(this.transform.position, this.colliders[i].transform.position);
                        indexOfNearestPlayer = i;
                    }

                    else if (Vector3.Distance(this.transform.position, this.colliders[i].transform.position) < minDistance) {

                        minDistance = Vector3.Distance(this.transform.position, this.colliders[i].transform.position);
                        indexOfNearestPlayer = i;
                    }
                }

                newEntityPosition = this.transform.position + collisionManager.radius * (this.transform.position - this.colliders[indexOfNearestPlayer].transform.position).normalized;
                entityNavMeshAgent.SetDestination(newEntityPosition);
            }
        }
    }

    private void PassiveNature() {

        this.RandomMovement();
    }

    private void AggressiveNature() {

        //no player is detected
        if (collisionManager.colliders.Length <= 0) this.RandomMovement();

        //at least one player is detected6
        else {

            this.colliders = collisionManager.colliders;

            //one player is detected
            if (this.colliders.Length == 1) {

                entityNavMeshAgent.SetDestination(this.colliders[0].transform.position);
            }

            //more than one player is detected
            else {

                float minDistance = 0;
                int indexOfNearestPlayer = 0;

                for (int i = 0; i < this.colliders.Length; i++) {

                    if (i == 0) {

                        minDistance = Vector3.Distance(this.transform.position, this.colliders[i].transform.position);
                        indexOfNearestPlayer = i;
                    }

                    else if (Vector3.Distance(this.transform.position, this.colliders[i].transform.position) < minDistance) {

                        minDistance = Vector3.Distance(this.transform.position, this.colliders[i].transform.position);
                        indexOfNearestPlayer = i;
                    }
                }

                entityNavMeshAgent.SetDestination(this.colliders[indexOfNearestPlayer].transform.position);
            }
        }
    }

    private void RandomMovement() {

        if (Time.time > canMove) {

            Vector3 randomPosition;
            NavMeshHit randomPositionHit;

            randomPosition = this.transform.position + (Random.insideUnitSphere * collisionManager.radius);
            NavMesh.SamplePosition(randomPosition, out randomPositionHit, collisionManager.radius, NavMesh.AllAreas);

            entityNavMeshAgent.SetDestination(randomPositionHit.position);

            canMove += this.movment + 3;
        }
    }  
}
