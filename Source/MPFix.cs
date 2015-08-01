/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleParachuteFix - Written for KSP v1.0
 * - Fixes some minor graphics glitches
 * - Fixed log spam and NREs (causing crashes in Linux) for KSP v1.0.4
 * - Plus: Adds a couple visual effects (such as symmetric chute spread and asynchronous chute movement)
 * - Plus: Adds ability to reset chutes that are active but not deployed
 * - Plus: Adds safe/risky/unsafe indicator to staging icons
 * 
 * Change Log:
 * - v01.06  (1 Aug 15)  No code change, but updating version number for re-release (previous .dll was wrong compiled version).
 * - v01.05  (20 Jul 15) Reworked symmetry flare (includes stack chutes), added safe/unsafe indicator
 * - v01.04  (4 Jul 15)  Recompiled and tested for KSP v1.0.4, Fixed log spam and NREs for KSP v1.0.4
 * - v01.02  (18 May 15) Fixed some minor StockPlus integration bugs
 * - v01.01  (8 May 15)  Reworked for KSP v1.0.2 and to include StockPlus code
 * - v01.00  (26 Apr 15) Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MPFix : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Semi Time")]
        [UI_FloatRange(minValue = 1f, maxValue = 15f, stepIncrement = 1f)]
        public float semiDeploymentSpeed = 2f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Deploy Time")]
        [UI_FloatRange(minValue = 1f, maxValue = 15f, stepIncrement = 1f)]
        public float deploymentSpeed = 6f;

        //[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Flash Speed")]
        //[UI_FloatRange(minValue = 1f, maxValue = 30f, stepIncrement = 1f)]
        public float flashSpeed = 15f;

        //[KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Flash Time")]
        //[UI_FloatRange(minValue = 60f, maxValue = 240f, stepIncrement = 10f)]
        public float flashTime = 120f;

        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

