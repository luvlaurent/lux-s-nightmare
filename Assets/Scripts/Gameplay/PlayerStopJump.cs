using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Event triggered when the player stops jumping.
    /// </summary>
    public class PlayerStopJump : Simulation.Event<PlayerStopJump>
    {
        public PlayerController player;

        public override void Execute()
        {
            if (player != null && player.velocity.y > 0)
            {
                player.velocity.y *= 0.5f; // Reduz a velocidade vertical pela metade
            }
        }
    }
}
