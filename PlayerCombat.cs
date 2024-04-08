using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject youDiedUI;
    [SerializeField] private GameObject fireBall;
    [SerializeField] private GameObject reticle;
    [SerializeField] private float fireOffset=.7f;
    [SerializeField] private float cooldown = .5f;

    private float timeElapsedSinceFiring;
	Vector2 vecBtwnMouseAndPlayer;

	[SerializeField]
	GameObject DeathParticle;

	private GameObject projectile;
    private Vector2 launchDir;
	private Animator anim;

    private void Start()
    {
		anim = GetComponent<Animator>();
		timeElapsedSinceFiring = cooldown;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>()!=1f || timeElapsedSinceFiring > 0f)
            return;
        float angle = Mathf.Atan2(vecBtwnMouseAndPlayer.y, vecBtwnMouseAndPlayer.x);
		projectile = Instantiate(fireBall, transform.position + (fireOffset * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)))
			, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg));// * 0.5f));

		AudioManager.instance.PlaySound("FStart");

		timeElapsedSinceFiring = cooldown;
    }

    private void Update()
    {
        timeElapsedSinceFiring -= Time.deltaTime;

		Camera cam = Camera.main;
		Vector2 mousePosWorld = cam.ScreenToWorldPoint(reticle.transform.position);
		vecBtwnMouseAndPlayer = (mousePosWorld - (Vector2)transform.position).normalized;
		anim.SetFloat("x", vecBtwnMouseAndPlayer.x);
		anim.SetFloat("y", vecBtwnMouseAndPlayer.y);

	}

	private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            Die();
    }   
    void Die()
    {		
		AudioManager.instance.PlaySound("PlayerDie");
		Instantiate(DeathParticle, transform.position, transform.rotation);

    
        Destroy(gameObject);
		AudioManager.instance.PlaySound("PlayerDie");
		Instantiate(youDiedUI);
		GetComponent<PlayerInput>().actions.actionMaps[1].Enable();
    }

}
