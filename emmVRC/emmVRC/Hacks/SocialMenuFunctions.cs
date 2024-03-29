﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRC;
using VRC.UI;
using System.Collections;
using System.Net.Http;
using emmVRC.Managers;
using emmVRC.Libraries;
using emmVRC.Objects.ModuleBases;

namespace emmVRC.Functions.UI
{
    internal class SocialMenuFunctions : MelonLoaderEvents
    {
        private static GameObject SocialFunctionsButton;
        public static GameObject UserSendMessage;
        private static GameObject UserNotes;
        private static GameObject TeleportButton;
        private static GameObject VRCPlusSupporterButton;
        private static GameObject VRCPlusEarlyAdopterIcon;
        private static GameObject VRCPlusSubscriberIcon;

        private static GameObject PortalToUserButton;


        private static int PortalCooldownTimer = 0;

        public override void OnUiManagerInit()
        {
            VRC.UI.PageUserInfo userInfo = UnityEngine.Resources.FindObjectsOfTypeAll<VRC.UI.PageUserInfo>().FirstOrDefault();
                SocialFunctionsButton = GameObject.Instantiate(userInfo.transform.Find("Buttons/RightSideButtons/RightUpperButtonColumn/FavoriteButton").gameObject, userInfo.transform.Find("Buttons/RightSideButtons/RightUpperButtonColumn"));
            SocialFunctionsButton.GetComponentInChildren<Text>().text = "<color=#FF69B4>emmVRC</color> Functions";
            SocialFunctionsButton.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent();
            //SocialFunctionsButton.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 25f);

            UserSendMessage = GameObject.Instantiate(SocialFunctionsButton, SocialFunctionsButton.transform.parent);
            //UserSendMessage.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 60f);
            UserSendMessage.GetComponentInChildren<Text>().text = "Send Message";
            UserSendMessage.SetActive(false);

            UserNotes = GameObject.Instantiate(UserSendMessage, SocialFunctionsButton.transform.parent);
            //UserNotes.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 60f);
            UserNotes.GetComponentInChildren<Text>().text = "Notes";
            UserNotes.SetActive(false);

            TeleportButton = GameObject.Instantiate(UserNotes, SocialFunctionsButton.transform.parent);
            //TeleportButton.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 60f);
            TeleportButton.GetComponentInChildren<Text>().text = "Teleport";
            TeleportButton.SetActive(false);

            PortalToUserButton = GameObject.Instantiate(TeleportButton, SocialFunctionsButton.transform.parent);
            //PortalToUserButton.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 60f);
            PortalToUserButton.GetComponentInChildren<Text>().text = "Drop Portal";
            PortalToUserButton.SetActive(false);


            SocialFunctionsButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() =>
            {
                //UserSendMessage.SetActive(!UserSendMessage.activeSelf);
                //UserSendMessage.GetComponent<Button>().interactable = false;
                UserNotes.SetActive(!UserNotes.activeSelf);
                //ToggleBlockButton.SetActive(!ToggleBlockButton.activeSelf);
                TeleportButton.SetActive(!TeleportButton.activeSelf);
                try
                {
                    if (userInfo != null && userInfo.field_Private_APIUser_0 != null && userInfo.field_Private_APIUser_0.location != "private" && userInfo.field_Private_APIUser_0.location != "" && !userInfo.field_Private_APIUser_0.statusIsSetToOffline && !userInfo.field_Private_APIUser_0.location.Contains("friends"))
                        PortalToUserButton.SetActive(!PortalToUserButton.activeSelf);
                    else
                        PortalToUserButton.SetActive(false);
                }
                catch 
                { 
                }
                try
                {
                    GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/OnlineFriendButtons").transform.localScale = (GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/OnlineFriendButtons").transform.localScale == Vector3.zero ? Vector3.one : Vector3.zero);
                }
                catch
                {
                }
            }));

            UserSendMessage.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() =>
            {
                VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowInputPopup("Send a message to " + userInfo.field_Private_APIUser_0.GetName() + ":", "", UnityEngine.UI.InputField.InputType.Standard, false, "Send", new System.Action<string, Il2CppSystem.Collections.Generic.List<UnityEngine.KeyCode>, UnityEngine.UI.Text>((string msg, Il2CppSystem.Collections.Generic.List<UnityEngine.KeyCode> keyk, UnityEngine.UI.Text tx) =>
                {
                    //MelonLoader.MelonCoroutines.Start(MessageManager.SendMessage(msg, QuickMenuUtils.GetVRCUiMInstance().menuContent().GetComponentInChildren<PageUserInfo>().field_Private_APIUser_0.id));
                }), null, "Enter message...");
            }));

            UserNotes.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() =>
            {
                Functions.UI.PlayerNotes.LoadNote(userInfo.field_Private_APIUser_0.id, userInfo.field_Private_APIUser_0.GetName());
            }));

            TeleportButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() =>
            {
                Player plrToTP = null;
                Libraries.PlayerUtils.GetEachPlayer((Player plr) =>
                {
                    if (plr.prop_APIUser_0.id == userInfo.field_Private_APIUser_0.id)
                        plrToTP = plr;
                });
                if (plrToTP != null)
                {
                    VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = plrToTP._vrcplayer.transform.position;
                }
                try
                {
                    Functions.UI.CustomAvatarFavorites.baseChooseEvent.Invoke();
                }
                catch
                {
                }
            }));
            PortalToUserButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() =>
            {
                if (userInfo.field_Private_APIUser_0.location != "private" && !userInfo.field_Private_APIUser_0.statusIsSetToOffline && userInfo.field_Private_APIUser_0.location != "" && !userInfo.field_Private_APIUser_0.location.Contains("friends"))
                {
                    try
                    {
                        if (PortalCooldownTimer == 0)
                        {
                            string[] instanceInfo = userInfo.field_Private_APIUser_0.location.Split(':');
                            GameObject portal = VRC.SDKBase.Networking.Instantiate(VRC.SDKBase.VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * 2, VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.rotation);
                            VRC.SDKBase.Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, portal, "ConfigurePortal", new Il2CppSystem.Object[]
                            {
                        (Il2CppSystem.String)instanceInfo[0],
                        (Il2CppSystem.String)instanceInfo[1],
                        new Il2CppSystem.Int32
                        {
                            m_value = 0
                        }.BoxIl2CppObject()
                            });
                            PortalCooldownTimer = 5;
                            MelonLoader.MelonCoroutines.Start(PortalCooldown());
                        }
                        else
                        {
                            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowStandardPopup("emmVRC", "You must wait " + PortalCooldownTimer + " seconds before dropping another portal.", "Dismiss", new System.Action(() => { VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup(); }));
                        }
                    }
                    catch (Exception ex)
                    {
                        emmVRCLoader.Logger.LogError(ex.ToString());
                    }
                }
                else
                {
                    VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.ShowStandardPopup("emmVRC", "You cannot drop a portal to this user.", "Dismiss", new System.Action(() => { VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.HideCurrentPopup(); }));
                }
            }));

            Components.EnableDisableListener listener = SocialFunctionsButton.transform.parent.gameObject.AddComponent<Components.EnableDisableListener>();
            listener.OnEnabled += () =>
            {
                if (Configuration.JSONConfig.DisableVRCPlusUserInfo)
                {
                    VRCPlusSupporterButton.transform.localScale = Vector3.zero;
                    VRCPlusEarlyAdopterIcon.transform.localScale = Vector3.zero;
                    VRCPlusSubscriberIcon.transform.localScale = Vector3.zero;
                }
                else
                {
                    VRCPlusSupporterButton.transform.localScale = Vector3.one;
                    VRCPlusEarlyAdopterIcon.transform.localScale = Vector3.one;
                    VRCPlusSubscriberIcon.transform.localScale = Vector3.one;
                }
            };
            listener.OnDisabled += () =>
             {
                 //UserSendMessage.SetActive(false);
                 GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/OnlineFriendButtons").transform.localScale = Vector3.one;
                 UserNotes.SetActive(false);
                 TeleportButton.SetActive(false);
                 //ToggleBlockButton.SetActive(false);
                 PortalToUserButton.SetActive(false);
             };
        }
        public static IEnumerator PortalCooldown()
        {
            while (PortalCooldownTimer > 0)
            {
                yield return new WaitForSeconds(1f);
                PortalCooldownTimer--;
            }
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == -1)
                MelonLoader.MelonCoroutines.Start(RoomEnter());
        }
        private static IEnumerator RoomEnter()
        {
            while (RoomManager.field_Internal_Static_ApiWorld_0 == null)
                yield return new WaitForEndOfFrame();
            if (VRCPlusSupporterButton == null)
            {
                VRCPlusSupporterButton = GameObject.Find("MenuContent/Screens/UserInfo/Buttons/RightSideButtons/RightUpperButtonColumn/Supporter/SupporterButton");
                VRCPlusEarlyAdopterIcon = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/VRCIcons/VRCPlusEarlyAdopterIcon");
                VRCPlusSubscriberIcon = GameObject.Find("MenuContent/Screens/UserInfo/User Panel/VRCIcons/VRCPlusSubscriberIcon");
            }
        }
    }
}
