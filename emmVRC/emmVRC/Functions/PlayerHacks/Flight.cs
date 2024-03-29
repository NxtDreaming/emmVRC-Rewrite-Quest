﻿using emmVRC.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Animation;
using VRC.Core;
using UnityEngine.XR;
using MelonLoader;
using emmVRC.Utils;
using emmVRC.Objects.ModuleBases;

namespace emmVRC.Functions.PlayerHacks
{
    public class Flight : MelonLoaderEvents
    {
        public static bool IsFlyEnabled { get; private set; }
        public static bool IsNoClipEnabled { get; private set; }

        public static ObjectPublicStSiBoSiObBoSiObStSiUnique VerticalInput { get; private set; }
        public static ObjectPublicStSiBoSiObBoSiObStSiUnique HorizontalInput { get; private set; }
        public static ObjectPublicStSiBoSiObBoSiObStSiUnique VerticalLookInput { get; private set; }
        public static ObjectPublicStSiBoSiObBoSiObStSiUnique RunInput { get; private set; }

        public static CharacterController Controller { get; private set; }
        public static VRCMotionState State { get; private set; }

        private static Vector3 gravity;
        private static object coroutine;

        public override void OnApplicationStart()
        {
            NetworkEvents.OnLocalPlayerJoined += (player) => { Controller = player.GetComponent<CharacterController>(); State = player.GetComponent<VRCMotionState>(); };
            NetworkEvents.OnLocalPlayerLeft += (player) => { SetFlyActive(false); SetNoClipActive(false); };
        }

        public override void OnUiManagerInit()
        {
            VerticalInput = VRCInputManager.Method_Public_Static_ObjectPublicStSiBoSiObBoSiObStSiUnique_String_0("Vertical");
            HorizontalInput = VRCInputManager.Method_Public_Static_ObjectPublicStSiBoSiObBoSiObStSiUnique_String_0("Horizontal");
            VerticalLookInput = VRCInputManager.Method_Public_Static_ObjectPublicStSiBoSiObBoSiObStSiUnique_String_0("LookVertical");
            RunInput = VRCInputManager.Method_Public_Static_ObjectPublicStSiBoSiObBoSiObStSiUnique_String_0("Run");
            Configuration.onConfigUpdated.Add(new KeyValuePair<string, Action>("RiskyFunctionsEnabled", () => { 
            if (!Configuration.JSONConfig.RiskyFunctionsEnabled && (IsFlyEnabled || IsNoClipEnabled))
                {
                    if (IsNoClipEnabled)
                        SetNoClipActive(false);
                    SetFlyActive(false);
                }
            }));
        }

        public static void SetFlyActive(bool active)
        {
            if (active)
            {
                if (IsFlyEnabled)
                    return;

                gravity = Physics.gravity;
                Physics.gravity = Vector3.zero;

                if (XRDevice.isPresent && Configuration.JSONConfig.VRFlightControls)
                    coroutine = MelonCoroutines.Start(FlyCoroutineVR());
                else
                    coroutine = MelonCoroutines.Start(FlyCoroutineDesktop());
                IsFlyEnabled = true;
            }
            else
            {
                if (coroutine != null)
                    MelonCoroutines.Stop(coroutine);
                coroutine = null;
                IsFlyEnabled = false;
                IsNoClipEnabled = false;
                Physics.gravity = gravity;
            }
        }

        public static void SetNoClipActive(bool active)
        {
            // Disable the character controller to make it not collide with things
            if (Controller != null)
                Controller.enabled = !active;
            IsNoClipEnabled = active;
        }

        private static IEnumerator FlyCoroutineVR()
        {
            while (true)
            {
                VRCPlayer localPlayer = VRCPlayer.field_Internal_Static_VRCPlayer_0;

                // Get vector containing strafe, run and run speed. scale them to right direction and input amount
                Vector3 movementVector = (Camera.main.transform.forward * localPlayer.field_Private_VRCPlayerApi_0.GetRunSpeed() * VerticalInput.field_Public_Single_0
                    + Vector3.up * localPlayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * VerticalLookInput.field_Public_Single_0
                    + Camera.main.transform.right * localPlayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * HorizontalInput.field_Public_Single_0) * Time.deltaTime;
                // Move the local player transform
                localPlayer.transform.position += movementVector;

                State.Reset();

                yield return null;
            }
        }

        private static IEnumerator FlyCoroutineDesktop()
        {
            while (true)
            {
                VRCPlayer localPlayer = VRCPlayer.field_Internal_Static_VRCPlayer_0;

                float vertModifier = 0;
                vertModifier += Input.GetKey(KeyCode.Q) ? -1 : 0;
                vertModifier += Input.GetKey(KeyCode.E) ? 1 : 0;

                // In desktop the vertical, horizontal and vertical look inputs dont work
                Vector3 movementVector;
                if (RunInput.field_Private_Boolean_0)
                {
                    movementVector = (Camera.main.transform.right * localPlayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * Input.GetAxis("Horizontal")
                        + Vector3.up * localPlayer.field_Private_VRCPlayerApi_0.GetRunSpeed() * vertModifier
                        + Camera.main.transform.forward * localPlayer.field_Private_VRCPlayerApi_0.GetRunSpeed() * Input.GetAxis("Vertical")) * Time.deltaTime;
                }
                else
                {
                    movementVector = (Camera.main.transform.right * localPlayer.field_Private_VRCPlayerApi_0.GetStrafeSpeed() * Input.GetAxis("Horizontal")
                        + Vector3.up * localPlayer.field_Private_VRCPlayerApi_0.GetWalkSpeed() * vertModifier
                        + Camera.main.transform.forward * localPlayer.field_Private_VRCPlayerApi_0.GetWalkSpeed() * Input.GetAxis("Vertical")) * Time.deltaTime;
                }

                localPlayer.transform.position += movementVector;
                localPlayer.prop_VRCPlayerApi_0.SetVelocity(Vector3.zero);
                yield return null;
            }
        }
    }
}