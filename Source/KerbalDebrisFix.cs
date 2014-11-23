/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.25.0
 *
 * KerbalDebrisFix v0.1
 * 
 * Change Log:
 * 
 * v0.1a - Fixed names when loading an already frozen kerbal.
 * v0.1 - Initial Release
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KerbalDebrisFix : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("KerbalDebrisFix.Start()");

            GameEvents.onVesselGoOffRails.Add(OffRails);
            GameEvents.onCrewOnEva.Add(OnEVA);
        }

        public void OnEVA (GameEvents.FromToAction<Part,Part> EVAParts)
        {
            Debug.Log("KerbalDebrisFix: Adding Fix Module onEVA");
            EVAParts.to.AddModule("ModuleKerbalDebrisFix");
        }

        public void OffRails (Vessel VesselToFix)
        {
            // Debug.LogWarning("KerbalDebrisFix: OffRails");

            for (int IndexParts = 0; IndexParts < VesselToFix.parts.Count; IndexParts++)
            {
                // Debug.LogWarning(VesselToFix.parts[IndexParts].name.Substring(0, 9));
                if ("kerbalEVA" == VesselToFix.parts[IndexParts].name.Substring(0, 9))
                {
                    // Debug.LogWarning("KerbalDebrisFix: Kerbal Found");
                    if (!VesselToFix.parts[IndexParts].Modules.Contains("ModuleKerbalDebrisFix"))
                    {
                        Debug.Log("KerbalDebrisFix: Adding Fix Module OffRails");
                        VesselToFix.parts[IndexParts].AddModule("ModuleKerbalDebrisFix");
                    }
                }
            }
        }

        public void OnDestroy()
        {
            // Debug.LogWarning("KerbalDebrisFix.OnDestroy()");

            GameEvents.onVesselGoOffRails.Remove(OffRails);
            GameEvents.onCrewOnEva.Remove(OnEVA);
        }
    }



    public class ModuleKerbalDebrisFix : PartModule
    {
        public void Update()
        {
            //Debug.LogWarning("ModuleKerbalDebrisFix.Update()");
            OnUpdate();
        }

        public override void OnUpdate()
        {
            //Debug.LogWarning("ModuleKerbalDebrisFix.OnUpdate()");

            if (null == part.parent)
            {
                if (VesselType.Debris == vessel.vesselType)
                {
                    Debug.LogWarning("ModuleKerbalDebrisFix: Fixing Kerbal");
                    vessel.vesselType = VesselType.EVA;
                    vessel.vesselName = part.protoModuleCrew[0].name;
                }
            }
            //else
            //{
            //    Debug.LogWarning("Parent is something else");
            //} 
        }
    }
}
