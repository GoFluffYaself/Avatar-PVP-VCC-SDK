using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarPVPAddToScene
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Avatar PVP SDK/Add Avatar PVP world prefab into scene")]
    static void Init()
    {
        var GO = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.snipeslow.avatarpvp/Runtime/Prefabs/SAPVP Manager.prefab");
        if(GO != null)
        {
            Selection.activeObject = PrefabUtility.InstantiatePrefab(GO);
            Selection.activeGameObject.transform.position = Vector3.zero;
            Selection.activeGameObject.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("Avatar PVP prefab failed to be added to scene!");
        }
    }
}
