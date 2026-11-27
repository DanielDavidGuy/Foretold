using UnityEngine;

public class KnockbackHelper
{
    public static void tryToHurt<Target>(GameObject collision, Transform self, Vector2 knockbackAngle, float knockbackForce) where Target : Hurtable
    {
        if (collision.TryGetComponent(out Target target)) {
			float horizontalDirection = Mathf.Sign(collision.transform.position.x - self.position.x);
			Vector2 direction = new Vector2(horizontalDirection * knockbackAngle.x, knockbackAngle.y).normalized;
			target.Hurt(0, direction, knockbackForce);
		}
    }
}