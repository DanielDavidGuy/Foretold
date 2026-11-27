using UnityEngine;

public interface Hurtable
{
    bool canTakeDamage {get; set;}
    bool paralyzed {get; set;}
    Rigidbody2D rb {get; set;}
    int health {get; set;}

    void Hurt(int damage, Vector2 knockbackDirection, float knockbackForce)
	{
		if (canTakeDamage) TakeDamage(damage, knockbackDirection, knockbackForce);
	}

	void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
	{
		canTakeDamage = false;
		paralyzed = true;
		rb.linearVelocity = Vector2.zero;
		rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
		health--;
		if (health <= 0) Die();
		RunCoroutineAfterDamage();
	}

    void Die();

    void RunCoroutineAfterDamage();
}
