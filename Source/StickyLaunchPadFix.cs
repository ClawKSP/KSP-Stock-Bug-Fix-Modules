/*
 * This module written by Claw. For more details please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * StickyLaunchPadFix - Written for KSP v1.0
 * 
 * Change Log:
 * v01.01 (3 May 15) - Recompiled for KSP v1.0.1
 * v01.00 (27 Apr 15) - Recompiled for KSP v1.0
 * v0.1.0a (29 Dec 14) - Reduced log spam
 * v0.1.0 (23 Dec 14) - Initial Release
 */


using UnityEngine;
using KSP;

namespace ClawKSP
{

    public class MSLPFix : PartModule
    {

        //public ModuleEngines EngineModule = null;

        private bool ColliderDisabled = false;

        public override void OnSave(ConfigNode node)
        {
            node.ClearNodes();
            node.ClearData();
        }

        public void FixedUpdate()
        {

            /* Collider - Tier 1 Launch Pad Collider Name
             * LP_main, LP_barsAlpha - Tier 2 Launch Pad Collider Names
             * Launch Pad - Tier 3 Launch Pad Collider Name
             */ 

            // Debug.LogWarning("MSLPFix.FixedUpdate()");



            if (vessel.situation != Vessel.Situations.PRELAUNCH)
            {
                //if (true == ColliderDisabled)
                //{
                //    for (int CollIndex = 0; CollIndex < part.currentCollisions.Count; CollIndex++)
                //    {
                //        Debug.LogWarning("MSLPFix.Update: Cleaning Up Sticky Launch Pad");

                //        if ("LP_main" == part.currentCollisions[CollIndex].name)
                //        {
                //            part.currentCollisions[CollIndex].enabled = true;
                //        }
                //    }
                //}

                part.RemoveModule(this);
                return;
            }

            if (true == ColliderDisabled) { return; }

            bool LP_main_Found = false;
            bool LP_barsAlpha_Found = false;
            bool LP_main_Double = false;

            for (int CollIndex = 0; CollIndex < part.currentCollisions.Count; CollIndex++)
            {
                //Debug.LogWarning("-- " + CollIndex + " || " + part.currentCollisions[CollIndex].name
                //    + " || " + part.currentCollisions[CollIndex].enabled);

                // Check for the main portion (outer ring) of the launch pad
                if ("LP_main" == part.currentCollisions[CollIndex].name)
                {
                    // Occasionally a second LP_main is found and causes problems
                    if (true == LP_main_Found)
                    {
                        LP_main_Double = true;
                    }
                    LP_main_Found = true;
                }
                // Check for mesh grating collision
                else if ("LP_barsAlpha" == part.currentCollisions[CollIndex].name)
                {
                    LP_barsAlpha_Found = true;
                }
            }

            // If two launch pad collision meshes were detected, disable the main.
            if ((true == LP_main_Found) && (true == LP_barsAlpha_Found)
                || (true == LP_main_Double) )
            {
                for (int CollIndex = 0; CollIndex < part.currentCollisions.Count; CollIndex++)
                {
                    //Debug.LogWarning("MSLPFix.FixedUpdate: Sticky Launch Pad Detected");

                    if ("LP_main" == part.currentCollisions[CollIndex].name)
                    {
                        part.currentCollisions[CollIndex].enabled = false;
                        ColliderDisabled = true;
                        break;
                    }
                }
            }

        } // Update

        public void OnDestroy()
        {
            Debug.Log("MSLPFix.OnDestroy()");
        } // OnDestroy
    }

    
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SLPFix : MonoBehaviour
    {

        public void Start()
        {
            Debug.LogWarning("SLPFix.Start(): v01.00");

            GameEvents.onVesselGoOffRails.Add(SLPFixHook);
        } // Start()

        public void SLPFixHook (Vessel VesselToFix)
        {
            if (null == VesselToFix)
            {
                Debug.LogError("SLPFix.SLPFixHook: Passed a null vessel.");
                return;
            }

            if (VesselToFix.situation != Vessel.Situations.PRELAUNCH)
            {
                // Debug.Log("SLPFix.SLPFixHook: Ship Not PRELAUNCH");
                return;
            }

            for (int PartsIndex = 0; PartsIndex < VesselToFix.Parts.Count; PartsIndex++)
            {
                Part CurrentPart = VesselToFix.Parts[PartsIndex];
                if (null == CurrentPart) continue;

                // Debug.LogWarning("Looping Modules = " + CurrentPart.Modules.Count);
                for (int ModuleIndex = 0; ModuleIndex < CurrentPart.Modules.Count; ModuleIndex++)
                {
                    // Debug.LogWarning(CurrentPart.Modules[ModuleIndex].moduleName);
                    if ("ModuleEngines" == CurrentPart.Modules[ModuleIndex].moduleName
                        || "ModuleEnginesFX" == CurrentPart.Modules[ModuleIndex].moduleName)
                    {
                        // Debug.LogWarning("Engine Found " + ModuleIndex + " || Attempting to add to part " + PartsIndex);

                        if (!CurrentPart.Modules.Contains("MSLPFix"))
                        {
                            CurrentPart.AddModule("MSLPFix");
                            break;
                        }
                    }
                }
            }

            // Debug.LogWarning("SLPFix.SLPFixHook: Finished adding modules.");

        } // SLPFixHook
        
        public void OnDestroy()
        {
            Debug.Log("SLPFix.OnDestroy()");

            GameEvents.onVesselGoOffRails.Remove(SLPFixHook);

        } // Destroy()

    } // SLPFix



} // ClawKSP
