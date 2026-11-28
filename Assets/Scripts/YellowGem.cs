using UnityEngine;

public class YellowGem : Gem
{
    public override void ApplyEffects(Player player)
    {
        player.jumpForce += 10f;
        player.maxSpeed -= 2f;
    }

    protected override void RemoveEffects(Player player)
    {
        player.jumpForce -= 10f;
        player.maxSpeed += 2f;
    }
}
