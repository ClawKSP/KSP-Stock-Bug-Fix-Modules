/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleWheelFix - Written for KSP v1.0
 * 
 * - Fixes the traction and breaking torque for wheels.
 * 
 * Change Log:
 * - v01.01  (1 May 15)    Reworked loading procedure and tweakables, recompiled for KSP 1.0.2
 * - v01.00  (27 Apr 15)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MWFixHook : MonoBehaviour
    {

        public void Start()
        {
            Debug.Log("MWFixHook.Start(): v01.01");

            if (null == PartLoader.LoadedPartsList) { return; }

            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                if (null == PartLoader.LoadedPartsList[indexParts].partPrefab) { continue; }

                Part currentPart = PartLoader.LoadedPartsList[indexParts].partPrefab;

                if (null == currentPart.Modules) { continue; }

                for (int indexModules = 0; indexModules < currentPart.Modules.Count; indexModules++)
                {
                    if ("ModuleWheel" == currentPart.Modules[indexModules].moduleName)
                    {
                        Debug.Log("MWFixHook: Hooking Wheel " + currentPart.Modules[indexModules].moduleName);
                        ModuleWheel MW = (ModuleWheel)currentPart.Modules[indexModules];
                        MWFix NewModule = (MWFix)currentPart.AddModule("MWFix");

                        NewModule.originalStiffness = MW.wheels[0].whCollider.forwardFriction.stiffness;
                        NewModule.brakeTorque = MW.brakeTorque / 2;
                        continue;
                    }
                }
            }

        } // Start

    }  // MWFixHook



    public class MWFix : PartModule
    {
        [KSPField(guiName = "Brake Torque", isPersistant = true, guiActive = true)]
        [UI_FloatRange(minValue = 0f, stepIncrement = 5f, maxValue = 200f)]
        public float brakeTorque = -1f;

        //[KSPField(guiName = "Grip Multi", isPersistant = true, guiActive = true)]
        //[UI_FloatRange(minValue = 1f, maxValue = 3f, stepIncrement = 0.25f)]
        public float Stiffness = 2f;

        private float lastStiffness = 2f;
        public float originalStiffness = 1;

        ModuleWheel WheelModule;

        private void GetModule()
        {
            if (null == part.Modules) { return; }

            for (int indexModules = 0; indexModules < part.Modules.Count; indexModules++)
            {
                if ("ModuleWheel" == part.Modules[indexModules].moduleName)
                {
                    WheelModule = (ModuleWheel)part.Modules[indexModules];
                }
            }
        }  // GetModule

        public void Start()
        {
            Debug.Log("ModuleWheelFix.Start()");

            GetModule();

            if (null == WheelModule) { return; }

            for (int indexWheels = 0; indexWheels < WheelModule.wheels.Count; indexWheels++)
            {
                Debug.Log("WFFix.Start(): " + WheelModule.wheels[indexWheels].whCollider.forwardFriction.stiffness);

                WheelFrictionCurve WFC = WheelModule.wheels[indexWheels].whCollider.forwardFriction;
                WFC.stiffness = originalStiffness * Stiffness;
                WheelModule.wheels[indexWheels].whCollider.forwardFriction = WFC;
            }

            WheelModule.Fields["brakeTorque"].guiActive = false;
            WheelModule.Fields["brakeTorque"].guiActiveEditor = false;

        }

        public void FixedUpdate()
        {
            if (null == WheelModule) { return; }

            if (lastStiffness != Stiffness)
            {
                lastStiffness = Stiffness;

                for (int indexWheels = 0; indexWheels < WheelModule.wheels.Count; indexWheels++)
                {
                    WheelFrictionCurve WFC = WheelModule.wheels[indexWheels].whCollider.forwardFriction;
                    WFC.stiffness = originalStiffness * Stiffness;
                    WheelModule.wheels[indexWheels].whCollider.forwardFriction = WFC;
                }
            }

            WheelModule.brakeTorque = brakeTorque;
        }
    }
}
