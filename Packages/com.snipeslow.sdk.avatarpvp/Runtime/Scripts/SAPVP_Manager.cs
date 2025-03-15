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
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using UdonSharp;
//TODO: CLEANUP TO NAMKING CONVENTION
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SAPVP_Manager : UdonSharpBehaviour
{
    [Header("Not ready yet")]
    public SAPVP_Plugin[] Plugins;
    [UdonSynced, HideInInspector,Header("Do not touch anything below this")]
    public string ClassSet = "Default";
    VRCUrl _defaultDatabaseUrl = new VRCUrl("https://api.snipeslow.dev/avatar-pvp/");
    [UdonSynced]
    VRCUrl CustomDatabaseURL = new VRCUrl("");
    public VRCUrlInputField urlInputField;
    [UdonSynced]
    string JSONData = "";
    public string DefaultJSONData = "";
    public Transform HudTransform;
    public VRCObjectPool PlayerPool;
    public CombatLinkManager CombatLinkManager;
    DataDictionary _database = new DataDictionary();
    [HideInInspector]
    public SAPVP_Player LocalPlayer;
    void Start()
    {
        //No longer used directly.
    }

    #region Core Functions
    [UdonSynced]
    public bool AllowMasterControl = true;
    public void ToggleMasterControl()
    {
        AllowMasterControl = !AllowMasterControl;
        RequestSerialization();
    }
    public bool IsAllowedToManage(VRCPlayerApi targetPlayer)
    {
        bool isAllowed = false;
        foreach (SAPVP_Plugin plugin in Plugins)
        {
            if (plugin)
            {
                isAllowed = plugin.IsAllowedToManage(targetPlayer);
                if(isAllowed)
                {
                    break;
                }
            }
        }
        return (targetPlayer.isMaster && AllowMasterControl) || targetPlayer.isInstanceOwner || isAllowed;
    }
    public void SetClass(int classIndex)
    {
        Debug.Log("WE SHOULD BE CHANGING CLASSES NOW");
        //LocalPlayer.Class = Classes[classIndex];
        if (Classes[ActiveClass])
        {
            if (Classes[ActiveClass].ClassImage)
            {
                Classes[ActiveClass].ClassImage.color = Color.white;
            }
        }
        if (Classes[classIndex])
        {
            if (Classes[classIndex].ClassImage)
            {
                Classes[classIndex].ClassImage.color = Color.cyan;
            }
        }
        ActiveClass = classIndex;

        foreach (SAPVP_Plugin plugin in Plugins)
        {
            if (plugin)
            {
                if(VRCJson.TryDeserializeFromJson(Classes[classIndex].ClassCode, out DataToken tempToken))
                {
                    plugin.ClassData = tempToken.DataDictionary;
                }
            }
        }
    }
    
    [UdonSynced]
    string _classCode;

    public void MasterSyncClasses()
    {
        if (!IsAllowedToManage(Networking.LocalPlayer)) return;
        if (urlInputField)
        {
            CustomDatabaseURL = urlInputField.GetUrl();
        }

        if (CustomDatabaseURL.ToString().Length > 0)
        {

            VRCStringDownloader.LoadUrl(CustomDatabaseURL, (IUdonEventReceiver)this);
        }
        else
        {

            VRCStringDownloader.LoadUrl(_defaultDatabaseUrl, (IUdonEventReceiver)this);
        }
        RequestSerialization();
        //SendCustomNetworkEvent(NetworkEventTarget.All,"SyncClasses");
    }
    public void SyncClasses()
    {
        Debug.Log("CUSTOM DATABSE URL LENGTH:" + CustomDatabaseURL.ToString().Length);
    }

    public override void OnStringLoadError(IVRCStringDownload result)
    {
        if (!IsAllowedToManage(Networking.LocalPlayer)) return;
        //Backup snapshot of database incase of failure 
        JSONData = DefaultJSONData;
        RequestSerialization();
        FormDatabase();
    }

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        if (!IsAllowedToManage(Networking.LocalPlayer)) return;
        JSONData = result.Result;
        RequestSerialization();
        FormDatabase();
    }

    public override void OnDeserialization()
    {
        FormDatabase();
    }
    #endregion
    #region UI
    #region Local Toggles
    public UnityEngine.UI.Image HUDToggleImage;
    public void ToggleHUD()
    {
        if (!Networking.LocalPlayer.IsValid()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (HudTransform)
        {
            HudTransform.gameObject.SetActive(!HudTransform.gameObject.activeSelf);
            if (HUDToggleImage)
            {
                HUDToggleImage.color = HudTransform.gameObject.activeSelf ? Color.cyan : Color.white;
            }
        }
    }
    public UnityEngine.UI.Image CombatLinkToggleImage;
    public void ToggleCombatLink()
    {
        if (!Networking.LocalPlayer.IsValid()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (CombatLinkManager)
        {
            CombatLinkManager.Active = !CombatLinkManager.Active;
            if (CombatLinkToggleImage)
            {
                CombatLinkToggleImage.color = CombatLinkManager.Active ? Color.cyan : Color.white;
            }
        }
    }
    #endregion
    #region Master Toggles
    public GameObject MasterTogglePanel;
    [UdonSynced]// Push(0), Radiation(1), or Both(2)
    int Layer27Behaviour = 0;
    public int Layer27Mode()
    {
        return Layer27Behaviour;
    }
    public UnityEngine.UI.Image PushToggleImage;
    public void TogglePush()
    {
        if (!IsAllowedToManage(Networking.LocalPlayer)) return;
        if (!Networking.LocalPlayer.IsValid()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        switch (Layer27Behaviour)
        {
            case 0:
                Layer27Behaviour = 1;
                break;
            case 1:
                Layer27Behaviour = 0;
                break;
            case 2:
                Layer27Behaviour = 0;
                break;
        }
        FormDatabase();
        RequestSerialization();
    }
    public UnityEngine.UI.Image RadiationToggleImage;
    public void ToggleRadiation()
    {
        if (!IsAllowedToManage(Networking.LocalPlayer)) return;
        if (!Networking.LocalPlayer.IsValid()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        switch (Layer27Behaviour)
        {
            case 0:
                Layer27Behaviour = 1;
                break;
            case 1:
                Layer27Behaviour = 0;
                break;
            case 2:
                Layer27Behaviour = 1;
                break;
        }
        FormDatabase();
        RequestSerialization();
    }
    public UnityEngine.UI.Image PushAndRadiationToggleImage;
    public void ToggleBoth()
    {
        if (!IsAllowedToManage(Networking.LocalPlayer)) return;
        if (!Networking.LocalPlayer.IsValid()) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        switch (Layer27Behaviour)
        {
            case 0:
                Layer27Behaviour = 2;
                break;
            case 1:
                Layer27Behaviour = 2;
                break;
            case 2:
                Layer27Behaviour = 0;
                break;
        }
        FormDatabase();
        RequestSerialization();
    }
    #endregion
    #region Class Board
    public void FormDatabase()
    {
        if (RadiationToggleImage && PushToggleImage && PushAndRadiationToggleImage)
        {
            switch (Layer27Behaviour)
            {
                case 0:
                    PushToggleImage.color = Color.cyan;
                    RadiationToggleImage.color = Color.white;
                    PushAndRadiationToggleImage.color = Color.white;
                    break;
                case 1:
                    PushToggleImage.color = Color.white;
                    RadiationToggleImage.color = Color.cyan;
                    PushAndRadiationToggleImage.color = Color.white;
                    break;
                case 2:
                    PushToggleImage.color = Color.white;
                    RadiationToggleImage.color = Color.white;
                    PushAndRadiationToggleImage.color = Color.cyan;
                    break;
                default:
                    PushToggleImage.color = Color.white;
                    RadiationToggleImage.color = Color.white;
                    PushAndRadiationToggleImage.color = Color.white;
                    break;

            }
        }
        for (int i = 0; i < ClassSets.Length; i++)
        {
            if (ClassSets[i])
            {
                ClassSets[i].ClassSetImage.color = i == ActiveClassSet ? Color.cyan : Color.white;
            }
        }
        if (VRCJson.TryDeserializeFromJson(JSONData, out DataToken tempToken))
        {
            _database = tempToken.DataDictionary;
            SetupClassSetButtons();
            SetupClassButtons();
        }
    }

    public GameObject ClassSetPanel;
    [UdonSynced, HideInInspector]
    public int ActiveClassSet = 0;
    public SAPVP_ClassSet[] ClassSets;
    public void SetupClassSetButtons()
    {
        if (_database.TryGetValue("ClassSets", TokenType.DataDictionary, out DataToken tempToken))
        {
            DataList Keys = tempToken.DataDictionary.GetKeys();
            for (int i = 0; i < ClassSets.Length; i++)
            {
                if (i < Keys.Count)
                {
                    Debug.Log(Keys[i]);

                    if (ClassSets[i])
                    {
                        if (tempToken.DataDictionary.TryGetValue(Keys[i], TokenType.DataDictionary, out DataToken tempClassSetToken))
                        {
                            if(tempClassSetToken.DataDictionary.TryGetValue("DisplayName", TokenType.String, out DataToken tempDisplayNameToken))
                            {
                                ClassSets[i].ClassSetName = tempDisplayNameToken.String;
                                ClassSets[i].ClassSet = Keys[i].String;
                                ClassSets[i].ID = i;
                                ClassSets[i].gameObject.SetActive(true);
                                ClassSets[i].UpdateUI();
                                ClassSets[i].RequestSerialization();
                            }
                            else
                            {
                                ClassSets[i].gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            ClassSets[i].gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (ClassSets[i])
                    {
                        ClassSets[i].gameObject.SetActive(false);
                        ClassSets[i].UpdateUI();
                        ClassSets[i].RequestSerialization();
                    }
                }
            }
        }
    }
    [HideInInspector]
    public int ActiveClass = 0;
    public SAPVP_Class[] Classes;
    public void SetupClassButtons()
    {
        if (_database.TryGetValue("Classes", TokenType.DataDictionary, out DataToken tempToken))
        {
            if (tempToken.DataDictionary.TryGetValue(ClassSet, TokenType.DataDictionary, out DataToken tempClasSetToken))
            {
                DataList Keys = tempClasSetToken.DataDictionary.GetKeys();
                Debug.Log("CLASS COUNT " + Classes.Length);
                if (Classes.Length == 0)
                {
                    ActiveClass = 0;
                }
                for (int i = 0; i < Classes.Length; i++)
                {
                    if(i < Keys.Count)
                    {
                        Debug.Log("Count" + i);
                        if (Classes[i])
                        {
                            if (tempClasSetToken.DataDictionary.TryGetValue(Keys[i], TokenType.DataDictionary, out DataToken tempClassToken))
                            {
                                if(VRCJson.TrySerializeToJson(tempClassToken, JsonExportType.Minify, out DataToken tempClassCodeToken))
                                {
                                    Classes[i].ClassCode = tempClassCodeToken.String;
                                    Classes[i].ClassIndex = i;
                                    Classes[i].gameObject.SetActive(true);
                                    if (Classes[i].ClassImage)
                                    {
                                        if(i == ActiveClass)
                                        {
                                            Classes[i].ClassImage.color = Color.cyan;
                                        }
                                        else
                                        {
                                            Classes[i].ClassImage.color = Color.white;
                                        }
                                    }
                                    Classes[i].UpdateUI();
                                    Classes[i].RequestSerialization();
                                }
                                else
                                {
                                    Classes[i].gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                Classes[i].gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        if (Classes[i])
                        {
                            Classes[i].gameObject.SetActive(false);
                        }
                        if (i < ActiveClass)
                        {
                            ActiveClass = 0;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Classes.Length; i++)
                {
                    Classes[i].gameObject.SetActive(false);
                    Classes[i].UpdateUI();
                    Classes[i].RequestSerialization();
                }
                ActiveClass = 0;
            }
        }
        else
        {
            foreach(var pvpClass in Classes)
            {
                pvpClass.gameObject.SetActive(false);
            }
        }
    }
    #endregion
    #endregion
    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        return IsAllowedToManage(requestingPlayer);
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (MasterTogglePanel)
        {
            if (Networking.LocalPlayer.IsValid())
            {
                MasterTogglePanel.SetActive(IsAllowedToManage(Networking.LocalPlayer));
            }
        }
        FormDatabase();
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(MasterTogglePanel)
        {
            if(Networking.LocalPlayer.IsValid())
            {
                MasterTogglePanel.SetActive(IsAllowedToManage(Networking.LocalPlayer));
            }
        }
        AssignPlayerCapsule(player);
    }
    void AssignPlayerCapsule(VRCPlayerApi player)
    {
        if (!Networking.IsMaster)
            return;
        if (player == null)
            return;
        if (!player.IsValid())
            return;
        if (!Networking.IsNetworkSettled)
            return;
        Debug.Log("WE SHOULD BE ASSIGNING " + player.displayName + " A CAPSULE!");
        if (!PlayerPool)
            return;
        GameObject sapvp_PlayerGO = PlayerPool.TryToSpawn();
        if (sapvp_PlayerGO)
        {
            SAPVP_Player sapvp_Player = sapvp_PlayerGO.GetComponent<SAPVP_Player>();
            Networking.SetOwner(player, sapvp_PlayerGO);
        }

    }
    bool isSetup = false;
    private void LateUpdate()
    {
        if (!Networking.IsNetworkSettled)
            return;
        if(Networking.LocalPlayer == null)
            return;
        if (!Networking.LocalPlayer.IsValid())
            return;
        if(!isSetup)
        {
            AssignPlayerCapsule(Networking.LocalPlayer);
            MasterSyncClasses();
            isSetup = true;
        }
        if (HudTransform)
        {
            VRCPlayerApi.TrackingData trackingData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            HudTransform.transform.SetPositionAndRotation(trackingData.position, trackingData.rotation);
        }
        
    }
}
#else
public class SAPVP_Manager : MonoBehaviour
{
}
#endif