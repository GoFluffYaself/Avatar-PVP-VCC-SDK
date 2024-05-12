

using UnityEngine;
#if UDONSHARP
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;
using UdonSharp;

public class SAPVP_Plugin : UdonSharpBehaviour
{
    [HideInInspector]
    public float ChestDMGMult= 1;
    [HideInInspector]
    public float ChestDMGAdd;
    [HideInInspector]
    public float ChestDMGShieldRatioMult = 1;
    [HideInInspector]
    public float ChestDMGShieldRatioAdd;
    [HideInInspector]
    public float HeadDMGMult = 1;
    [HideInInspector]
    public float HeadDMGAdd;
    [HideInInspector]
    public float HeadDMGShieldRatioMult = 1;
    [HideInInspector]
    public float HeadDMGShieldRatioAdd;
    [HideInInspector]
    public float DOTDMGMult = 1;
    [HideInInspector]
    public float DOTDMGAdd;
    [HideInInspector]
    public float DOTEffectRatioMult = 1;
    [HideInInspector]
    public float DOTEffectRatioAdd;
    [HideInInspector]
    public float HOTAmountMult = 1;
    [HideInInspector]
    public float HOTAmountAdd;
    [HideInInspector]
    public float ElectricDMGMult = 1;
    [HideInInspector]
    public float ElectricDMGAdd;
    [HideInInspector]
    public float FireDMGMult = 1;
    [HideInInspector]
    public float FireDMGAdd;
    [HideInInspector]
    public float PoisonDMGMult = 1;
    [HideInInspector]
    public float PoisonDMGAdd;
    [HideInInspector]
    public float IceDMGMult = 1;
    [HideInInspector]
    public float IceDMGAdd;
    [HideInInspector]
    public float IceEffectRatioMult = 1;
    [HideInInspector]
    public float IceEffectRatioAdd;
    [HideInInspector]
    public float StunEffectRatioMult = 1;
    [HideInInspector]
    public float StunEffectRatioAdd;
    [HideInInspector]
    public float SlowEffectRatioMult = 1;
    [HideInInspector]
    public float SlowEffectRatioAdd;
    [HideInInspector]
    public float SpeedEffectRatioMult = 1;
    [HideInInspector]
    public float SpeedEffectRatioAdd;
    [HideInInspector]
    public float RepairAmountMult = 1;
    [HideInInspector]
    public float RepairAmountAdd;
    [HideInInspector]
    public float RadRateMult = 1;
    [HideInInspector]
    public float RadRateAdd;
    [HideInInspector]
    public float PushPowerMult = 1;
    [HideInInspector]
    public float PushPowerAdd;
    [HideInInspector]
    public float JumpSpeedMult = 1;
    [HideInInspector]
    public float JumpSpeedAdd;
    [HideInInspector]
    public float DoubleJumpSpeedMult = 1;
    [HideInInspector]
    public float DoubleJumpSpeedAdd;
    [HideInInspector]
    public float DashSpeedMult = 1;
    [HideInInspector]
    public float DashSpeedAdd;
    [HideInInspector]
    public float RunSpeedMult = 1;
    [HideInInspector]
    public float RunSpeedAdd;
    [HideInInspector]
    public float WalkSpeedMult = 1;
    [HideInInspector]
    public float WalkSpeedAdd;
    [HideInInspector]
    public float StrafeSpeedMult = 1;
    [HideInInspector]
    public float StrafeSpeedAdd;
    [HideInInspector]
    public DataDictionary ClassData;
    [HideInInspector]
    bool AllowedToManage = false;
    public bool IsAllowedToManage(VRCPlayerApi targetPlayer)
    {
        return AllowedToManage;
    }
}
#else
public class SAPVP_Plugin : MonoBehaviour
{
}
#endif