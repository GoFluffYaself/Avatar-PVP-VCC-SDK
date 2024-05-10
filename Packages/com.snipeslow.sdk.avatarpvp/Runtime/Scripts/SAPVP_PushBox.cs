
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SAPVP_PushBox : UdonSharpBehaviour
{
    public SAPVP_Player player;
    private void OnParticleCollision(GameObject other)
    {
        if (!player)
            return;
        if (!Networking.IsOwner(Networking.LocalPlayer, player.gameObject))
            return;
        if (!player.Manager)
            return;
        if (!player.Manager.Classes[player.Manager.ActiveClass])
            return;
        switch (gameObject.layer)
        {
            case 27:
                player.Radiation();
                player.Push(transform.position);
                break;
        }
    }
}
