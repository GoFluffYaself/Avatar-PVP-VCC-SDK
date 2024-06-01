using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

public struct IsLocalGameObject
{
    public GameObject Target;
    public bool IsLocal;
} 

public class SAPVP_IsLocal : MonoBehaviour, IPreprocessCallbackBehaviour, IEditorOnly
{
    [Header("Not ready for use")]
    public AnimationClip IsLocalClip;
    public IsLocalGameObject[] IsLocalGameObjects;
    public int PreprocessOrder { get; }
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
        if(!IsLocalClip)
        {
            IsLocalClip = new AnimationClip();
        }
        string buildPath = "";
        foreach (IsLocalGameObject gameObject in IsLocalGameObjects)
        {
            if(gameObject.Target)
            {
                //IsLocalClip.SetCurve();
                //SearchUtils.GetHierarchyPath(gameObject.Target, false);
                buildPath = buildPathToObject(gameObject.Target.transform);
                if(IsLocalClip)
                {
                    IsLocalClip.SetCurve(buildPath,typeof(GameObject), "m_IsActive", AnimationCurve.Constant(0,0, gameObject.IsLocal ? 1 : 0));
                }
            }
        }
        return false;
    }
    string buildPathToObject(Transform transformObject)
    {
        string currentPath = transformObject.name;
        while(transformObject.parent != null)
        {
            currentPath = transformObject.name + "/" + currentPath;
            if(transformObject.GetComponent<VRCAvatarDescriptor>())
            {
                break;
            }
            transformObject = transformObject.parent;
        }
        return currentPath;
    }
}
