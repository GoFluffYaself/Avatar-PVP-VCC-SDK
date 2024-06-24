#if !SAPVP_VRCSDK3_AVATARS && !SAPVP_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#define SAPVP_VRCSDK3_WORLDS
#else
#define SAPVP_VRCSDK3_AVATARS
#endif
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct ParticleObjects
{
    [SerializeField]
    public ParticleSystem Target;

    [SerializeField]
    public ParticleSystem SingleShotTrigger;

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
#if UNITY_EDITOR
    [MenuItem("GameObject/Avatar PVP SDK/Create Single Shot Trigger", false, 20)]
    public static void ContextMenu_AddSingleShot(MenuCommand menuCommand){
        GameObject go = new GameObject("SingleShotTrigger");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.startLifetime = 0.1f;
        main.playOnAwake = true;
        main.startSpeed = 0.01f;
        ParticleSystem.EmissionModule emissionModule = ps.emission;
        emissionModule.enabled = true;
        emissionModule.rateOverTime = 0;
        emissionModule.rateOverDistance = 0;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0, 1, 1, 0.01f);
        emissionModule.SetBursts(new ParticleSystem.Burst[] { burst });
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = false;
        
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

        Selection.activeObject = go;
    }
    [MenuItem("CONTEXT/ParticleSystem/Avatar PVP SDK/Make Single Shot Trigger", false, 15)]
    public static void ContextMenu_MakeSingleShot(MenuCommand menuCommand)
    {
        GameObject pgo = (menuCommand.context as ParticleSystem).transform.parent.gameObject;
        ParticleSystem subps = (menuCommand.context as ParticleSystem);
        if (!pgo)
        {
            Debug.LogWarning("Please parent this object before using this command.", (menuCommand.context as ParticleSystem).gameObject);
            return;
        }
        ParticleSystem.EmissionModule subEmission = subps.emission;
        subEmission.enabled = false;
        subEmission.rateOverTime = 0;
        subEmission.rateOverDistance = 0;
        ParticleSystem.Burst subburst = new ParticleSystem.Burst(0, 1, 1, 0.01f);
        subEmission.SetBursts(new ParticleSystem.Burst[] { subburst });
        GameObject go = new GameObject((menuCommand.context as ParticleSystem).gameObject.name);
        (menuCommand.context as ParticleSystem).gameObject.name = (menuCommand.context as ParticleSystem).gameObject.name+"_SingleShot";
        GameObjectUtility.SetParentAndAlign(go, pgo);
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.startLifetime = 0.1f;
        main.playOnAwake = true;
        main.startSpeed = 0.01f;
        ParticleSystem.EmissionModule emissionModule = ps.emission;
        emissionModule.enabled = true;
        emissionModule.rateOverTime = 0;
        emissionModule.rateOverDistance = 0;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0, 1, 1, 0.01f);
        emissionModule.SetBursts(new ParticleSystem.Burst[] { burst });
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = false;
        ParticleSystem.SubEmittersModule se = ps.subEmitters;
        se.AddSubEmitter((menuCommand.context as ParticleSystem), ParticleSystemSubEmitterType.Birth, ParticleSystemSubEmitterProperties.InheritNothing);
        se.enabled = true;
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

        Selection.activeObject = go;
    }
#endif
    private void Start()
    {
        Process();
    }
    [ContextMenu("[SAPVP] Compile settings into Particle and delete component")]
    void ContextMenu_CompileDelete()
    {
        Process();
#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(this);
#else
            DestroyImmediate(this);
#endif
    }

    [ContextMenu("[SAPVP] Compile settings into Particle")]
    void ContextMenu_Compile()
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
            if(TargetParticleObject.SingleShotTrigger)
            {
                ParticleSystem.SubEmittersModule se = TargetParticleObject.SingleShotTrigger.subEmitters;
                se.AddSubEmitter(TargetParticleObject.Target, ParticleSystemSubEmitterType.Birth,ParticleSystemSubEmitterProperties.InheritNothing);
                se.enabled = true;
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