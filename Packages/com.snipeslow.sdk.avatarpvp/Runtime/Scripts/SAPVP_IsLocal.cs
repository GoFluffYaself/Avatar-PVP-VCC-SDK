using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Search;
using UnityEngine;
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
    public AnimatorController FXController;
    public IsLocalGameObject[] IsLocalGameObjects;
    public int PreprocessOrder { get; }

    public bool OnPreprocess()
    {
        foreach (IsLocalGameObject gameObject in IsLocalGameObjects)
        {
            if(gameObject.Target)
            {
                //IsLocalClip.SetCurve();
                //SearchUtils.GetHierarchyPath(gameObject.Target, false);
                
            }
        }
        return false;
    }

}
