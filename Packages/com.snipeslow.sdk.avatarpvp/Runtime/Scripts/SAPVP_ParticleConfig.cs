using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;
[RequireComponent(typeof(ParticleSystem))]
public class SAPVP_ParticleConfig : MonoBehaviour, IPreprocessCallbackBehaviour, IEditorOnly
{
    public ParticleSystem TargetParticleSystem;

    /*[Header("Animator Config")]
    public AnimationClip clip
    public bool Local = true;
    public bool Remote = true;*/

    [Header("Non-PVP collisions")]
    public bool CollideWithDefaultLayer;
    public bool CollideWithRemotePlayer;
    public bool CollideWithLocalPlayer;

    [Header("Damage Types")]
    public bool Cancel;
    public bool Electric;
    public bool Slow;
    public bool Damage;
    public bool Ice;
    public bool DamageOverTime;
    public bool Stun;
    public bool Poison;
    public bool Speed;
    public bool PushRad;
    public bool Shield;
    public bool HealOverTime;
    public bool Repair;
    public bool Burn;
    bool Process()
    {
        if(!TargetParticleSystem)
        {
            TargetParticleSystem = GetComponent<ParticleSystem>();
        }
        if (TargetParticleSystem)
        {
            var col = TargetParticleSystem.collision;
            col.enabled = true;
            col.sendCollisionMessages = true;
            col.mode = ParticleSystemCollisionMode.Collision3D;
            col.type = ParticleSystemCollisionType.World;
            int CollisionBitMask = 0;

            if (CollideWithDefaultLayer)
            {
                CollisionBitMask |= (1 << 0);
            }

            if (CollideWithRemotePlayer)
            {
                CollisionBitMask |= (1 << 9);
            }
            if (CollideWithLocalPlayer)
            {
                CollisionBitMask |= (1 << 10);
            }

            if (Cancel)
            {
                CollisionBitMask |= (1 << 14);
            }
            if (Electric)
            {
                CollisionBitMask |= (1 << 15);
            }
            if (Slow)
            {
                CollisionBitMask |= (1 << 16);
            }
            if (Damage)
            {
                CollisionBitMask |= (1 << 17);
            }
            if (Ice)
            {
                CollisionBitMask |= (1 << 22);
            }
            if (DamageOverTime)
            {
                CollisionBitMask |= (1 << 23);
            }
            if (Stun)
            {
                CollisionBitMask |= (1 << 24);
            }
            if (Poison)
            {
                CollisionBitMask |= (1 << 25);
            }
            if (Speed)
            {
                CollisionBitMask |= (1 << 26);
            }
            if (PushRad)
            {
                CollisionBitMask |= (1 << 27);
            }
            if (Shield)
            {
                CollisionBitMask |= (1 << 28);
            }
            if (HealOverTime)
            {
                CollisionBitMask |= (1 << 29);
            }
            if (Repair)
            {
                CollisionBitMask |= (1 << 30);
            }
            if (Burn)
            {
                CollisionBitMask |= (1 << 31);
            }
            col.collidesWith = CollisionBitMask;

            /*if(clip)
            {
                clip.set
            }*/
            return true;
        }
        Debug.LogError("Particle invalid! Aborting SDK upload!");
        return false;

    }
    bool IPreprocessCallbackBehaviour.OnPreprocess()
    {
        return Process();

    }

    public int PreprocessOrder { get; }
}
