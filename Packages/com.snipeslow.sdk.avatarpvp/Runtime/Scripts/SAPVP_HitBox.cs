
using UnityEngine;
#if UDONSHARP
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;

public class SAPVP_HitBox : UdonSharpBehaviour
{
    int LayerIteration = 17;
    public SAPVP_Player player;
    public bool IsHead = false;
    private void OnParticleCollision(GameObject other)
    {
        if(!player)
            return;
        if(!Networking.IsOwner(Networking.LocalPlayer,player.gameObject))
            return;
        if (!player.Manager)
            return;
        if (!player.Manager.Classes[player.Manager.ActiveClass])
            return;
        switch (gameObject.layer)
        {
            case 14:
                player.Cancel();
                break;
            case 15:
                player.Electric();
                break;
            case 16:
                player.Slow();
                break;
            case 17:
                if(IsHead)
                {
                    player.TakeDamageHead(player.Manager.Classes[player.Manager.ActiveClass].HeadDMG);
                }
                else
                {
                    player.TakeDamageChest(player.Manager.Classes[player.Manager.ActiveClass].ChestDMG);
                }
                break;
            case 22:
                player.Frost();
                break;
            case 23:
                player.Bleed();
                break;
            case 24:
                player.Stun();
                break;
            case 25:
                player.Poison();
                break;
            case 26:
                player.Speed();
                break;
            case 27:
                //Do nothing as it is handled by other code
                break;
            case 28:
                player.Shield();
                break;
            case 29:
                player.HOT();
                break;
            case 30:
                player.Repair();
                break;
            case 31:
                player.Fire();
                break;
        }
    }
    private void FixedUpdate()
    {
        LayerIteration++;
        //LayerIteration = Mathf.Clamp(LayerIteration % 32, 17, 31);
        if (LayerIteration < 14)
        {
            LayerIteration = 14;
        }
        if (LayerIteration > 17 && LayerIteration < 22)
        {
            LayerIteration = 22;
        }
        if (LayerIteration > 31)
        {
            LayerIteration = 14;
        }
        gameObject.layer = LayerIteration;
    }
}
#else
public class SAPVP_HitBox : MonoBehaviour
{
}
#endif