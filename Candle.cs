using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Candle : MonoBehaviour
{
	[Tooltip("Has the player interacted with the light?")]
	public bool isLit = true;
	CircleCollider2D LitArea;
		
	[Header("Master Candle"), SerializeField]
	bool isMasterCandle;

	[Header("Ghost Spawning")]
	[SerializeField, Tooltip("Amount of ghosts that spawn when candle goes out")]
	int GhostSpawns = 2;
	[SerializeField, Tooltip("Radius of which ghosts wander from candle")]
	float WanderRadius = 3;
	[SerializeField]
	GameObject ghostPrefab;

	// Start is called before the first frame update
	void Start()
    {
		LitArea = GetComponent<CircleCollider2D>();
		AstarPath.active.UpdateGraphs(LitArea.bounds);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		OnCollisionStay2D(collision);
	}
	private void OnCollisionStay2D(Collision2D collision)
	{
		Debug.Log("collide");

		if (collision.transform.tag == "Player" && isLit)
		{
			SnuffCandle();
		}
	}

	void SnuffCandle()
	{
		if (isMasterCandle)
		{
			foreach (Candle candle in FindObjectsOfType<Candle>())
				if (candle.isLit && candle != this)
					return; // dont get snuffed if any other candles are lit
		}
		
		// decrement candles remaining on HUD
        CandlesRemaining hud = GameObject.FindGameObjectWithTag("CandleCount").GetComponent<CandlesRemaining>();
        hud.DecrementRemainingCandles();	
        
		isLit = false;
		
		gameObject.layer = 8; // place the object out of the GhostWall layer
		// define area of the graph to update, so we dont update the whole level
		Bounds litbounds = new Bounds(transform.position, Vector3.one * LitArea.radius);
		AstarPath.active.UpdateGraphs(litbounds); // update the graph
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject); // delete visual indicators
		}

		for (int i = 0; i < GhostSpawns; i++)
		{
			GhostBrain newghost = Instantiate(ghostPrefab).GetComponent<GhostBrain>();
			newghost.SpawnConstructor(transform.position, WanderRadius, isMasterCandle);
		}		
	
		AudioManager.instance.PlaySound("BlowCandle");

	}
}
