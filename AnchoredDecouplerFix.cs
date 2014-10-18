using System.Linq;
using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class ModuleAnchoredDecouplerFix : PartModule
    {

        public int DecouplerModuleIndex;

        public void Start()
        {
            Debug.Log("ModuleAnchoredDecouplerFix: Start() against Module #" + DecouplerModuleIndex);
        }

        public override void OnFixedUpdate()
        {

            ModuleAnchoredDecoupler DecouplerModule = (ModuleAnchoredDecoupler)part.Modules.GetModule(DecouplerModuleIndex);
            if (false == DecouplerModule.isDecoupled)
            {
                return;
            }

            Vector3d PartVelocity = part.rigidbody.velocity;
            Debug.LogWarning("ModuleAnchoredDecouplerFix: Velocity X, Y, Z, Magnitude = " + PartVelocity.x + " " + PartVelocity.y + " " + PartVelocity.z + " " + PartVelocity.magnitude);

            part.rigidbody.AddRelativeForce(Vector3d.left * DecouplerModule.ejectionForce, ForceMode.Force);
            part.RemoveModule(this);
        }

        public void OnDestroy()
        {
            Debug.LogWarning("ModuleAnchoredDecouplerFix: Destroyed.");
        }

    }  // ModuleAnchoredDecouplerFix


    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class AnchoredDecouplerFix : UnityEngine.MonoBehaviour
    {
        public void Awake()
        {
            Debug.LogWarning("AnchoredDecouplerFix: Awake");
        }

        public void Start()
        {
            Debug.LogWarning("AnchoredDecouplerFix: Start");

            GameEvents.onVesselLoaded.Add(AnchoredDecouplerFixHook);
            GameEvents.onVesselGoOffRails.Add(AnchoredDecouplerFixHook);

            Debug.LogWarning("AnchoreDecouplerFix: Start complete.");
        }

        public void AnchoredDecouplerFixHook(Vessel VesselToFix)
        {
            if (null == VesselToFix)
            {
                Debug.LogError("AnchoredDecouplerFixHook: Was passed a null vessel.");
                return;
            }

            Debug.LogWarning("AnchoredDecouplerFixHook: Attempting to add module fix. (" + VesselToFix.Parts.Count + " parts.)");

            for (int PartsIndex = 0; PartsIndex < VesselToFix.Parts.Count; PartsIndex++)
            {
                Part CurrentPart = VesselToFix.Parts[PartsIndex];
                if (null == CurrentPart) continue;

                Debug.LogWarning("Looping Modules = " + CurrentPart.Modules.Count);
                for (int ModuleIndex = 0; ModuleIndex < CurrentPart.Modules.Count; ModuleIndex++)
                {
                    ModuleAnchoredDecoupler DecouplerModule;
                    Debug.LogWarning(CurrentPart.Modules[ModuleIndex].moduleName);
                    if ("ModuleAnchoredDecoupler" == CurrentPart.Modules[ModuleIndex].moduleName)
                    {
                        Debug.LogWarning("Decoupler Found " + ModuleIndex);
                        DecouplerModule = (ModuleAnchoredDecoupler)CurrentPart.Modules.GetModule(ModuleIndex);

                        if (false == DecouplerModule.isDecoupled)
                        {
                            Debug.LogWarning("AnchoredDecouplerFixHook: Attempting to add a fix module to part " + PartsIndex);
                            ModuleAnchoredDecouplerFix NewModule = (ModuleAnchoredDecouplerFix) CurrentPart.AddModule("ModuleAnchoredDecouplerFix");
                            NewModule.DecouplerModuleIndex = ModuleIndex;
                            Debug.LogWarning("AnchoredDecouplerFixHook: Added Fix module.");
                        }
                        break;
                    }
                }
            }

            Debug.LogWarning("AnchoreDecouplerFixHook: Finished adding modules.");
        }  // AnchoredDecouplerFixHook

        public void OnDestroy ()
        {
            Debug.LogWarning("AnchoredDecouplerFix: OnDestroy");

            GameEvents.onVesselLoaded.Remove(AnchoredDecouplerFixHook);
            GameEvents.onVesselGoOffRails.Remove(AnchoredDecouplerFixHook);
        }

    }  // AnchoredDecouplerFix

} // ClawKSP
