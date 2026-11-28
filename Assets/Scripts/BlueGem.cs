using UnityEngine;

public class BlueGem : Gem
{
    public override void ApplyEffects(Player player)
    {
        player.jumpForce -= 6f;
        player.maxSpeed += 2f;
    }

    protected override void RemoveEffects(Player player)
    {
        player.jumpForce += 6f;
        player.maxSpeed -= 2f;
    }
}
