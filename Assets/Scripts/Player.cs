using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Hurtable
{
	public int health {get; set;}
	[SerializeField] private int startingHealth;
	[SerializeField] public float maxSpeed, acceleration, friction, jumpForce, coyoteTime, dashForce, dashDuration, dashCooldown;
	[SerializeField] private float paralysisTime, invincibilityTime;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius = 0.2f;
	[SerializeField] private LayerMask groundLayer;
	public bool isGrounded;
	public bool canTakeDamage {get; set;}
	public bool paralyzed {get; set;}
	public Rigidbody2D rb {get; set;}
	private int facingDirection;
	private PlayerInput input;
	private InputAction moveAction, jumpAction, attackAction;
	private bool jumpRequested, airDashRequested;
	public List<Vector2> additionalMovementVectors;

	public Gem equippedGem;
	public bool hasAirDash, canAirDash;

	void Awake()
	{
		input = new PlayerInput();
		moveAction = input.Player.Move;
		jumpAction = input.Player.Jump;
		attackAction = input.Player.Attack;
		canTakeDamage = true;
		canAirDash = true;
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
		int facingDirection = (int) horizontalMovement;

		if (horizontalMovement != 0 && rb.linearVelocity.x * horizontalMovement < maxSpeed) {
			rb.AddForce(Vector2.right * horizontalMovement * acceleration);
		}
		else {
			rb.AddForce(Vector2.right * -rb.linearVelocity.x * friction);
		}

		if (airDashRequested) {
			rb.AddForce(Vector2.right * horizontalMovement * dashForce, ForceMode2D.Impulse);
			airDashRequested = false;
			StartCoroutine(DashCoroutine());
		}

		if (jumpRequested)
		{
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			jumpRequested = false;
		}

	}

	private void CheckGrounded()
	{
		if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)) isGrounded = true;
		else StartCoroutine(CoyoteTime());
	}

	public void Update()
	{
		if (jumpAction.WasPressedThisFrame() && isGrounded) jumpRequested = true;
		if (attackAction.WasPressedThisFrame() && hasAirDash && canAirDash && !isGrounded) airDashRequested = true;
	}

	public void FixedUpdate()
	{
		CheckGrounded();
		CalculateMovement();
		foreach (Vector2 vector in additionalMovementVectors) rb.linearVelocity += vector;
	}

	private IEnumerator DashCoroutine()
	{
		canAirDash = false;
		friction *= 0.1f;
		yield return new WaitForSeconds(dashDuration);
		friction /= 0.1f;
		yield return new WaitForSeconds(dashCooldown - dashDuration);
		canAirDash = true;
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

	public void Die()
	{
		Destroy(gameObject);
	}

	public void equipGem(Gem newGem)
	{
		if (equippedGem != null) equippedGem.DropGem();
		equippedGem = newGem;
		newGem.ApplyEffects(this);
	}
}
