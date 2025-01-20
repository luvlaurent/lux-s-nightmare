using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;

            if (player != null)  // Verifica se o player foi inicializado
            {
                if (player.health != null && player.health.IsAlive)  // Verifica se o health está disponível
                {
                    player.health.Die();
                    model.virtualCamera.Follow = null;
                    model.virtualCamera.LookAt = null;
                    player.controlEnabled = false;

                    if (player.audioSource != null && player.ouchAudio != null)  // Verifica se o áudio está configurado
                    {
                        player.audioSource.PlayOneShot(player.ouchAudio);
                    }

                    if (player.animator != null)  // Verifica se o animator está presente
                    {
                        player.animator.SetTrigger("hurt");
                        player.animator.SetBool("dead", true);
                    }

                    Simulation.Schedule<PlayerSpawn>(2);
                }
            }
            else
            {
                Debug.LogError("Player não encontrado no model.");
            }
        }
    }
}
