using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	[SerializeField] private float friction;
	[SerializeField] private float maxSpeed;
	Rigidbody2D rb;
	bool canTakeDamage = true;
	bool paralyzed = false;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
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
		canTakeDamage = true;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Vector2 direction = (collision.transform.position - transform.position).normalized;
		direction.y += 1.5f;
		direction.Normalize();
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null) player.Hurt(0, direction, 10f);
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
}
