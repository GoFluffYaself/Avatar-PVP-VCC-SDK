#if !SAPVP_VRCSDK3_AVATARS && !SAPVP_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#define SAPVP_VRCSDK3_WORLDS
#else
#define SAPVP_VRCSDK3_AVATARS
#endif
#endif
using System;
using UnityEngine;
#if SAPVP_VRCSDK3_WORLDS
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SAPVP_Class : UdonSharpBehaviour
{
    DataDictionary ClassStats = new DataDictionary();
    public SAPVP_Manager Manager;
    [UdonSynced]
    public string ClassCode = "";
    public int ClassIndex;
    public Text StatsText;
    public Text ClassNameText;
    public Image ClassImage;
    public int ErrorCode = -1;
    public override void Interact()
    {
        if (!Manager)
            return;
        Manager.SetClass(ClassIndex);
        Manager.LocalPlayer._Health = (Manager.LocalPlayer._Health/ Manager.LocalPlayer._MaxHealth) * MaxHealth;
        Manager.LocalPlayer._MaxHealth = MaxHealth;
        Networking.LocalPlayer.SetManualAvatarScalingAllowed(AllowScaling);
    }
    public override void OnPreSerialization()
    {
        if(VRCJson.TrySerializeToJson(ClassStats, JsonExportType.Minify, out DataToken tempToken))
        {
            ClassCode = tempToken.String;
        }
    }
    public override void OnDeserialization()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (VRCJson.TryDeserializeFromJson(ClassCode, out DataToken tempToken))
        {
            ClassStats = tempToken.DataDictionary;
            if (StatsText)
            {
                StatsText.text = DisplayName+"\n";
                StatsText.text += "Max Health: " + MaxHealth + "\n";
                StatsText.text += "Allow Scaling: " + AllowScaling + "\n";
                StatsText.text += "Sprint speed: " + RunSpeed + "\n";
                StatsText.text += "Run/Walk speed: " + WalkSpeed + "\n";
                StatsText.text += "Strafe speed: " + StrafeSpeed + "\n";
                if (StrafeSpeed > 0)
                {
                    StatsText.text += "Dash Speed: " + DashSpeed + "\n";
                }
                if (DoubleJumpSpeed > 0)
                {
                    StatsText.text += "Double Jump Power: " + DoubleJumpSpeed + "\n";
                }
                StatsText.text += "Jump Power: " + JumpSpeed + "\n";
                StatsText.text += "Chest Direct Damage: " + ChestDMG + " (" + (ChestDMG * ChestDMGShieldRatio) + " with Shield)\n";
                StatsText.text += "Head Direct Damage: " + HeadDMG + " (" + (HeadDMG * HeadDMGShieldRatio) + " with Shield)\n";
                StatsText.text += "DOT Damage: " + DOTDMG + " per second for 5 seconds with " + DOTEffectRatio + " speed multiplier\n";
                StatsText.text += "HOT healing: " + HOTAmount + " per second for 5 seconds\n";
                StatsText.text += "Electric Damage: " + ElectricDMG + "\n";
                StatsText.text += "Fire Damage: " + FireDMG + " per second for 5 seconds\n";
                StatsText.text += "Poison Damage: " + PoisonDMG + " per second till healed\n";
                StatsText.text += "Ice Damage: " + IceDMG + " per second for 6 seconds with " + IceEffectRatio + " speed multiplier\n";
                StatsText.text += "Stun Debuff: " + StunEffectRatio + " speed multiplier\n";
                StatsText.text += "Slow Debuff: " + SlowEffectRatio + " speed multiplier\n";
                if (SpeedEffectRatio > 4)
                {
                    StatsText.text += "Speed Buff: Skooma levels of fast\n";
                }
                else
                {
                    StatsText.text += "Speed Buff: " + SpeedEffectRatio + " speed multiplier\n";
                }
            }
            if (ClassNameText)
            {
                ClassNameText.text = DisplayName;
            }
        }

    }

    public string DisplayName {
        get {
            if(ClassStats.TryGetValue("DisplayName",TokenType.String, out DataToken tempToken))
            {
                return tempToken.String;
            }
            return "";
        }
    }

    public float MaxHealth
    {
        get
        {
            float currentValue = 2000;
            if (ClassStats.TryGetValue("MaxHealth", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float ChestDMG
    {
        get
        {
            float currentValue = 200;
            if (ClassStats.TryGetValue("ChestDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float ChestDMGShieldRatio
    {
        get
        {
            float currentValue = 0.5f;
            if (ClassStats.TryGetValue("ChestDMGShieldRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float HeadDMG
    {
        get
        {
            float currentValue = 1900;
            if (ClassStats.TryGetValue("HeadDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float HeadDMGShieldRatio
    {
        get
        {
            float currentValue = 0.75f;
            if (ClassStats.TryGetValue("HeadDMGShieldRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float DOTDMG
    {
        get
        {
            float currentValue = 200;
            if (ClassStats.TryGetValue("DOTDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float DOTEffectRatio
    {
        get
        {
            float currentValue = 0.8f;
            if (ClassStats.TryGetValue("DOTEffectRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float HOTAmount
    {
        get
        {
            float currentValue = 250;
            if (ClassStats.TryGetValue("HOTAmount", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float ElectricDMG
    {
        get
        {
            float currentValue = 80;
            if (ClassStats.TryGetValue("ElectricDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }

    public float FireDMG
    {
        get
        {
            float currentValue = 280;
            if (ClassStats.TryGetValue("FireDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float PoisonDMG
    {
        get
        {
            float currentValue = 16;
            if (ClassStats.TryGetValue("PoisonDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float IceDMG
    {
        get
        {
            float currentValue = 80;
            if (ClassStats.TryGetValue("IceDMG", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float IceEffectRatio
    {
        get
        {
            float currentValue = 0.4f;
            if (ClassStats.TryGetValue("IceEffectRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float StunEffectRatio
    {
        get
        {
            float currentValue = 0.1f;
            if (ClassStats.TryGetValue("StunEffectRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float SlowEffectRatio
    {
        get
        {
            float currentValue = 0.6f;
            if (ClassStats.TryGetValue("SlowEffectRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float SpeedEffectRatio
    {
        get
        {
            float currentValue = 1.8f;
            if (ClassStats.TryGetValue("SpeedEffectRatio", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float RepairAmount
    {
        get
        {
            float currentValue = -70;
            if (ClassStats.TryGetValue("RepairAmount", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }

    public float RadRate
    {
        get
        {
            float currentValue = 2;
            if (ClassStats.TryGetValue("RadRate", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }

    public float PushPower
    {
        get
        {
            float currentValue = 16;
            if (ClassStats.TryGetValue("PushPower", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }

    public float JumpSpeed
    {
        get
        {
            float currentValue = 5;
            if (ClassStats.TryGetValue("JumpSpeed", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float DoubleJumpSpeed
    {
        get
        {
            float currentValue = 0;
            if (ClassStats.TryGetValue("DoubleJumpSpeed", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float DashSpeed
    {
        get
        {
            float currentValue = 0;
            if (ClassStats.TryGetValue("DashSpeed", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float RunSpeed
    {
        get
        {
            float currentValue = 5;
            if (ClassStats.TryGetValue("RunSpeed", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float WalkSpeed
    {
        get
        {
            float currentValue = 3;
            if (ClassStats.TryGetValue("WalkSpeed", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public float StrafeSpeed
    {
        get
        {
            float currentValue = 3;
            if (ClassStats.TryGetValue("StrafeSpeed", TokenType.Double, out DataToken tempToken))
            {
                currentValue = (float)tempToken.Double;
            }
            return currentValue;
        }
    }
    public bool AllowScaling
    {
        get
        {
            bool currentValue = false;
            if (ClassStats.TryGetValue("AllowScale", TokenType.Double, out DataToken tempToken))
            {
                currentValue = tempToken.Boolean;
            }
            return currentValue;
        }
    }
}
#else
public class SAPVP_Class : MonoBehaviour
{
}
#endif