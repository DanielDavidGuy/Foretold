using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordHitbox : MonoBehaviour
{
	private Vector2 initialLocalOffset;
	private Vector2 localOffset;
	private PlayerInput input;
	private InputAction moveAction, attackAction;
	private bool canAttack;
	private BoxCollider2D collider;
	void Awake()
	{
		input = new PlayerInput();
		moveAction = input.Player.Move;
		attackAction = input.Player.Attack;
		initialLocalOffset = transform.localPosition;
		collider = GetComponent<BoxCollider2D>();
		collider.enabled = false;
		canAttack = true;
	}

	void OnEnable()
	{
		input.Player.Enable();
	}

	void OnDisable()
	{
		input.Player.Disable();
	}

	void Update()
	{
		float facingDirection = moveAction.ReadValue<float>();
		if (facingDirection != 0) transform.localPosition = initialLocalOffset * facingDirection;
		if (attackAction.WasPressedThisFrame() && canAttack) { StartCoroutine(Attack()); }
	}

	protected IEnumerator Attack()
	{
		canAttack = false;
		collider.enabled = true;
		yield return new WaitForSeconds(0.5f);
		collider.enabled = false;
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 direction = (collision.transform.position - transform.position).normalized;
		direction.y += 1.5f;
		direction.Normalize();
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
		if (enemy != null) enemy.Hurt(0, direction, 10f);
    }
}
