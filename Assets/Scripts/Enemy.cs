using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, Hurtable
{
	public int health {get; set;}
	[SerializeField] private int startingHealth;
	[SerializeField] private float friction, maxSpeed, knockbackForce;
	[SerializeField] private Vector2 knockbackAngle;
	[SerializeField] private float paralysisTime, invincibilityTime;
	public bool canTakeDamage {get; set;}
	public bool paralyzed {get; set;}
	public Rigidbody2D rb {get; set;}

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		canTakeDamage = true;
		health = startingHealth;
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

	void OnCollisionEnter2D(Collision2D collision)
	{
		KnockbackHelper.tryToHurt<Player>(collision.gameObject, gameObject.transform, knockbackAngle, knockbackForce);
	}

	void CalculateMovement()
	{
		if (paralyzed) return;
		rb.AddForce(Vector2.right * -rb.linearVelocity.x * friction);
		ClampVelocity();
	}

	void FixedUpdate()
	{
		CalculateMovement();
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
