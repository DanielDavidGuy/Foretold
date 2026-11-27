using UnityEngine;

public class CameraBoundToPlayer : MonoBehaviour
{
	[SerializeField] private Player player;
	[SerializeField] private GameObject focus;
	[SerializeField] private float cameraSpeed, cameraSettleRadius;
	private bool cameraIsSet, playerWasGrounded;
	private float targetY;
	
	void Start()
	{
		cameraIsSet = true;
		targetY = focus.transform.position.y;
		transform.position = new Vector3(focus.transform.position.x, targetY, transform.position.z);
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.TryGetComponent<Player>(out _)) cameraIsSet = false;
	}

	void Update()
	{
		if ((!playerWasGrounded && player.isGrounded) || focus.transform.position.y < targetY) {
			targetY = focus.transform.position.y;
			cameraIsSet = false;
		}
		playerWasGrounded = player.isGrounded;
	}

	void LateUpdate()
	{
		if (cameraIsSet) return;

		float t = 1f - Mathf.Exp(-cameraSpeed * Time.deltaTime);
		Vector3 targetPosition = new Vector3(focus.transform.position.x, targetY, transform.position.z);
		transform.position = Vector3.Lerp(transform.position, targetPosition, t);
		cameraIsSet = Vector2.Distance(transform.position, targetPosition) < cameraSettleRadius;
	}
}
