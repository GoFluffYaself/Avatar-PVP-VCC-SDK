using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;
using System.Linq;


[System.Serializable]
public struct ParticleObjects
{
    [SerializeField]
    public ParticleSystem Target;


    public bool PVPEnabled;

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


}

public class SAPVP_ParticleConfig : MonoBehaviour, IPreprocessCallbackBehaviour, IEditorOnly
{
    public ParticleObjects[] TargetParticleObjects;

    private void Start()
    {
        Process();
    }
    [ContextMenu("[SAPVP] Compile settings into Particle and delete component")]
    void contextMenuCompileDelete()
    {
        Process();
#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(this);
#else
            DestroyImmediate(this);
#endif
    }

    [ContextMenu("[SAPVP] Compile settings into Particle")]
    void contextMenuCompile()
    {
        Process();
    }

    public bool Process()
    {
#if UNITY_EDITOR
        List<UnityEngine.Object> ToRecordList = new List<UnityEngine.Object>();
        foreach (var obj in TargetParticleObjects)
        {
            ToRecordList.Add(obj.Target);
        }
        UnityEditor.Undo.RecordObjects(ToRecordList.ToArray(), "Avatar PVP Particle Config Compile");
#endif
        foreach (var TargetParticleObject in TargetParticleObjects)
        {
            if (!TargetParticleObject.Target)
            {
                continue;
            }
            if (TargetParticleObject.Target)
            {
                var col = TargetParticleObject.Target.collision;
                col.enabled = true;
                col.sendCollisionMessages = TargetParticleObject.PVPEnabled;
                col.mode = ParticleSystemCollisionMode.Collision3D;
                col.type = ParticleSystemCollisionType.World;
                col.quality = ParticleSystemCollisionQuality.High;
                int CollisionBitMask = 0;

                if (TargetParticleObject.CollideWithDefaultLayer)
                {
                    CollisionBitMask |= (1 << 0);
                }

                if (TargetParticleObject.CollideWithRemotePlayer)
                {
                    CollisionBitMask |= (1 << 9);
                }
                if (TargetParticleObject.CollideWithLocalPlayer)
                {
                    CollisionBitMask |= (1 << 10);
                }

                if (TargetParticleObject.Cancel)
                {
                    CollisionBitMask |= (1 << 14);
                }
                if (TargetParticleObject.Electric)
                {
                    CollisionBitMask |= (1 << 15);
                }
                if (TargetParticleObject.Slow)
                {
                    CollisionBitMask |= (1 << 16);
                }
                if (TargetParticleObject.Damage)
                {
                    CollisionBitMask |= (1 << 17);
                }
                if (TargetParticleObject.Ice)
                {
                    CollisionBitMask |= (1 << 22);
                }
                if (TargetParticleObject.DamageOverTime)
                {
                    CollisionBitMask |= (1 << 23);
                }
                if (TargetParticleObject.Stun)
                {
                    CollisionBitMask |= (1 << 24);
                }
                if (TargetParticleObject.Poison)
                {
                    CollisionBitMask |= (1 << 25);
                }
                if (TargetParticleObject.Speed)
                {
                    CollisionBitMask |= (1 << 26);
                }
                if (TargetParticleObject.PushRad)
                {
                    CollisionBitMask |= (1 << 27);
                }
                if (TargetParticleObject.Shield)
                {
                    CollisionBitMask |= (1 << 28);
                }
                if (TargetParticleObject.HealOverTime)
                {
                    CollisionBitMask |= (1 << 29);
                }
                if (TargetParticleObject.Repair)
                {
                    CollisionBitMask |= (1 << 30);
                }
                if (TargetParticleObject.Burn)
                {
                    CollisionBitMask |= (1 << 31);
                }
                col.collidesWith = CollisionBitMask;
            }
        }
        return true;

    }

    public bool OnPreprocess()
    {
        return Process();

    }

    public int PreprocessOrder { get; }
}