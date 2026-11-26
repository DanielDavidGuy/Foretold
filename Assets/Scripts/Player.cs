using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] private float maxSpeed;
	[SerializeField] private float acceleration;
	[SerializeField] private float friction;
	[SerializeField] private float jumpForce;
	[SerializeField] private float coyoteTime;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius = 0.2f;
	[SerializeField] private LayerMask groundLayer;
	public bool isGrounded;
	private Rigidbody2D rb;
	private PlayerInput input;
	private InputAction moveAction, jumpAction;
	private bool canTakeDamage, paralyzed, jumpRequested;
	public List<Vector2> additionalMovementVectors;

	void Awake()
	{
		input = new PlayerInput();
		moveAction = input.Player.Move;
		jumpAction = input.Player.Jump;
		paralyzed = false;
		canTakeDamage = true;
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

	public void Hurt(int damage, Vector2 knockbackDirection, float knockbackForce)
	{
		if (canTakeDamage) StartCoroutine(TakeDamage(damage, knockbackDirection, knockbackForce));
	}

	IEnumerator TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
	{
		canTakeDamage = false;
		paralyzed = true;
		rb.linearVelocity = Vector2.zero;
		rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.4f);
		paralyzed = false;
		yield return new WaitForSeconds(0.6f);
		canTakeDamage = true;
	}

	void ClampVelocity()
	{
		Vector2 velocity = rb.linearVelocity;
		velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
		rb.linearVelocity = velocity;
	}
}
