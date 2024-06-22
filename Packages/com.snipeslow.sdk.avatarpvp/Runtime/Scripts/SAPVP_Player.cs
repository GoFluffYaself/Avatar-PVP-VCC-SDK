#if !SAPVP_VRCSDK3_AVATARS && !SAPVP_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#define SAPVP_VRCSDK3_WORLDS
#else
#define SAPVP_VRCSDK3_AVATARS
#endif
#endif
using UnityEngine;
#if SAPVP_VRCSDK3_WORLDS
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using UdonSharp;

public class SAPVP_Player : UdonSharpBehaviour
{
    public float _MaxHealth = 2000;
    [UdonSynced]
    public float _Health = 0;
    public SAPVP_Manager Manager;
    public VRCObjectPool PlayerPool;
    public UnityEngine.UI.Image HealthBar;
    void Start()
    {
        _Health = _MaxHealth;
        if (Manager)
        {
            if(Manager.CombatLinkManager)
            {
                Manager.CombatLinkManager.Health = _MaxHealth;
                Manager.CombatLinkManager.MaxHealth = _MaxHealth;
                Manager.CombatLinkManager.AuxHealth = 0;
                Manager.CombatLinkManager.MaxAuxHealth = 20f;
                Manager.CombatLinkManager.Temperature = Manager.CombatLinkManager.DefaultTemperature;
                Manager.CombatLinkManager.Oxygen = 1;
                Manager.CombatLinkManager.MaxOxygen = 1;
            }
        }
    }
    private void OnEnable()
    {

    }
    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {

    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (player == null)
            return;
        if (!Manager)
            return;
        if(PlayerPool)
        {
            if(player.IsOwner(gameObject))
            {
                PlayerPool.Return(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    #region Overtime Effects
    public float FrostTime = 0;
    public void Frost()
    {
        if (FrostTime > 0) return;
        FrostTime = 6;
    }
    public bool IsFrozen()
    {
        return FrostTime > 0;
    }

    public float _RadDamage = 1;
    public void Radiation()
    {
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        if (Manager.Layer27Mode() != 1 && Manager.Layer27Mode() != 2)
        {
            _RadDamage = 1;
            return;
        }
        if (IsShielded())
        {
            _RadDamage = 1;
            return;
        }
        float currentValue = Manager.Classes[Manager.ActiveClass].RadRate;
            foreach (SAPVP_Plugin plugin in Manager.Plugins)
            {
                if (plugin)
                {
                    currentValue = plugin.RadRateMult * currentValue + plugin.RadRateAdd;
                }
            }
        TakeDamage(_RadDamage);
        _RadDamage = _RadDamage * currentValue;
    }

    public void Push(Vector3 position)
    {
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        if (Manager.Layer27Mode() == 1)
        {
            return;
        }
        VRCPlayerApi owner = Networking.GetOwner(gameObject);
        if (owner == null)
            return;
        if (!owner.IsValid())
            return;
        float currentValue = Manager.Classes[Manager.ActiveClass].PushPower;
        foreach (SAPVP_Plugin plugin in Manager.Plugins)
        {
            if (plugin)
            {
                currentValue = plugin.PushPowerMult * currentValue + plugin.PushPowerAdd;
            }
        }
        owner.SetVelocity(((owner.GetPosition()+ Vector3.up) -position).normalized * currentValue);
    }
        
    public float SpeedTime = 0;
    public void Speed()
    {
        if (SpeedTime > 0) return;
        SpeedTime = 8;
    }
    public bool IsBeingSped()
    {
        if (IsBeingCanceled())
        {
            SpeedTime = 0;
            return false;
        }
        return SpeedTime > 0;
    }

    public float HOTTime = 0;
    public void HOT()
    {
        if (HOTTime > 0) return;
        HOTTime = 5;
    }
    public bool IsHealing()
    {
        if (IsBeingCanceled())
        {
            HOTTime = 0;
            return false;
        }
        return HOTTime > 0;
    }


    public float RepairTime = 0;
    public void Repair()
    {
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        if (RepairTime > 0) return;
        float currentValue = Manager.Classes[Manager.ActiveClass].RepairAmount;
            foreach (SAPVP_Plugin plugin in Manager.Plugins)
            {
                if (plugin)
                {
                    currentValue = plugin.RepairAmountMult * currentValue + plugin.RepairAmountAdd;
                }
            }
        if (currentValue > 0)
        {
            TakeHeal(Mathf.Abs(currentValue));
        }
        else
        {
            TakeDamage(Mathf.Abs(currentValue));
        }
        RepairTime = 0.5f;
    }
    public bool CanRepair()
    {
        if (IsBeingCanceled())
            return false;
        return RepairTime <= 0;
    }

    public float DOTTime = 0;
    public void Bleed()
    {
        if (DOTTime > 0) return;
        DOTTime = 5;
    }
    public bool IsBleeding()
    {
        return DOTTime > 0;
    }
    public float StunTime = 0;
    public void Stun()
    {
        if (StunTime > 0) return;
        StunTime = 4;
    }
    public bool IsStunned()
    {
        return StunTime > 0;
    }
    public float FireTime = 0;
    public void Fire()
    {
        if (FireTime > 0) return;
        FireTime = 5;
    }
    public bool IsBurning()
    {
        return FireTime > 0;
    }

    public float ShieldTime = 0;
    public void Shield()
    {
        if (ShieldTime > 0) return;
        ShieldTime = 20;
    }
    public bool IsShielded()
    {
        if(IsBeingCanceled())
        {
            ShieldTime = 0;
            return false;
        }
        return ShieldTime > 0;
    }

    public float SlowTime = 0;
    public void Slow()
    {
        if (SlowTime > 0) return;
        SlowTime = 8;
    }
    public bool IsBeingSlowed()
    {
        return SlowTime > 0;
    }

    public float CancelTime = 0;
    public void Cancel()
    {
        if (CancelTime > 0) return;
        CancelTime = 18;
    }
    public bool IsBeingCanceled()
    {
        return CancelTime > 0;
    }

    public void Electric()
    {
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        ShieldTime = 0;
        float currentValue = Manager.Classes[Manager.ActiveClass].ElectricDMG;

        foreach (SAPVP_Plugin plugin in Manager.Plugins)
        {
            if (plugin)
            {
                currentValue = plugin.ElectricDMGMult * currentValue + plugin.ElectricDMGAdd;
            }
        }
        TakeDamage(currentValue);
    }

    public bool PoisonState = false;
    public void Poison()
    {
        PoisonState = true;
    }
    public bool IsPoisoned()
    {
        return PoisonState;
    }
    #endregion
    #region Move Tech
    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        if (!value)
            return;
        DoubleJump();
    }
    float _DoubleJumpCooldown = 0;
    void DoubleJump()
    {
        if (_DoubleJumpCooldown > Mathf.Epsilon)
            return;
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        float currentValue = Manager.Classes[Manager.ActiveClass].DoubleJumpSpeed;
        foreach (SAPVP_Plugin plugin in Manager.Plugins) if (plugin)
        {
            currentValue = plugin.DoubleJumpSpeedMult * currentValue + plugin.DoubleJumpSpeedAdd;
        }
        if (currentValue <= Mathf.Epsilon)
            return;
        VRCPlayerApi owner = Networking.GetOwner(gameObject);
        if(owner == null)
            return;
        if (!owner.IsValid())
            return;
        if (owner.IsPlayerGrounded())
            return;
        owner.SetVelocity((owner.GetVelocity().normalized + Vector3.up).normalized * currentValue);
        _DoubleJumpCooldown = 5;
    }

    float _DashCooldown = 0;
    float _DashPrimeWindow = 0;

    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        VRCPlayerApi owner = Networking.GetOwner(gameObject);
        if (owner == null)
            return;
        if (!owner.IsValid())
            return;
        if (!owner.IsPlayerGrounded())
            return;
        if (owner.IsUserInVR())
            return;
        if (Mathf.Abs(value) <= -0.75f)
        {
            PrimeDash(owner);
        }
        if (Mathf.Abs(value) >= 0.75f)
        {
            ReleaseDash(owner);
        }
    }
    public override void InputLookVertical(float value, UdonInputEventArgs args)
    {
        VRCPlayerApi owner = Networking.GetOwner(gameObject);
        if (owner == null)
            return;
        if (!owner.IsValid())
            return;
        if (!owner.IsPlayerGrounded())
            return;
        if (!owner.IsUserInVR())
            return;
        if (Mathf.Abs(value) <= -0.75f)
        {
            PrimeDash(owner);
        }
        if (Mathf.Abs(value) >= 0.75f)
        {
            ReleaseDash(owner);
        }
    }
    void ReleaseDash(VRCPlayerApi owner)
    {
        if (_DashCooldown > Mathf.Epsilon)
            return;
        if (_DashPrimeWindow <= Mathf.Epsilon)
            return;
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        float dashSpeed = Manager.Classes[Manager.ActiveClass].DashSpeed;
        foreach (SAPVP_Plugin plugin in Manager.Plugins)
        {
            if (plugin)
            {
                dashSpeed = plugin.DashSpeedMult * dashSpeed + plugin.DashSpeedAdd;
            }

        }
        if (dashSpeed <= Mathf.Epsilon)
            return;

        owner.SetVelocity(owner.GetRotation() * new Vector3(0,0.5f,0.5f) * dashSpeed);
        _DashCooldown = 5;
        _DashPrimeWindow = 0;
    }

    void PrimeDash(VRCPlayerApi owner)
    {
        if (_DashCooldown > Mathf.Epsilon)
            return;
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        if (Manager.Classes[Manager.ActiveClass].DashSpeed <= Mathf.Epsilon)
            return;
        _DashPrimeWindow = 0.25f;
    }

    #endregion
    private void FixedUpdate()
    {
        if (!Networking.IsNetworkSettled)
            return;

        VRCPlayerApi remotePlayer = Networking.GetOwner(gameObject);
        if (remotePlayer == null)
            return;

        VRCPlayerApi.TrackingData trackingData =remotePlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        transform.SetPositionAndRotation(trackingData.position, remotePlayer.GetRotation());
        if (!remotePlayer.IsValid())
            return;
        if (!Manager)
            return;
        if (remotePlayer.IsPlayerGrounded())
        {
            _DoubleJumpCooldown = Mathf.Max(_DoubleJumpCooldown - Time.deltaTime, 0);
            _DashCooldown = Mathf.Max(_DashCooldown - Time.deltaTime, 0);
            _DashPrimeWindow = Mathf.Max(_DashPrimeWindow - Time.deltaTime, 0);
        }

        if (Manager.Classes[Manager.ActiveClass])
        {

            float ResultJumpSpeed = Manager.Classes[Manager.ActiveClass].JumpSpeed;
            float ResultStrafeSpeed = Manager.Classes[Manager.ActiveClass].StrafeSpeed;
            float ResultRunSpeed = Manager.Classes[Manager.ActiveClass].RunSpeed;
            float ResultWalkSpeed = Manager.Classes[Manager.ActiveClass].WalkSpeed;
            // PLUGIN SYSTEM START
            float dotDamage = Manager.Classes[Manager.ActiveClass].DOTDMG;
            float dotEffectRatio = Manager.Classes[Manager.ActiveClass].DOTEffectRatio;
            float hotAmount = Manager.Classes[Manager.ActiveClass].HOTAmount;
            float fireDamage = Manager.Classes[Manager.ActiveClass].FireDMG;
            float poisonDamage = Manager.Classes[Manager.ActiveClass].PoisonDMG;
            float iceDamage = Manager.Classes[Manager.ActiveClass].IceDMG;
            float iceEffectRatio = Manager.Classes[Manager.ActiveClass].IceEffectRatio;
            float stunEffectRatio = Manager.Classes[Manager.ActiveClass].StunEffectRatio;
            float slowEffectRatio = Manager.Classes[Manager.ActiveClass].SlowEffectRatio;
            float speedEffectRatio = Manager.Classes[Manager.ActiveClass].SpeedEffectRatio;
            foreach (SAPVP_Plugin plugin in Manager.Plugins)
            {
                if (plugin)
                {
                    dotDamage = plugin.DOTDMGMult * dotDamage + plugin.DOTDMGAdd;
                    dotEffectRatio = plugin.DOTEffectRatioMult * dotEffectRatio + plugin.DOTEffectRatioAdd;
                    hotAmount = plugin.HOTAmountMult * hotAmount + plugin.HOTAmountAdd;
                    fireDamage = plugin.FireDMGMult * fireDamage + plugin.FireDMGAdd;
                    poisonDamage = plugin.PoisonDMGMult * poisonDamage + plugin.PoisonDMGAdd;
                    iceDamage = plugin.IceDMGMult * iceDamage + plugin.IceDMGAdd;
                    iceEffectRatio = plugin.IceEffectRatioMult * iceEffectRatio + plugin.IceEffectRatioAdd;
                    stunEffectRatio = plugin.StunEffectRatioMult * stunEffectRatio + plugin.StunEffectRatioAdd;
                    slowEffectRatio = plugin.SlowEffectRatioMult * slowEffectRatio + plugin.SlowEffectRatioAdd;
                    speedEffectRatio = plugin.SpeedEffectRatioMult * speedEffectRatio + plugin.SpeedEffectRatioAdd;
                    ResultJumpSpeed = plugin.JumpSpeedMult * ResultJumpSpeed + plugin.JumpSpeedAdd;
                    ResultRunSpeed = plugin.RunSpeedMult * ResultRunSpeed + plugin.RunSpeedAdd;
                    ResultWalkSpeed = plugin.WalkSpeedMult * ResultWalkSpeed + plugin.WalkSpeedAdd;
                    ResultStrafeSpeed = plugin.StrafeSpeedMult * ResultStrafeSpeed + plugin.StrafeSpeedAdd;
                }
            }
            //  PLUGIN SYSTEM END
            // ...
            if (IsShielded())
            {
                ShieldTime -= Time.deltaTime;
            }
            if(IsBeingCanceled())
            {
                CancelTime -= Time.deltaTime;
            }
            if (!CanRepair())
            {
                RepairTime -= Time.deltaTime;
            }

            if (IsHealing())
            {
                TakeHeal(hotAmount * Time.deltaTime);
                PoisonState = false;
                HOTTime -= Time.deltaTime;
            }

            if (IsBleeding())
            {
                TakeDamage(dotDamage * Time.deltaTime);
                ResultJumpSpeed *= dotEffectRatio;
                ResultStrafeSpeed *= dotEffectRatio;
                ResultRunSpeed *= dotEffectRatio;
                ResultWalkSpeed *= dotEffectRatio;
                DOTTime -= Time.deltaTime;
            }
            if (IsBurning())
            {
                TakeDamage(fireDamage * Time.deltaTime);
                FireTime -= Time.deltaTime;
            }
            if (IsStunned())
            {
                ResultJumpSpeed *= stunEffectRatio;
                ResultStrafeSpeed *= stunEffectRatio;
                ResultRunSpeed *= stunEffectRatio;
                ResultWalkSpeed *= stunEffectRatio;
                StunTime -= Time.deltaTime;
            }
            if(IsBeingSped())
            {
                ResultJumpSpeed *= speedEffectRatio;
                ResultStrafeSpeed *= speedEffectRatio;
                ResultRunSpeed *= speedEffectRatio;
                ResultWalkSpeed *= speedEffectRatio;
                SpeedTime -= Time.deltaTime;
            }
            if (IsBeingSlowed())
            {
                ResultJumpSpeed *= slowEffectRatio;
                ResultStrafeSpeed *= slowEffectRatio;
                ResultRunSpeed *= slowEffectRatio;
                ResultWalkSpeed *= slowEffectRatio;
                SlowTime -= Time.deltaTime;
            }
            if (IsFrozen())
            {
                TakeDamage(iceDamage * Time.deltaTime);
                ResultJumpSpeed *= iceEffectRatio;
                ResultStrafeSpeed *= iceEffectRatio;
                ResultRunSpeed *= iceEffectRatio;
                ResultWalkSpeed *= iceEffectRatio;
                FrostTime -= Time.deltaTime;
            }
            if (IsPoisoned())
            {
                TakeDamage(poisonDamage * Time.deltaTime);
            }
            // ...
            remotePlayer.SetJumpImpulse(ResultJumpSpeed);
            remotePlayer.SetStrafeSpeed(ResultStrafeSpeed);
            remotePlayer.SetWalkSpeed(ResultWalkSpeed);
            remotePlayer.SetRunSpeed(ResultRunSpeed);
            if (_Health <= 0)
            {
                Death();
            }
            if (Manager.CombatLinkManager && remotePlayer.isLocal)
            {
                Manager.CombatLinkManager.Health = _Health;
                Manager.CombatLinkManager.AuxHealth = ShieldTime;

                Manager.CombatLinkManager.Boost = SpeedTime;
                Manager.CombatLinkManager.Slow = SlowTime;
                Manager.CombatLinkManager.Stun = StunTime;
                Manager.CombatLinkManager.Healing = HOTTime;

                Manager.CombatLinkManager.Bleed = DOTTime;
                Manager.CombatLinkManager.Burn = FireTime;
                Manager.CombatLinkManager.Temperature = Manager.CombatLinkManager.DefaultTemperature + (Manager.CombatLinkManager.DefaultTemperature*FireTime);
                Manager.CombatLinkManager.Poison = IsPoisoned() ? 1 : 0;
                Manager.CombatLinkManager.Frost = FrostTime;

            }
            if(HealthBar)
            {
                HealthBar.fillAmount = _Health / _MaxHealth;
            }
        }
    }
    void Death(bool respawn = true)
    {
        _Health = _MaxHealth;
        _RadDamage = 1;

        PoisonState = false;
        FrostTime = 0;
        SlowTime = 0;
        SpeedTime = 0;

        StunTime = 0;
        FireTime = 0;
        DOTTime = 0;
        HOTTime = 0;

        CancelTime = 0;
        ShieldTime = 0;
        RepairTime = 0;

        if (respawn)
        {
            Networking.LocalPlayer.Respawn();
        }

    }
    public void TakeDamageChest(float damage)
    {
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;

        foreach (SAPVP_Plugin plugin in Manager.Plugins)
        {
            if (plugin)
            {
                damage = plugin.ChestDMGMult * damage + plugin.ChestDMGAdd;
            }
        }

        if (IsShielded())
        {
            float chestDMGShieldRatio = Manager.Classes[Manager.ActiveClass].ChestDMGShieldRatio;
            foreach (SAPVP_Plugin plugin in Manager.Plugins)
            {
                if (plugin)
                {
                    chestDMGShieldRatio = plugin.ChestDMGShieldRatioMult * chestDMGShieldRatio + plugin.ChestDMGShieldRatioAdd;
                }
            }
            damage *= chestDMGShieldRatio;
        }

        TakeDamage(damage);
    }
    public void TakeDamageHead(float damage)
    {
        if (!Manager)
            return;
        if (!Manager.Classes[Manager.ActiveClass])
            return;
        foreach (SAPVP_Plugin plugin in Manager.Plugins)
        {
            if (plugin)
            {
                damage = plugin.HeadDMGMult * damage + plugin.HeadDMGAdd;
            }
        }
        if (IsShielded())
        {
            float headDMGShieldRatio = Manager.Classes[Manager.ActiveClass].HeadDMGShieldRatio;
            foreach (SAPVP_Plugin plugin in Manager.Plugins)
            {
                if (plugin)
                {
                    headDMGShieldRatio = plugin.HeadDMGShieldRatioMult * headDMGShieldRatio + plugin.HeadDMGShieldRatioAdd;
                }
            }
            damage *= headDMGShieldRatio;
        }
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        _Health -= Mathf.Max(damage,0);
        if (_Health <= 0)
        {
            Death();
        }
    }
    public void TakeHeal(float heal)
    {
        if(_Health < _MaxHealth)
        {
            _Health += heal;
            _Health = Mathf.Clamp(_Health, 0, _MaxHealth);
        }

        if (_Health <= 0)
        {
            Death();
        }
    }
    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (player == null)
            return;
        if (player.isLocal)
            return;
        _Health = _MaxHealth;
        _RadDamage = 1;

        PoisonState = false;
        FrostTime = 0;
        SlowTime = 0;
        SpeedTime = 0;

        StunTime = 0;
        FireTime = 0;
        DOTTime = 0;
        HOTTime = 0;

        CancelTime = 0;
        ShieldTime = 0;
        RepairTime = 0;
    }
}
#else
public class SAPVP_Player : MonoBehaviour
{
}
#endif