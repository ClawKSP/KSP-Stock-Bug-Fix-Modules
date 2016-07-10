/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * DockingPortFix - Written for KSP v1.1.3
 * - Fixes docking ports frozen by previous bugs
 * 
 * Change Log:
 * - v01.00  (10 Jul 16)  Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DPFixHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleDockingNode", "DockingPortFix");
        }
    }

    public class DockingPortFix : PartModule
    {
        ModuleDockingNode MDN;

        public void Start()
        {
            Debug.Log("DockingPortFix.Start(): v01.00");
            Events["ForceDecouple"].active = false;
            Events["ForceDecouple"].guiActive = false;
            Events["ForceDecouple"].guiActiveEditor = false;
        }

        public void Update()
        {
            // Check if the parent is another docking port
            if (part.parent != null && part.parent.Modules != null && part.parent.Modules.Contains("ModuleDockingNode"))
            {
                MDN = (ModuleDockingNode)GetModule("ModuleDockingNode");

                if (MDN != null)
                {
                    // Check if the referenced docking part is pointing at the parent
                    if (MDN.referenceNode.attachedPart != part.parent && !MDN.Events["Undock"].active && !MDN.Events["UndockSameVessel"].active)
                    {
                        Events["ForceDecouple"].active = true;
                        Events["ForceDecouple"].guiActive = true;
                    }
                    else
                    {
                        Events["ForceDecouple"].active = false;
                        Events["ForceDecouple"].guiActive = false;
                    }
                }
            }
        }
        
        [KSPEvent(guiName = "Force Undock", guiActive = true, guiActiveEditor = false)]
        public void ForceDecouple()
        {
            MDN = (ModuleDockingNode)GetModule("ModuleDockingNode");

            if (MDN == null) { return; }

            MDN.referenceNode.attachedPart = part.parent;
            MDN.Decouple();

            Events["ForceDecouple"].active = false;
            Events["ForceDecouple"].guiActive = false;
        }

        private PartModule GetModule(string moduleName)
        {
            for (int indexModules = part.Modules.Count - 1; indexModules >= 0; --indexModules)
            {
                if (moduleName == part.Modules[indexModules].moduleName)
                {
                    return (part.Modules[indexModules]);
                }
            }

            return (null);

        }  // GetModule
    }
}
