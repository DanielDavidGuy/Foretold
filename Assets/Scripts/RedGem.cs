using UnityEngine;

public class RedGem : Gem
{
    public override void ApplyEffects(Player player)
    {
        player.hasAirDash = true;
    }

    protected override void RemoveEffects(Player player)
    {
        player.hasAirDash = false;
    }
}
