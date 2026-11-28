using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordHitbox : MonoBehaviour
{
	[SerializeField] private float attackDuration, attackCooldown, knockbackForce;
	[SerializeField] private Vector2 knockbackAngle;
	private Vector2 initialLocalOffset;
	private Vector2 localOffset;
	private PlayerInput input;
	private InputAction moveAction, attackAction;
	private bool canAttack;
	private BoxCollider2D collider;
	private SpriteRenderer spriteRenderer;
	void Awake()
	{
		input = new PlayerInput();
		moveAction = input.Player.Move;
		attackAction = input.Player.Attack;
		initialLocalOffset = transform.localPosition;
		collider = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		Toggle(false);
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

	void Toggle(bool on)
	{
		collider.enabled = on;
		spriteRenderer.enabled = on;
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
		Toggle(true);
		yield return new WaitForSeconds(attackDuration);
		Toggle(false);
		yield return new WaitForSeconds(attackCooldown - attackDuration);
		canAttack = true;
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
		KnockbackHelper.tryToHurt<Enemy>(collision.gameObject, gameObject.transform, knockbackAngle, knockbackForce);
    }
}