//        private bool spreadAllowed = false;
        private bool resetActive = false;

        private string currentDeploySafe;
        private int flashCounter = 0;
        private bool iconOn = true;

        //private bool rotateY = false;
        //private bool rotateZ = false;

        private Transform canopy;
        private ModuleParachute ParachuteModule;

        [KSPEvent(guiName = "Reset Chute", active = false, guiActive = false)]
        public void ResetChute()
        {
            if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.ACTIVE)
            {
                ParachuteModule.deploymentState = ModuleParachute.deploymentStates.STOWED;
                ParachuteModule.persistentState = "STOWED";

                part.deactivate();
                part.stackIcon.SetIconColor(Color.white);

                ParachuteModule.Events["Deploy"].active = true;
                ParachuteModule.Events["CutParachute"].active = false;
                ParachuteModule.Events["Repack"].active = false;
                Events["ResetChute"].active = false;
                Events["ResetChute"].guiActive = false;
            }
        }

        public override void OnActive()
        {
            Events["ResetChute"].active = true;
            Events["ResetChute"].guiActive = true;
        }

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

        private void SetupStockPlus ()
        {
            if (StockPlusController.plusActive == false || plusEnabled == false)
            {
                plusEnabled = false;
                Fields["semiDeploymentSpeed"].guiActive = false;
                Fields["semiDeploymentSpeed"].guiActiveEditor = false;
                Fields["deploymentSpeed"].guiActive = false;
                Fields["deploymentSpeed"].guiActiveEditor = false;
                return;
            }

            if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.ACTIVE)
            {
                Events["ResetChute"].active = true;
                Events["ResetChute"].guiActive = true;
                resetActive = true;
            }
            //if ((part.attachMode == AttachModes.SRF_ATTACH) && (part.attachMode != AttachModes.STACK))
            //{
            //    spreadAllowed = true;
            //}

            currentDeploySafe = ParachuteModule.deploySafe;

            Fields["semiDeploymentSpeed"].guiActive = true;
            Fields["semiDeploymentSpeed"].guiActiveEditor = true;
            Fields["deploymentSpeed"].guiActive = true;
            Fields["deploymentSpeed"].guiActiveEditor = true;

        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Debug.Log("ModuleParachuteFix.Start(): v01.06");

            ParachuteModule = (ModuleParachute)GetModule("ModuleParachute");

            if (null == ParachuteModule)
            {
                Debug.LogWarning("ModuleParachuteFix.Start(): Did not find Parachute Module.");
                return;
            }

            // This part fixes log spam in KSP v1.0.4
            if (vessel == null)
            {
                ParachuteModule.part.packed = true;
                return;
            }

            canopy = part.FindModelTransform(ParachuteModule.canopyName);

            SetupStockPlus();
        }

        public void FixedUpdate ()
        {
            if (null == ParachuteModule) { return; }

            // This part fixes log spam in KSP v1.0.4
            if (vessel == null)
            {
                ParachuteModule.part.packed = true;
                return;
            }

            if (plusEnabled)
            {
                if (resetActive == true)
                {
                    if (ParachuteModule.deploymentState != ModuleParachute.deploymentStates.ACTIVE)
                    {
                        Events["ResetChute"].active = false;
                        Events["ResetChute"].guiActive = false;
                        resetActive = false;
                    }
                }
                else
                {
                    if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.ACTIVE)
                    {
                        Events["ResetChute"].active = true;
                        Events["ResetChute"].guiActive = true;
                        resetActive = true;
                    }
                }
                ParachuteModule.semiDeploymentSpeed = 1 / semiDeploymentSpeed;
                ParachuteModule.deploymentSpeed = 1 / deploymentSpeed;


                #region Safe Deploy Indicator
                if (currentDeploySafe != ParachuteModule.deploySafe)
                {
                    currentDeploySafe = ParachuteModule.deploySafe;
                    flashCounter = 0;
                    iconOn = true;
                }

                if (flashCounter < flashTime)
                {
                    flashCounter++;
                    if (flashCounter % flashSpeed == 0)
                    {
                        iconOn = !iconOn;
                    }
                }
                else
                {
                    iconOn = true;
                }

                if (iconOn == true)
                {
                    if (ParachuteModule.deploySafe == "Safe")
                    {
                        part.stackIcon.SetBgColor(XKCDColors.RadioactiveGreen);
                    }
                    else if (ParachuteModule.deploySafe == "Risky")
                    {
                        part.stackIcon.SetBgColor(XKCDColors.BrightYellow);
                    }
                    else if (ParachuteModule.deploySafe == "Unsafe")
                    {
                        part.stackIcon.SetBgColor(XKCDColors.BrightRed);
                    }
                    else
                    {
                        part.stackIcon.SetBgColor(Color.white);
                    }
                }
                else
                {
                    part.stackIcon.SetBgColor(Color.white);
                } 
                #endregion
            }

            if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED || ParachuteModule.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
            {
                Vector3 chuteDirection = transform.forward;

                float dot = Vector3.Dot((rigidbody.velocity + Krakensbane.GetFrameVelocity()).normalized, transform.forward);

                // Attempts to prevent the transforms from blowing up when chutes are rotated 90 degrees
                if (Mathf.Abs(dot) > 0.99f)
                {
                    chuteDirection = Vector3.RotateTowards(chuteDirection, Vector3.right, 0.01f, 0.01f);
                }

                if (ParachuteModule.invertCanopy)
                {
                    canopy.rotation = Quaternion.LookRotation(-(rigidbody.velocity + Krakensbane.GetFrameVelocity()).normalized, chuteDirection);
                }
                else
                {
                    canopy.rotation = Quaternion.LookRotation((rigidbody.velocity + Krakensbane.GetFrameVelocity()).normalized, chuteDirection);
                }

                if (plusEnabled)
                {
                    #region Symmetric Flare
                    // Applies flare to multiple chute arrangements, 7 degrees per counterpart plus initial offset of 7 degrees

                    if (part.symmetryCounterparts != null && part.symmetryCounterparts.Count > 0)
                    {
                        int counterparts = part.symmetryCounterparts.Count + 2;

                        // do not deflect more than ~45 degrees
                        //if (counterparts > 5)
                        //{
                        //    counterparts = 5;
                        //}

                        //Quaternion flare = Quaternion.Euler(counterparts * 10f * (1f - Mathf.Abs(dot)), 0, 0);
                        //Quaternion flare = Quaternion.Euler(counterparts * 10f * (1f - Mathf.Abs(dot)), rotateY?10:0, 0);

                        float flareAngle = counterparts * 7f * Mathf.Sqrt(1f - Mathf.Abs(dot));
                        
                        // max flare of 45 degrees
                        if (flareAngle > 45)
                        {
                            flareAngle = 45;
                        }

                        if (flareAngle >= 0.0001f)
                        {

                            float xTwist = Mathf.Cos((part.attRotation.eulerAngles.y * 2 * 3.141592f) / 360);
                            float yTwist = Mathf.Sin((part.attRotation.eulerAngles.y * 2 * 3.141592f) / 360);

                            Quaternion flare = Quaternion.Euler(xTwist * flareAngle, yTwist * flareAngle, 0);

                            canopy.rotation = canopy.rotation * flare;
                        }
                    }
                    #endregion


                    #region Drag Cube Drift
                    // Applies wind drift to the chutes, using the part ID to desync canopy movement
                    float timeOffset = Time.time + (float)(part.craftID % 32);
                    canopy.Rotate(new Vector3(10f * (Mathf.PerlinNoise(timeOffset, 0f) - 0.5f),
                              10f * (Mathf.PerlinNoise(timeOffset, 8f) - 0.5f),
                              10f * (Mathf.PerlinNoise(timeOffset, 16f) - 0.5f)));

                    Quaternion dragVectorRotation = Quaternion.LookRotation(part.partTransform.InverseTransformDirection(canopy.forward));
                    part.DragCubes.SetDragVectorRotation(dragVectorRotation); 
                    #endregion
                }

                #region DebugKeys
                //if (Input.GetKeyDown(KeyCode.L))
                //{
                //    Debug.LogWarning("X: " + part.attRotation.eulerAngles.x + " | Y: " + part.attRotation.eulerAngles.y + " | Z: " + part.attRotation.eulerAngles.z);
                //}
                //if (Input.GetKeyDown(KeyCode.B))
                //{
                //    rotateY = !rotateY;
                //    Debug.LogWarning("RotateY: " + rotateY);
                //}
                //if (Input.GetKeyDown(KeyCode.N))
                //{
                //    rotateZ = !rotateZ;
                //    Debug.LogWarning("RotateZ: " + rotateZ);
                //} 
                #endregion

            }

        }

    }
}
