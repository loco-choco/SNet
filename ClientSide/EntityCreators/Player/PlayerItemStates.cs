﻿using UnityEngine;

using SNet_Client.Utils;
using SNet_Client.EntityScripts.StateSync;
using System;

namespace SNet_Client.EntityCreators.Player
{
    public class PlayerItemStates : MonoBehaviour
    {
        private EntityStatesSync statesSync;

        public GameObject Telescope;

        public GameObject MellowStick;

        public GameObject Flashlight;

        //Suit Stuff
        private PlayerSuit playerSuit;

        public void Start()
        {
            statesSync = GetComponent<EntityStatesSync>();
            playerSuit = GetComponent<PlayerSuit>();

            bool isPlayerAlreadyWithSuit = false;
            if (gameObject.GetAttachedNetworkedEntity().IsOurs())
            {
                GlobalMessenger.AddListener("SuitUp", OnSuitUp);
                GlobalMessenger.AddListener("RemoveSuit", OnRemoveSuit);

                GlobalMessenger.AddListener("TurnOnFlashlight", OnFlashlightOn);
                GlobalMessenger.AddListener("TurnOffFlashlight", OnFlashlightOff);
            }
            else
            {
            }

            statesSync.AddStateListener((byte)PlayerStates.SUIT_EQUIP, isPlayerAlreadyWithSuit, OnSuitStateChange);
            statesSync.AddStateListener((byte)PlayerStates.FLASHLIGHT, false, OnFlashlightStateChange);
            statesSync.AddStateListener((byte)PlayerStates.ROASTING_MELLOWS, false, OnMellowsStateChange);
            statesSync.AddStateListener((byte)PlayerStates.USING_TELESCOPE, false, OnTelescopeStateChange);
        }
        

        public void OnDestroy()
        {
            GlobalMessenger.RemoveListener("SuitUp", OnSuitUp);
            GlobalMessenger.RemoveListener("RemoveSuit", OnRemoveSuit);
            GlobalMessenger.RemoveListener("TurnOnFlashlight", OnFlashlightOn);
            GlobalMessenger.RemoveListener("TurnOffFlashlight", OnFlashlightOff);
        }
        

        private void OnTelescopeStateChange(bool isUsingTelescope)
        {
            //Fazer o telescopio aparecer
        }

        private void OnMellowsStateChange(bool isRoastingMellows)
        {
            //Fazer a animação do graveto aparecer para o outro player
        }

        private void OnFlashlightOff() => statesSync.ChangeValue((byte)PlayerStates.FLASHLIGHT, false);

        private void OnFlashlightOn() => statesSync.ChangeValue((byte)PlayerStates.FLASHLIGHT, true);

        private void OnFlashlightStateChange(bool isLightEnabled)
        {
            //Ligar a lanterna do outro player
        }

        private void OnRemoveSuit() => statesSync.ChangeValue((byte)PlayerStates.SUIT_EQUIP, false);
        private void OnSuitUp() => statesSync.ChangeValue((byte)PlayerStates.SUIT_EQUIP, true);

        private void OnSuitStateChange(bool isWithSuit)
        {
            playerSuit.EquipSuit(isWithSuit);
        }
    }
}