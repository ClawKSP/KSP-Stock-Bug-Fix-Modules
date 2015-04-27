/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleWheelFix - Written for KSP v1.00
 * 
 * - Fixes the traction and breaking torque for wheels.
 * 
 * Change Log:
 * - v01.00    Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class MWFix : PartModule
    {
        public float steeringResponseSpeed = 0.1f;
        [KSPField(guiName = "Brake Torque", isPersistant = true, guiActive = true)]
        [UI_FloatRange(minValue = 0f, stepIncrement = 10f, maxValue = 200f)]
        public float brakeTorque = -1f;

        [KSPField(guiName = "Grip Multi", isPersistant = true, guiActive = true)]
        [UI_FloatRange(minValue = 1f, maxValue = 3f, stepIncrement = 0.25f)]
        public float Stiffness = 2f;

        private float lastStiffness = 2f;
        private float originalStiffness;

        ModuleWheel WheelModule;

        public void Start ()
        {
            WheelModule = (ModuleWheel) part.Modules["ModuleWheel"];

            if (WheelModule == null) { return; }

            AvailablePart originalPart = PartLoader.getPartInfoByName(part.name);
            
            if (originalPart != null)
            {
                for (int indexModules = 0; indexModules < originalPart.partPrefab.Modules.Count; indexModules++)
                {
                    if ("ModuleWheel" == originalPart.partPrefab.Modules[indexModules].moduleName)
                    {
                        ModuleWheel MW = (ModuleWheel) originalPart.partPrefab.Modules[indexModules];
                        originalStiffness = MW.wheels[0].whCollider.forwardFriction.stiffness;
                        if (brakeTorque == -1f)
                        {
                            brakeTorque = MW.brakeTorque / 2;
                        }
                    }
                }
            }

            for (int indexWheels = 0; indexWheels < WheelModule.wheels.Count; indexWheels++)
            {
                Debug.LogWarning("WFFix.Start(): " + WheelModule.wheels[indexWheels].whCollider.forwardFriction.stiffness);

                WheelFrictionCurve WFC = WheelModule.wheels[indexWheels].whCollider.forwardFriction;
                WFC.stiffness = originalStiffness * Stiffness;
                WheelModule.wheels[indexWheels].whCollider.forwardFriction = WFC;
            }

            WheelModule.Fields["brakeTorque"].guiActive = false;
            WheelModule.Fields["brakeTorque"].guiActiveEditor = false;

        }

        public void FixedUpdate ()
        {
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
