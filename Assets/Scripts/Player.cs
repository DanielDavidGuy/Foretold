using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Hurtable
{
	public int health {get; set;}
	[SerializeField] private int startingHealth;
	[SerializeField] private float maxSpeed, acceleration, friction, jumpForce, coyoteTime;
	[SerializeField] private float paralysisTime, invincibilityTime;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius = 0.2f;
	[SerializeField] private LayerMask groundLayer;
	public bool isGrounded;
	public bool canTakeDamage {get; set;}
	public bool paralyzed {get; set;}
	public Rigidbody2D rb {get; set;}
	private PlayerInput input;
	private InputAction moveAction, jumpAction;
	private bool jumpRequested;
	public List<Vector2> additionalMovementVectors;

	void Awake()
	{
		input = new PlayerInput();
		moveAction = input.Player.Move;
		jumpAction = input.Player.Jump;
		paralyzed = false;
		canTakeDamage = true;
		health = startingHealth;
		rb = GetComponent<Rigidbody2D>();
	}

	void OnEnable()
	{
		input.Player.Enable();
	}

	void OnDisable()
	{
		input.Player.Disable();
	}

	private void CalculateMovement()
	{
		if (paralyzed) return;

		float horizontalMovement = moveAction.ReadValue<float>();

		if (horizontalMovement != 0) rb.AddForce(Vector2.right * horizontalMovement * acceleration);
		else rb.AddForce(Vector2.right * -rb.linearVelocity.x * friction);
		ClampVelocity();

		if (jumpRequested)
		{
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			jumpRequested = false;
		}
	}
	private void RequestJump()
	{
		if (isGrounded) jumpRequested = true;
	}

	private void CheckGrounded()
	{
		if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)) isGrounded = true;
		else StartCoroutine(CoyoteTime());
	}

	public void Update()
	{
		if (jumpAction.WasPressedThisFrame() && isGrounded) RequestJump();
	}

	public void FixedUpdate()
	{
		CheckGrounded();
		CalculateMovement();
		foreach (Vector2 vector in additionalMovementVectors) rb.linearVelocity += vector;
	}

	IEnumerator CoyoteTime()
	{
		yield return new WaitForSeconds(coyoteTime);
		if (!Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)) isGrounded = false;
	}

	public void RunCoroutineAfterDamage()
	{
		StartCoroutine(ResetAfterDamage());
	}

	IEnumerator ResetAfterDamage()
	{
		yield return new WaitForSeconds(paralysisTime);
		paralyzed = false;
		yield return new WaitForSeconds(invincibilityTime - paralysisTime);
		canTakeDamage = true;
	}

	void ClampVelocity()
	{
		Vector2 velocity = rb.linearVelocity;
		velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
		rb.linearVelocity = velocity;
	}

	public void Die()
	{
		Destroy(gameObject);
	}
}
