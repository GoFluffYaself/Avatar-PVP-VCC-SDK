using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

[System.Serializable]
public struct IsLocalGameObject
{
    [SerializeField]
    public GameObject Target;
    [SerializeField]
    public bool IsLocal;
} 

public class SAPVP_IsLocal : MonoBehaviour, IPreprocessCallbackBehaviour, IEditorOnly
{
    [Header("Not ready for use")]
    public bool AutoClear = false;
    public AnimationClip IsLocalClip;

    [SerializeField]
    public IsLocalGameObject[] IsLocalGameObjects;
    public int PreprocessOrder { get; }
    private void Start()
    {
        Process();
    }
    GameObject FindAvatarRoot()
    {
        VRC_AvatarDescriptor descriptor = GetComponentInParent<VRC_AvatarDescriptor>();
        if(descriptor)
        {
            return descriptor.gameObject;
        }
        return null;
    }
    public bool OnPreprocess()
    {
        return Process();

    }

    [ContextMenu("[SAPVP] Compile settings into Animation Clip and delete component")]
    void contextMenuCompileDelete()
    {
        Process();
#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(this);
#else
            DestroyImmediate(this);
#endif
    }

    [ContextMenu("[SAPVP] Compile settings into Animation Clip")]
    void contextMenuCompile()
    {
        Process();
    }

    public bool Process()
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(IsLocalClip, "Undo Animation Clip Compile");
#endif
        if (!IsLocalClip)
        {
            return false;
        }
        if (AutoClear)
        {
            IsLocalClip.ClearCurves();
        }
        string buildPath = "";
        foreach (IsLocalGameObject lgo in IsLocalGameObjects)
        {
            if(lgo.Target)
            {
                //IsLocalClip.SetCurve();
                //SearchUtils.GetHierarchyPath(gameObject.Target, false);
                buildPath = buildPathToObject(lgo.Target.transform);
                if(IsLocalClip)
                {
                    IsLocalClip.SetCurve(buildPath,typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0,0, lgo.IsLocal ? 1 : 0));
                }
            }
        }
        return true;
    }
    string buildPathToObject(Transform transformObject)
    {
        string currentPath = transformObject.name;
        while(transformObject.parent != null)
        {
            if (transformObject.parent.GetComponent<VRCAvatarDescriptor>())
            {
                break;
            }
            currentPath = transformObject.parent.name + "/" + currentPath;
            transformObject = transformObject.parent;
        }
        return currentPath;
    }
}
