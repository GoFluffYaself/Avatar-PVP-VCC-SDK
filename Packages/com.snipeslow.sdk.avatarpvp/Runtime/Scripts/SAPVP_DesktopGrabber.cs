
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
[RequireComponent(typeof(VRCPickup), typeof(MeshRenderer))]
public class SAPVP_DesktopGrabber : UdonSharpBehaviour
{

    VRCPickup VRCPickup;
    MeshRenderer MeshRenderer;
    private void Start()
    {
        VRCPickup = GetComponent<VRCPickup>();
        MeshRenderer = GetComponent<MeshRenderer>();
    }
    public override void OnPickup()
    {
        if(VRCPickup)
        {
            VRCPickup.pickupable = false;
        }
        if (MeshRenderer)
        {
            MeshRenderer.enabled = false;
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (VRCPickup)
            {
                VRCPickup.pickupable = true;
            }
            if (MeshRenderer)
            {
                MeshRenderer.enabled = true;
            }
            VRCPlayerApi.TrackingData headTrackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            transform.SetPositionAndRotation(headTrackingData.position + (headTrackingData.rotation * Vector3.forward * 0.5f ), headTrackingData.rotation);
        }
    }
}
