/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleParachuteFix - Written for KSP v1.00
 * - Fixes drag values displayed in Editor to show values useful for KSP 1.0 aero
 * - Unlocks tweakables to for deployment time and higher altitudes (for drogues)
 * - Adds a couple visual effects (such as symmetric chute spread and asymmetric chute movement)
 * 
 * Change Log:
 * - v01.00  (26 Apr 15) Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class ParachuteInfoFix : MonoBehaviour
    {
        public static bool Fixed = false;

        public void Start()
        {
            if (Fixed) { return; }

            Debug.Log("ParachuteInfoFix.Start(): v01.00");

            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                AvailablePart currentAP = PartLoader.LoadedPartsList[indexParts];
                Part currentPart = currentAP.partPrefab;

                //Debug.LogWarning("BWFix.Start(): " + currentPart.name);

                for (int indexModules = 0; indexModules < currentPart.Modules.Count; indexModules++)
                {
                    if ("ModuleParachute" == currentPart.Modules[indexModules].moduleName)
                    {
                        for (int indexInfo = 0; indexInfo < currentAP.moduleInfos.Count; indexInfo++)
                        {
                            if ("Parachute" == currentAP.moduleInfos[indexInfo].moduleName)
                            {
                                Debug.LogWarning("ParachuteInfoFix: Fixing " + currentPart.name);

                                ModuleParachute ParachuteModule = (ModuleParachute)currentPart.Modules[indexModules];

                                float stowedDrag = ParachuteModule.stowedDrag;
                                float semiDeployedDrag = ParachuteModule.semiDeployedDrag;
                                float fullyDeployedDrag = ParachuteModule.fullyDeployedDrag;

                                // if you want to be _really_ fancy, evaluate Tip.eval(0) * YN_area * YN_drag + (for each XZ P/N) 0.01 *
                                // surf.eval(0) + tail.eval(0) * YP_area * YP_drag, all multiplied by mach.eval(0) then multiply by the two global drag mults, that's that's your real effective area
                                for (int indexCubes = 0; indexCubes < currentPart.DragCubes.Cubes.Count; indexCubes++)
                                {
                                    DragCube DC = currentPart.DragCubes.Cubes[indexCubes];
                                    
                                    //float drag = PhysicsGlobals.DragCurveSurface.Evaluate(0) * DC.Area[0] * DC.Drag[0] +
                                    //             PhysicsGlobals.DragCurveSurface.Evaluate(0) * DC.Area[1] * DC.Drag[1] +
                                    //             PhysicsGlobals.DragCurveTail.Evaluate(0) * DC.Area[2] * DC.Drag[2] +
                                    //             PhysicsGlobals.DragCurveTip.Evaluate(0) * DC.Area[3] * DC.Drag[3] +
                                    //             PhysicsGlobals.DragCurveSurface.Evaluate(0) * DC.Area[4] * DC.Drag[4] +
                                    //             PhysicsGlobals.DragCurveSurface.Evaluate(0) * DC.Area[5] * DC.Drag[5];

                                    //drag = drag * PhysicsGlobals.DragCurveMultiplier.Evaluate(0) * PhysicsGlobals.DragCubeMultiplier * PhysicsGlobals.DragMultiplier;

                                    float drag = DC.Area[0] * DC.Drag[0];
                                    
                                    if (DC.Name == "PACKED")
                                    {
                                        drag = (float)(((int)(drag * 100f)) / 100f);
                                        ParachuteModule.stowedDrag = drag;
                                        //ParachuteModule.stowedDrag = (float)(((int)((DC.Area[3] * DC.Drag[3]) * 100)) / 100);
                                        //ParachuteModule.stowedDrag = (float) ( (int)(DC.Area[3] * 100) / 100f);
                                    }
                                    else if (DC.Name == "SEMIDEPLOYED")
                                    {
                                        drag = (float)(((int)(drag * 10f)) / 10f);
                                        ParachuteModule.semiDeployedDrag = drag;
                                        //ParachuteModule.semiDeployedDrag = (float)(((int)((DC.Area[3] * DC.Drag[3]) * 10)) / 10);
                                        //ParachuteModule.semiDeployedDrag = (float)(  ((int)(DC.Area[3] * 10)) / 10f);
                                    }
                                    else if (DC.Name == "DEPLOYED")
                                    {
                                        drag = (float)((int)drag);
                                        ParachuteModule.fullyDeployedDrag = drag;
                                        //ParachuteModule.fullyDeployedDrag = (float)((int)(DC.Area[3] * DC.Drag[3]));
                                        //ParachuteModule.fullyDeployedDrag = (float)((int)DC.Area[3]);
                                    }
                                }

                                //ParachuteModule.stowedDrag = (float)(((int)((currentPart.DragCubes.Cubes[2].Area[3] * currentPart.DragCubes.Cubes[2].Drag[3]) * 100)) / 100);
                                //ParachuteModule.semiDeployedDrag = (float) ( ((int)((currentPart.DragCubes.Cubes[1].Area[3] * currentPart.DragCubes.Cubes[1].Drag[3])* 10)) / 10);
                                //ParachuteModule.fullyDeployedDrag = (float) ( (int)(currentPart.DragCubes.Cubes[0].Area[3] * currentPart.DragCubes.Cubes[0].Drag[3]) );

                                Debug.LogWarning("Stowed: " + ParachuteModule.stowedDrag);
                                Debug.LogWarning("Semi: " + ParachuteModule.semiDeployedDrag);
                                Debug.LogWarning("Deployed: " + ParachuteModule.fullyDeployedDrag);

                                currentAP.moduleInfos[indexInfo].info = ParachuteModule.GetInfo();

                                ParachuteModule.stowedDrag = stowedDrag;
                                ParachuteModule.semiDeployedDrag = semiDeployedDrag;
                                ParachuteModule.fullyDeployedDrag = fullyDeployedDrag;
                            }
                        }
                    }
                }
            }
            Fixed = true;
        }
    }




    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MPFix : PartModule
    {
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Semi Time")]
        [UI_FloatRange(minValue = 1f, maxValue = 15f, stepIncrement = 1f)]
        public float semiDeploymentSpeed = 2f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Alt (Low)")]
        [UI_FloatRange(minValue = 100f, maxValue = 2500f, stepIncrement = 50f)]
        public float deployAltitude = 500f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Alt (High)")]
        [UI_FloatRange(minValue = 3000f, maxValue = 10000f, stepIncrement = 1000f)]
        public float highAltitude = 5000f;

        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Deploy Time")]
        [UI_FloatRange(minValue = 1f, maxValue = 15f, stepIncrement = 1f)]
        public float deploymentSpeed = 6f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Deploy")]
        [UI_Toggle(enabledText = "High Alt", disabledText = "Low Alt", scene = UI_Scene.All)]
        public bool highAltitudeActive = false;

        private Transform canopy, cap;
        private ModuleParachute ParachuteModule;

        public void Start()
        {
            if (null == part.Modules)
            {
                Debug.LogWarning("MPFix: Null Modules");
                return;
            }
            for (int ModuleIndex = 0; ModuleIndex < part.Modules.Count; ModuleIndex++)
            {
                if ("ModuleParachute" == part.Modules[ModuleIndex].moduleName)
                {
                    ParachuteModule = (ModuleParachute)part.Modules[ModuleIndex];
                    canopy = part.FindModelTransform(ParachuteModule.canopyName);
                    cap = part.FindModelTransform(ParachuteModule.capName);
                    break;
                }
            }

            deploymentSpeed = (int) (1 / ParachuteModule.deploymentSpeed);
            semiDeploymentSpeed = (int) (1 / ParachuteModule.semiDeploymentSpeed);

            ParachuteModule.Fields["deployAltitude"].guiActiveEditor = false;
            ParachuteModule.Fields["deployAltitude"].guiActive = false;
        }

        public void FixedUpdate ()
        {
            if (null == ParachuteModule) { return; }

            ParachuteModule.semiDeploymentSpeed = 1 / semiDeploymentSpeed;
            ParachuteModule.deploymentSpeed = 1 / deploymentSpeed;

            if (highAltitudeActive)
            {
                ParachuteModule.deployAltitude = highAltitude;

                Fields["highAltitude"].guiActive = true;
                Fields["highAltitude"].guiActiveEditor = true;

                Fields["deployAltitude"].guiActive = false;
                Fields["deployAltitude"].guiActiveEditor = false;
            }
            else
            {
                ParachuteModule.deployAltitude = deployAltitude;

                Fields["highAltitude"].guiActive = false;
                Fields["highAltitude"].guiActiveEditor = false;

                Fields["deployAltitude"].guiActive = true;
                Fields["deployAltitude"].guiActiveEditor = true;
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

                #region Symmetric Flare
                // Applies flare to multiple chute arrangements, 10 degrees per counterpart plus initial offset of 10 degrees
                if (part.symmetryCounterparts != null && part.symmetryCounterparts.Count > 0)
                {
                    int counterparts = part.symmetryCounterparts.Count + 2;

                    // do not deflect more than ~45 degrees
                    if (counterparts > 5)
                    {
                        counterparts = 5;
                    }

                    Quaternion flare = Quaternion.Euler(counterparts * 10f * (1f - Mathf.Abs(dot)), 0, 0);

                    canopy.rotation = canopy.rotation * flare;
                }
                #endregion

                // Applies wind drift to the chutes, using the part ID to desync canopy movement
                float timeOffset = Time.time + (float)(part.craftID % 32);
                canopy.Rotate(new Vector3(10f * (Mathf.PerlinNoise(timeOffset, 0f) - 0.5f),
                          10f * (Mathf.PerlinNoise(timeOffset, 8f) - 0.5f),
                          10f * (Mathf.PerlinNoise(timeOffset, 16f) - 0.5f)));

            }

        }

    }
}
