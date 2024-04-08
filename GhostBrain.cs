using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

// elements borrowed from https://arongranberg.com/astar/documentation/dev_4_3_2_0f72d50c/old/wander.php
public class GhostBrain : MonoBehaviour
{
    public enum GhostState
    {
        Wander,
        Chase
    };
    [Tooltip("What state of movement the Ghost is in.\nWander: moves within WanderRadius of initial spot\nChase: tracks detected player")]
    public GhostState ActiveState = GhostState.Wander;

	[Header("Fireball Interaction"), SerializeField, Tooltip("How slow until the Ghost regains control after being pushed")]
	float minPushSpeed = 0.05f;
	[SerializeField, Tooltip("How many hits the Ghost can take before being defeated")]
	int MaxHealth = 5;
	int Health;
	bool beingPushed = false;

    [SerializeField,Tooltip("How far the ghost can move from its start point when wandering")] 
    public float WanderRadius = 20;
    [SerializeField, Tooltip("The range in which the ghost detects the player, overrides the Colllider's radius")]
    float DetectRadius = 3;

    [Header("Components"), SerializeField]
    AIDestinationSetter Chaser;
    IAstarAI ai;
	Rigidbody2D rb;
	Animator anim;
	[HideInInspector] public Vector3 WanderPoint;

	[SerializeField]
	GameObject DeathParticle;

	[Header("DEBUG"), SerializeField, Tooltip("Ghost only stays around where it spawned, not near candle")]
	bool StayAtSpawn = false;

    void Start()
    {
        ai = GetComponent<IAstarAI>();
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		if (StayAtSpawn)
			WanderPoint = transform.position;
		
		// default values
		Health = MaxHealth;
		ActiveState = GhostState.Wander;
		beingPushed = false;
		ai.canMove = true;

		// Play sound
		AudioManager.instance.PlaySound("EnemyFloat");


		// make sure the visualizer size matches radius
		GetComponent<CircleCollider2D>().radius = DetectRadius;
        transform.GetChild(0).localScale = new Vector3(DetectRadius * 2, DetectRadius * 2, 1); 
    }
    void Update()
    {
		anim.SetFloat("x", ai.velocity.normalized.x);
		anim.SetFloat("y", ai.velocity.normalized.y);


		// When the push slows down enough the Ghost should resume normal movement
		if (beingPushed && rb.velocity.magnitude <= minPushSpeed)
		{
			ai.canMove = true;
			beingPushed = false;
		}

		// Update the destination of the AI if
		// the AI is not already calculating a path and
		// the ai has reached the end of the path or it has no path at all
		// the ai is in a Wandering state
		if (ActiveState == GhostState.Wander && !ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
        {
            ai.destination = PickRandomPoint();
            ai.SearchPath();
        }

		// if there is no way to get to the player (or the point we picked is inaccessible)
		GraphNode startOfPath = AstarPath.active.GetNearest(transform.position).node;
		GraphNode endOfPath = AstarPath.active.GetNearest(ai.destination).node;
		if (PathUtilities.IsPathPossible(startOfPath, endOfPath) == false)
		{
			ActiveState = GhostState.Wander; // turn on wander
			Chaser.enabled = false;

			ai.destination = PickRandomPoint();
			ai.SearchPath();
		}

	}

	Vector3 PickRandomPoint()
    {
        var point = Random.insideUnitSphere * WanderRadius;
        point.z = transform.position.z; // flatten point for 2D
        point += WanderPoint; // localize it within their area
        return point;
    }

    // detect if we find the player then start chasing them
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") // if we find the player
			StartChase(collision.transform);
    }
	void StartChase(Transform player)
	{
		ActiveState = GhostState.Chase; // turn off wander
		Chaser.enabled = true;
		Chaser.target = player.transform; // set player as our target to chase
	}

	/// <summary>
	/// disable/enable enemy movement
	/// </summary>
	/// <param name="pause">whether the game is paused or not</param>
	public void OnPause(bool pause)
	{
		ai.canMove = !pause;
	}

	/// <summary>
	/// Default Parameters to set up ghost spawning neatly
	/// </summary>
	/// <param name="NewWanderPoint">The point at which the ghost sets as his territory and wanders</param>
	/// <param name="radius">How far away from NewWanderPoint the Ghost goes</param>
	/// <param name="chase">Wether this ghost is part of the end chase, if true will try to place the ghost away from the exit</param>
	public void SpawnConstructor(Vector3 NewWanderPoint, float radius, bool chase = false)
	{
		Vector2 spawnpoint;
		Vector2 doorpos = FindObjectOfType<ExitDoor>().transform.position;
		if (chase)
			spawnpoint = (Vector2.up * AstarPath.active.data.gridGraph.depth * 0.5f) + (Vector2)AstarPath.active.data.gridGraph.center + Vector2.right * Random.Range(-15,15);
		else
			spawnpoint = (Random.insideUnitCircle.normalized * AstarPath.active.data.gridGraph.depth * 0.5f) + (Vector2)AstarPath.active.data.gridGraph.center;

		if (chase)
		{
			AudioManager.instance.StopSound("LevelMusic");
			AudioManager.instance.PlaySound("ChaseMusic");

			StartChase(FindObjectOfType<PlayerCombat>().transform);
		}

		transform.position = spawnpoint;
		WanderPoint = NewWanderPoint;
		WanderRadius = radius;
		//ai.canMove = true;
	}

	public void PushGhost()
	{
		beingPushed = true;
		ai.canMove = false;
		if (--Health <= 0)
		{
			GhostBrain newghost = Instantiate(gameObject).GetComponent<GhostBrain>();
			newghost.SpawnConstructor(transform.position, WanderRadius);
			AudioManager.instance.StopSound("EnemyFloat");
			AudioManager.instance.PlaySound("EnemyDie");
			Instantiate(DeathParticle, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}
