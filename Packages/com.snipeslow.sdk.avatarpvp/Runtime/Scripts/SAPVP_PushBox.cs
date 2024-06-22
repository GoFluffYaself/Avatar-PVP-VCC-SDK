#if !SAPVP_VRCSDK3_AVATARS && !SAPVP_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#define SAPVP_VRCSDK3_WORLDS
#else
#define SAPVP_VRCSDK3_AVATARS
#endif
#endif
using UnityEngine;
#if SAPVP_VRCSDK3_WORLDS
using UdonSharp;
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
#else
public class SAPVP_PushBox : MonoBehaviour
{
}
#endif