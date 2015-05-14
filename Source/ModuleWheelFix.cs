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
 * - v01.03  (11 May 15)   New UI fix, reworked loading procedures (again again) and fixed a bug
 * - v01.02  (9 May 15)    Reworked loading procedures (again) and updated for StockPlus
 * - v01.01  (1 May 15)    Reworked loading procedure and tweakables, recompiled for KSP 1.0.2
 * - v01.00  (27 Apr 15)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class MWFix : PartModule
    {
        [KSPField(guiName = "Grip Multi", isPersistant = true, guiActive = false, guiActiveEditor = false)]
        [UI_FloatRange(minValue = 1f, maxValue = 3f, stepIncrement = 0.25f)]
        public float Stiffness = 2f;

        private float lastStiffness = 2f;
        public float originalStiffness = 1;

        ModuleWheel WheelModule;

        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

        private PartModule GetModule(string moduleName)
        {
            for (int indexModules = 0; indexModules < part.Modules.Count; indexModules++)
            {
                if (moduleName == part.Modules[indexModules].moduleName)
                {
                    return (part.Modules[indexModules]);
                }
            }

            return (null);

        }  // GetModule

        private float GetMaxTorque()
        {
            Part prefab = PartLoader.getPartInfoByName(part.name).partPrefab;

            ModuleWheel MW = (ModuleWheel) prefab.Modules["ModuleWheel"];

            if (null != MW)
            {
                if (MW.brakeTorque > 30f)
                {
                    return (MW.brakeTorque);
                }
            }
            return (30f);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            Debug.Log(moduleName + ".Start(): v01.03");

            WheelModule = (ModuleWheel)GetModule("ModuleWheel");

            if (null == WheelModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Wheel Module.");
                return;
            }

            // fix the wheel grip
            for (int indexWheels = 0; indexWheels < WheelModule.wheels.Count; indexWheels++)
            {
                WheelFrictionCurve WFC = WheelModule.wheels[indexWheels].whCollider.forwardFriction;
                WFC.stiffness *= Stiffness;
                WheelModule.wheels[indexWheels].whCollider.forwardFriction = WFC;
            }

            //// fix the stock slider by setting the max selectable value to the max value specified in the part.cfg
            UI_FloatRange torqueFloat;

            if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {
                torqueFloat = (UI_FloatRange)WheelModule.Fields["brakeTorque"].uiControlEditor;
            }
            else
            {
                torqueFloat = (UI_FloatRange)WheelModule.Fields["brakeTorque"].uiControlFlight;
            }
            if (torqueFloat != null)
            {
                torqueFloat.maxValue = GetMaxTorque();
                torqueFloat.stepIncrement = torqueFloat.maxValue / 10f;
            }

            SetupStockPlus();
        }

        private void SetupStockPlus()
        {
            if (StockPlusController.plusActive == false || plusEnabled == false)
            {
                plusEnabled = false;
                return;
            }

            Fields["Stiffness"].guiActive = true;
            Fields["Stiffness"].guiActiveEditor = true;
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
        }
    }
}
