/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleParachutePlus - Written for KSP v1.1.2
 * - Fixes some minor graphics glitches
 * - Plus: Adds a couple visual effects (such as symmetric chute spread and asynchronous chute movement)
 * - Plus: Adds ability to reset chutes that are active but not deployed
 * - Plus: Adds safe/risky/unsafe indicator to staging icons
 * 
 * Change Log:
 * - v01.10  ( 8 May 16) Updated for KSP v1.1.2
 * - v01.09  (21 Apr 16) Updated for KSP v1.1.0
 * - v01.08  (9 Nov 15)  Renamed from ModuleParachuteFix to ModuleParachutePlus. Integrated into new StockBugFixPlusController
 * - v01.07  (3 Aug 15)  Added some more chute flare cases.
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
using System;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MPPlusHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleParachute", "MPPlus");
        }
    }


    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MPPlus : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Semi Time")]
        [UI_FloatRange(minValue = 1f, maxValue = 15f, stepIncrement = 1f)]
        public float semiDeploymentSpeed = 2f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Deploy Time")]
        [UI_FloatRange(minValue = 1f, maxValue = 15f, stepIncrement = 1f)]
        public float deploymentSpeed = 6f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Spread Angle")]
        [UI_FloatRange(minValue = 0f, maxValue = 10f, stepIncrement = 1f)]
        public float spreadAngle = 7f;

        //[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Risky Seconds")]
        //[UI_FloatRange(minValue = 0f, maxValue = 1f, stepIncrement = 0.05f)]
        //public float secondsForRisky = 0.35f;

        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

        //        private bool spreadAllowed = false;

        private int siblingCount = 0;

        //private bool rotateY = false;
        //private bool rotateZ = false;

        private Transform canopy;
        private ModuleParachute ParachuteModule;

        //public override void OnActive()
        //{

        //}

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
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.parachutePlus == false)
            {
                plusEnabled = false;
                Fields["semiDeploymentSpeed"].guiActive = false;
                Fields["semiDeploymentSpeed"].guiActiveEditor = false;
                Fields["deploymentSpeed"].guiActive = false;
                Fields["deploymentSpeed"].guiActiveEditor = false;
                Fields["spreadAngle"].guiActive = false;
                Fields["spreadAngle"].guiActiveEditor = false;
                return;
            }

            plusEnabled = true;
            Debug.Log(moduleName + " StockPlus Enabled");

            //if ((part.attachMode == AttachModes.SRF_ATTACH) && (part.attachMode != AttachModes.STACK))
            //{
            //    spreadAllowed = true;
            //}

            Fields["semiDeploymentSpeed"].guiActive = true;
            Fields["semiDeploymentSpeed"].guiActiveEditor = true;
            Fields["deploymentSpeed"].guiActive = true;
            Fields["deploymentSpeed"].guiActiveEditor = true;
            Fields["spreadAngle"].guiActive = true;
            Fields["spreadAngle"].guiActiveEditor = true;

        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Debug.Log("ModuleParachutePlus.Start(): v01.10");

            ParachuteModule = (ModuleParachute)GetModule("ModuleParachute");

            if (null == ParachuteModule)
            {
                Debug.LogWarning("ModuleParachuteFix.Start(): Did not find Parachute Module.");
                return;
            }

            canopy = part.FindModelTransform(ParachuteModule.canopyName);

            SetupStockPlus();
            GameEvents.onVesselWasModified.Add(CountSiblings);
            CountSiblings(vessel);
        }

        public void OnDestroy()
        {
            GameEvents.onVesselWasModified.Remove(CountSiblings);
        }

        public void FixedUpdate()
        {
            if (null == ParachuteModule) { return; }

            if (plusEnabled)
            {
                ParachuteModule.semiDeploymentSpeed = 1 / semiDeploymentSpeed;
                ParachuteModule.deploymentSpeed = 1 / deploymentSpeed;
            }

            //ParachuteModule.secondsForRisky = secondsForRisky;

            if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED || ParachuteModule.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
            {
                Vector3 chuteDirection = transform.forward;

                float dot = Vector3.Dot((part.Rigidbody.velocity + Krakensbane.GetFrameVelocity()).normalized, transform.forward);

                // Attempts to prevent the transforms from blowing up when chutes are rotated 90 degrees
                if (Mathf.Abs(dot) > 0.99f)
                {
                    chuteDirection = Vector3.RotateTowards(chuteDirection, Vector3.right, 0.01f, 0.01f);
                }

                if (ParachuteModule.invertCanopy)
                {
                    canopy.rotation = Quaternion.LookRotation(-(part.Rigidbody.velocity + Krakensbane.GetFrameVelocity()).normalized, chuteDirection);
                }
                else
                {
                    canopy.rotation = Quaternion.LookRotation((part.Rigidbody.velocity + Krakensbane.GetFrameVelocity()).normalized, chuteDirection);
                }

                if (plusEnabled)
                {
                    #region Symmetric Flare
                    // Applies flare to multiple chute arrangements, 7 degrees per counterpart plus initial offset of 7 degrees

                    if (siblingCount > 2)
                    {
                        float flareAngle = (siblingCount) * spreadAngle * Mathf.Sqrt(1f - Mathf.Abs(dot));
                        if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED)
                        {
                            flareAngle /= 3f;
                        }
                        //else if (ParachuteModule.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                        else if (ParachuteModule.Anim.isPlaying)
                        {
                            flareAngle /= (3f - 2f * ParachuteModule.Anim[ParachuteModule.fullyDeployedAnimation].normalizedTime);
                        }

                        // max flare of 45 degrees
                        if (flareAngle > 45)
                        {
                            flareAngle = 45;
                        }

                        if (flareAngle >= 0.0001f)
                        {

                            float xTwist = Mathf.Cos((part.attRotation.eulerAngles.y * 2 * 3.1415926535f) / 360);
                            float yTwist = Mathf.Sin((part.attRotation.eulerAngles.y * 2 * 3.1415926535f) / 360);

                            Quaternion flare = Quaternion.Euler(xTwist * flareAngle, yTwist * flareAngle, 0);

                            canopy.rotation = canopy.rotation * flare;
                        }
                    }

                    //if (part.symmetryCounterparts != null && part.symmetryCounterparts.Count > 0)
                    //{
                    //    int counterparts = part.symmetryCounterparts.Count + 2;

                    //    // do not deflect more than ~45 degrees
                    //    //if (counterparts > 5)
                    //    //{
                    //    //    counterparts = 5;
                    //    //}

                    //    //Quaternion flare = Quaternion.Euler(counterparts * 10f * (1f - Mathf.Abs(dot)), 0, 0);
                    //    //Quaternion flare = Quaternion.Euler(counterparts * 10f * (1f - Mathf.Abs(dot)), rotateY?10:0, 0);

                    //    float flareAngle = counterparts * 7f * Mathf.Sqrt(1f - Mathf.Abs(dot));

                    //    // max flare of 45 degrees
                    //    if (flareAngle > 45)
                    //    {
                    //        flareAngle = 45;
                    //    }

                    //    if (flareAngle >= 0.0001f)
                    //    {

                    //        float xTwist = Mathf.Cos((part.attRotation.eulerAngles.y * 2 * 3.141592f) / 360);
                    //        float yTwist = Mathf.Sin((part.attRotation.eulerAngles.y * 2 * 3.141592f) / 360);

                    //        Quaternion flare = Quaternion.Euler(xTwist * flareAngle, yTwist * flareAngle, 0);

                    //        canopy.rotation = canopy.rotation * flare;
                    //    }
                    //}
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

        private void CountSiblings(Vessel V)
        {
            if (V != vessel) { return; }

            siblingCount = 0;

            if (part.parent != null)
            {

                // if there are symmetric counterparts, count those
                if (part.symmetryCounterparts != null && part.symmetryCounterparts.Count > 0)
                {
                    siblingCount = part.symmetryCounterparts.Count + 1; // need to also count self
                }
                else // if no symmetric counterparts, see if there are similar parts on this parent
                {
                    string tempName = part.name;

                    for (int IndexParts = 0; IndexParts < part.parent.children.Count; IndexParts++)
                    {
                        if (tempName == part.parent.children[IndexParts].name)
                        {
                            siblingCount++;
                        }
                    }
                }

                siblingCount++; // add one more for initial spread
                //Debug.LogWarning("Count: " + siblingCount);
            }
        }

    }
}
