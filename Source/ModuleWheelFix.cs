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
 * - (Plus) Adds tweakable grip
 * 
 * Change Log:
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
        [KSPField(guiName = "Brake Torque", isPersistant = true, guiActive = true)]
        [UI_FloatRange(minValue = 0f, stepIncrement = 5f, maxValue = 200f)]
        public float brakeTorque = -1f;

        [KSPField(guiName = "Grip Multi", isPersistant = true, guiActive = false)]
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

        private void SetupStockPlus()
        {
            if (StockPlusController.plusActive == false || plusEnabled == false)
            {
                plusEnabled = false;
                return;
            }

            Fields["Stiffness"].guiActive = true;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            WheelModule = (ModuleWheel)GetModule("ModuleWheel");

            if (null == WheelModule)
            {
                Debug.LogWarning("ModuleParachuteFix.Start(): Did not find Wheel Module.");
                return;
            }

            for (int indexWheels = 0; indexWheels < WheelModule.wheels.Count; indexWheels++)
            {
                Debug.Log("WFFix.Start(): " + WheelModule.wheels[indexWheels].whCollider.forwardFriction.stiffness);

                WheelFrictionCurve WFC = WheelModule.wheels[indexWheels].whCollider.forwardFriction;
                WFC.stiffness = originalStiffness * Stiffness;
                WheelModule.wheels[indexWheels].whCollider.forwardFriction = WFC;
            }

            WheelModule.Fields["brakeTorque"].guiActive = false;
            WheelModule.Fields["brakeTorque"].guiActiveEditor = false;

            if (brakeTorque == -1)
            {
                brakeTorque = WheelModule.brakeTorque / 2f;
            }

            SetupStockPlus();
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
