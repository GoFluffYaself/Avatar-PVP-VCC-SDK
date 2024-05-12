using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorUtility = UnityEditor.EditorUtility;

public class PVPSetupWindow : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Avatar PVP SDK/Setup Layers")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PVPSetupWindow window = EditorWindow.GetWindow<PVPSetupWindow>();
        window.titleContent = new GUIContent("Avatar PVP SDK");
        window.Show();
    }
    bool _HasChecked = false;
    bool _CheckFailed = false;
    void OnGUI()
    {
        if (!UpdateLayers.AreLayersSetup())
        {

            EditorGUILayout.HelpBox("VRChat layers not setup! Plese run the setup wizard below!",MessageType.Error);
            // Taken and modified from VRChat SDK to replicate behavior
            bool doIt = EditorUtility.DisplayDialog("Setup Layers for VRChat",
                "This adds all VRChat layers to your project and pushes any custom layers down the layer list. If you have custom layers assigned to gameObjects, you'll need to reassign them. Are you sure you want to continue?",
                "Do it!", "Don't do it");
            if (doIt)
                UpdateLayers.SetupEditorLayers();
        }
        if(!_HasChecked)
        {
            //Based on https://forum.unity.com/threads/create-tags-and-layers-in-the-editor-using-script-both-edit-and-runtime-modes.732119/
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            for (int i = 22; i <= 31; i++)
            {
                if (layersProp.GetArrayElementAtIndex(i).stringValue == "")
                {
                    _CheckFailed = true;
                    break;
                }
            }
            _HasChecked = true;
        }
        if(_CheckFailed)
        {
            EditorGUILayout.HelpBox("Avatar PVP layers not setup! Plese run the setup wizard below!", MessageType.Warning);
        }
        if (GUILayout.Button("Setup layers for Avatar PVP"))
        {
            SetupAvatarPVPLayers();
        }
    }

    void SetupAvatarPVPLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        bool doIt = EditorUtility.DisplayDialog("Setup Layers for Avatar PVP",
                        "This adds all Avatar PVP layers to your project and will override custom layers (22 through 31). Are you sure you want to continue?",
                        "Setup", "Do not setup");
        if (doIt)
        {
            SerializedProperty iceLayer = layersProp.GetArrayElementAtIndex(22);
            iceLayer.stringValue = "Ice";
            SerializedProperty NewDOTLayer = layersProp.GetArrayElementAtIndex(23);
            NewDOTLayer.stringValue = "DOT";
            SerializedProperty StunLayer = layersProp.GetArrayElementAtIndex(24);
            StunLayer.stringValue = "Stun";
            SerializedProperty PoisonLayer = layersProp.GetArrayElementAtIndex(25);
            PoisonLayer.stringValue = "Poison";
            SerializedProperty SpeedLayer = layersProp.GetArrayElementAtIndex(26);
            SpeedLayer.stringValue = "Speed";
            SerializedProperty RadLayer = layersProp.GetArrayElementAtIndex(27);
            RadLayer.stringValue = "PUSH/RAD";
            SerializedProperty ShieldLayer = layersProp.GetArrayElementAtIndex(28);
            ShieldLayer.stringValue = "Shield";
            SerializedProperty HOTLayer = layersProp.GetArrayElementAtIndex(29);
            HOTLayer.stringValue = "HOT";
            SerializedProperty RepairLayer = layersProp.GetArrayElementAtIndex(30);
            RepairLayer.stringValue = "Repair";
            SerializedProperty FireLayer = layersProp.GetArrayElementAtIndex(31);
            FireLayer.stringValue = "Fire";
            tagManager.ApplyModifiedProperties();
            for (int i = 22; i <= 31; i++)
            {
                for (int i2 = 0; i2 <= 31; i2++)
                {
                    Physics.IgnoreLayerCollision(i2, i, true);
                }
            }
        }
        _HasChecked = false;
        _CheckFailed = false;
    }
}
