using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
	public float ImpulseSpeed;
	[SerializeField]
	float ExplosionRadius;
	[SerializeField]
	float PushForce;
	[SerializeField]
	GameObject explosionParticle;

	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(transform.right * ImpulseSpeed, ForceMode2D.Impulse);
		//rb.AddRelativeForce(transform.rotation.eulerAngles * ImpulseSpeed ,ForceMode2D.Impulse);
		AudioManager.instance.PlaySound("FFly");
	}

	// Update is called once per frame
	void Update()
    {
        
    }
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			return;
		Debug.Log(Physics2D.OverlapCircleAll(transform.position, ExplosionRadius).Length);
		foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, ExplosionRadius))
		{
			if (col.isTrigger || col.tag == "Player")
				continue;

			if (col.GetComponent<Fireball>() != null)
			{
				Destroy(col.gameObject);
				continue;
			}

			Vector2 force = (col.transform.position - transform.position) * PushForce;

			Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
			if (rb != null)
			{ 
				rb.AddForce(force,ForceMode2D.Impulse);
				Debug.Log(rb.gameObject);
			}

			GhostBrain gb = col.GetComponent<GhostBrain>();
			if (gb != null)
			{
				Debug.Log(gb.gameObject.GetInstanceID());
				gb.PushGhost();
			}
		}
		AudioManager.instance.StopSound("FFly");
		AudioManager.instance.PlaySound("FHit");

		Instantiate(explosionParticle, transform.position, transform.rotation);
		Destroy(gameObject);
	}
}
