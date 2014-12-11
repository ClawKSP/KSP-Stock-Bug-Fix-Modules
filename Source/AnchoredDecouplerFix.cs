/*
 * This module written by Claw (with an incredible amount of help from NathanKell). Please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.25.0
 *
 * AnchoredDecouplerFix v0.1.2a
 * 
 * Change Log:
 * 
 * v0.1.2a - Updated for compatibility with Kerbquake
 * v0.1.2 - Fixed decouplers breaking off at 700-750 m/s and strut disconnect problems.
 * v0.1.1 - Scaled force based on airspeed
 * v0.1 - Initial Release
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class MADFix : PartModule
    {
        public int DecouplerModuleIndex;
        private int StateTimer = 0;
        private float EjectionForce = 250;

        public void Start()
        {
            Debug.Log("MADFix: Start() against Module #" + DecouplerModuleIndex);

            ModuleAnchoredDecoupler DecouplerModule = (ModuleAnchoredDecoupler)part.Modules.GetModule(DecouplerModuleIndex);
            EjectionForce = DecouplerModule.ejectionForce;
            DecouplerModule.ejectionForce = 0;
        }
        
        public override void OnUpdate()
        {
            ModuleAnchoredDecoupler DecouplerModule = (ModuleAnchoredDecoupler)part.Modules.GetModule(DecouplerModuleIndex);
            if (false == DecouplerModule.isDecoupled)
            {
                return;
            }
            part.vessel.angularMomentum.Zero();
            part.vessel.angularVelocity.Zero();
            part.rigidbody.angularVelocity.Zero();
        }

        public override void OnFixedUpdate()
        {
            ModuleAnchoredDecoupler DecouplerModule = (ModuleAnchoredDecoupler)part.Modules.GetModule(DecouplerModuleIndex);
            if (false == DecouplerModule.isDecoupled)
            {
                return;
            }

            // Debug.Log("MADFix: OnFixedUpdate, State Timer = " + StateTimer);

            if (0 == StateTimer)
            {
                part.vessel.angularMomentum.Zero();
                part.vessel.angularVelocity.Zero();
                part.rigidbody.angularVelocity.Zero();
            }
            else if (1 == StateTimer) // Waits for struts and fuel lines to all disconnect
            {
                part.rigidbody.AddRelativeForce(Vector3d.left * EjectionForce, ForceMode.Force);
                Debug.Log("ModuleAnchordDecouplerFix: Fix Applied. Force = " + EjectionForce);
            }
            else if (2 == StateTimer) // Waits for the force to disappate before deleting
            {
                part.RemoveModule(this);
            }
            StateTimer++;
        }

        public void OnDestroy()
        {
            // Debug.LogWarning("MADFix: Destroyed.");

            ModuleAnchoredDecoupler DecouplerModule = (ModuleAnchoredDecoupler)part.Modules.GetModule(DecouplerModuleIndex);
            DecouplerModule.ejectionForce = EjectionForce;
        }

    }  // MADFix


    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AnchoredDecouplerFix : UnityEngine.MonoBehaviour
    {
        public void Awake()
        {
            // Debug.LogWarning("AnchoredDecouplerFix: Awake");
        }

        public void Start()
        {
            // Debug.LogWarning("AnchoredDecouplerFix: Start");

            GameEvents.onVesselLoaded.Add(AnchoredDecouplerFixHook);
            GameEvents.onVesselGoOffRails.Add(AnchoredDecouplerFixHook);

            // Debug.LogWarning("AnchoreDecouplerFix: Start complete.");
        }

        public void AnchoredDecouplerFixHook(Vessel VesselToFix)
        {
            if (null == VesselToFix)
            {
                Debug.LogError("AnchoredDecouplerFixHook: Was passed a null vessel.");
                return;
            }

            // Debug.LogWarning("AnchoredDecouplerFixHook: Attempting to add module fix. (" + VesselToFix.Parts.Count + " parts.)");

            for (int PartsIndex = 0; PartsIndex < VesselToFix.Parts.Count; PartsIndex++)
            {
                Part CurrentPart = VesselToFix.Parts[PartsIndex];
                if (null == CurrentPart) continue;

                // Debug.LogWarning("Looping Modules = " + CurrentPart.Modules.Count);
                for (int ModuleIndex = 0; ModuleIndex < CurrentPart.Modules.Count; ModuleIndex++)
                {
                    ModuleAnchoredDecoupler DecouplerModule;
                    // Debug.LogWarning(CurrentPart.Modules[ModuleIndex].moduleName);
                    if ("ModuleAnchoredDecoupler" == CurrentPart.Modules[ModuleIndex].moduleName)
                    {
                        // Debug.LogWarning("Decoupler Found " + ModuleIndex);
                        DecouplerModule = (ModuleAnchoredDecoupler)CurrentPart.Modules.GetModule(ModuleIndex);

                        if (false == DecouplerModule.isDecoupled)
                        {
                            // Debug.LogWarning("AnchoredDecouplerFixHook: Attempting to add a fix module to part " + PartsIndex);
                            MADFix NewModule = (MADFix) CurrentPart.AddModule("MADFix");
                            NewModule.DecouplerModuleIndex = ModuleIndex;
                            // Debug.LogWarning("AnchoredDecouplerFixHook: Added Fix module.");
                        }
                        break;
                    }
                }
            }

            // Debug.LogWarning("AnchoreDecouplerFixHook: Finished adding modules.");
        }  // AnchoredDecouplerFixHook

        public void OnDestroy ()
        {
            // Debug.LogWarning("AnchoredDecouplerFix: OnDestroy");

            GameEvents.onVesselLoaded.Remove(AnchoredDecouplerFixHook);
            GameEvents.onVesselGoOffRails.Remove(AnchoredDecouplerFixHook);
        }

    }  // AnchoredDecouplerFix

} // ClawKSP
