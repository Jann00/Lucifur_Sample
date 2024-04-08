using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	public bool isPaused;

	// maxSpeed in units/sec
	[SerializeField] private float maxSpeed = 2f;
	
	// acceleration in units/sec^2
	[SerializeField] private float acceleration = 5f;
	
	// should the player immediately stop when keys are released?
	[SerializeField] private bool stopImmediately = true;

	[SerializeField] private bool enableBoost = true;
	[SerializeField] private float maxBoostSpeed = 10f;

	[SerializeField] private float minSpeed = .05f;

	[SerializeField] private GameObject pauseMenu;

	private Vector2 inputDir;
	private Rigidbody2D rigidBody;

	private bool isBoosting = false;
	private PlayerInput input;
	private GameObject currentPauseMenu;

	// Start is called before the first frame update
	void Start()
	{
		 rigidBody = GetComponent<Rigidbody2D>();
		 input = GetComponent<PlayerInput>();
	}

	public void TogglePause()
	{
			isPaused = !isPaused;
    		
    		Cursor.visible = isPaused;
    		Time.timeScale = ((isPaused) ? 0f:1f);
    
    		if (isPaused)
            {
	            currentPauseMenu = Instantiate(pauseMenu);
    			input.actions.Disable();
    			input.actions.actionMaps[1].Enable();
    		}
    		else
    		{
	            Destroy(currentPauseMenu);
    			input.actions.Enable();
    			input.actions.actionMaps[1].Disable();
    		}
    		
    
    		// grab all current ghosts and pause them
    		foreach (GhostBrain ghost in GameObject.FindObjectsOfType<GhostBrain>())
    			ghost.OnPause(isPaused);	
	}

	public void OnPause(InputAction.CallbackContext context)
	{
		float val = context.ReadValue<float>();
		if (val==1f)
			TogglePause();
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		inputDir = context.ReadValue<Vector2>();
	}

	public void OnBoost(InputAction.CallbackContext context)
	{
		if(context.started)
			isBoosting = true;
		else if (context.canceled)
		{
			isBoosting = false;
		}
	}

	private void FixedUpdate()
	{
		if (isPaused) // dont move if paused
			return;

		float currentMaxSpeed = (isBoosting&&enableBoost) ? (maxBoostSpeed) : (maxSpeed);
		
        if (inputDir == Vector2.zero && stopImmediately)
        {
	       rigidBody.velocity = Vector2.zero;
        }

        if (inputDir != Vector2.zero && rigidBody.velocity.magnitude < currentMaxSpeed)
        {
	        rigidBody.AddForce(inputDir.normalized*acceleration*rigidBody.mass);
        }

        else if (inputDir != Vector2.zero && rigidBody.velocity.magnitude > currentMaxSpeed)
	        rigidBody.velocity = inputDir*currentMaxSpeed;

        if (rigidBody.velocity.magnitude < minSpeed)
        {
	        rigidBody.velocity=Vector2.zero;
        }
	}

	// Update is called once per frame
	void Update()
	{
	
	}
}
