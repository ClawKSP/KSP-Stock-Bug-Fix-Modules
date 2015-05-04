/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleProceduralFairingFix - Written for KSP v1.0
 * 
 * - Fixes some bugs with pulling and replacing fairings.
 * - Activates a tweakable slider for the user to select the number of panels on the fairing.
 * - Activates a tweakable slider to control ejection forces on the panels.
 * 
 * Change Log:
 * - v01.02  (3 May 15)   Moved ejection force out to Module Manager
 * - v01.01  (2 May 15)   Updated and recompiled for KSP 1.0.2
 * - v01.00  (27 Apr 15)  Initial Release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MPFFixHook : MonoBehaviour
    {

        public void Start()
        {
            Debug.Log("ModuleProceduralFairingFixHook.Start(): v01.01");

            if (null == PartLoader.LoadedPartsList) { return; }

            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                if (null == PartLoader.LoadedPartsList[indexParts].partPrefab) { continue; }

                Part currentPart = PartLoader.LoadedPartsList[indexParts].partPrefab;

                if (null == currentPart.Modules) { continue; }

                for (int indexModules = 0; indexModules < currentPart.Modules.Count; indexModules++)
                {
                    if ("ModuleProceduralFairing" == currentPart.Modules[indexModules].moduleName)
                    {
                        currentPart.AddModule("MPFFix");

                        ModuleProceduralFairing MPF = (ModuleProceduralFairing) currentPart.Modules[indexModules];

                        //switch (currentPart.name)
                        //{
                        //    case "fairingSize1":
                        //        MPF.ejectionForce = 30;
                        //        Debug.Log("MPFFix: Fixing " + currentPart.name + " | Force: " + MPF.ejectionForce);
                        //        break;
                        //    case "fairingSize2":
                        //        MPF.ejectionForce = 100;
                        //        Debug.Log("MPFFix: Fixing " + currentPart.name + " | Force: " + MPF.ejectionForce);
                        //        break;
                        //    case "fairingSize3":
                        //        MPF.ejectionForce = 150;
                        //        Debug.Log("MPFFix: Fixing " + currentPart.name + " | Force: " + MPF.ejectionForce);
                        //        break;
                        //}

                        continue;
                    }
                }
            }

        } // Start

    }  // MWFixHook



    public class MPFFix : PartModule
    {

        ModuleProceduralFairing FairingModule;

        private void GetModule()
        {
            if (null == part.Modules) { return; }

            for (int indexModules = 0; indexModules < part.Modules.Count; indexModules++)
            {
                if ("ModuleProceduralFairing" == part.Modules[indexModules].moduleName)
                {
                    FairingModule = (ModuleProceduralFairing)part.Modules[indexModules];
                }
            }
        }  // GetModule

        public void Start()
        {
            Debug.Log("ModuleProceduralFairingFix.OnStart(): v01.02");

            GetModule();

            if (null == FairingModule) { return; }

            GameEvents.onPartRemove.Add(RemovePart);
        }

        public void RemovePart(GameEvents.HostTargetAction<Part, Part> RemovedPart)
        {
            if (null == FairingModule) { return; }

            if (RemovedPart.target == part)
            {
                if (FairingModule.xSections.Count > 0)
                {
                    Debug.LogWarning("Deleting");
                    FairingModule.DeleteFairing();
                }
                MethodInfo MPFMethod = FairingModule.GetType().GetMethod("DumpInterstage", BindingFlags.NonPublic | BindingFlags.Instance);

                if (MPFMethod != null)
                {
                    MPFMethod.Invoke(FairingModule, new object[] { });
                }
            }
        }

        public void FixedUpdate()
        {
            // Do nothing for now
        }

        public void OnDestroy()
        {
            GameEvents.onPartRemove.Remove(RemovePart);
        }
    }
}
