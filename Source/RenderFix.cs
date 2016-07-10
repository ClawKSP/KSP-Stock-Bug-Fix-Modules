/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * RenderFix - Written for KSP v1.1.3
 * - Fixes issue with renderers for Fairings, which causes docking port failure and inability to reboard from EVA
 * - Attempts to clean up some leaking objects
 * 
 * Change Log:
 * - v00.01  ( 9 Jul 16) Initial Release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class RFHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleProceduralFairing", "MPFRenderFix");
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class PODestroyer : MonoBehaviour
    {
        public void Start ()
        {
            Debug.Log("PODestroyer.Start(): v00.01");
        }
        public void OnDestroy()
        {
            physicalObject[] pOList = FindObjectsOfType<physicalObject>();

            if (pOList.Length <= 0)
                Debug.Log("PODestroyer.Destroying 0 Orphaned Objects");
            else
                Debug.LogError("PODestroyer.Destroying " + pOList.Length + " Orphaned Objects");

            for (int i = pOList.Length - 1; i >= 0; --i)
            {
                MethodInfo pOInfo = pOList[i].GetType().GetMethod("OnDestory", BindingFlags.NonPublic | BindingFlags.Instance);
                if (null != pOInfo)
                {
                    pOInfo.Invoke(pOList[i], null);
                }
            }
        }
    }

    public class MPFRenderFix : PartModule
    {
        private ModuleProceduralFairing FairingModule;
        private bool isJettisoned = false;

        public void Start()
        {
            Debug.Log("RendererFix.Start(): v01.00");
            isJettisoned = false;
        }

        public void Update()
        {
            FixedUpdate();
        }

        public void FixedUpdate()
        {
            if (isJettisoned == true) { return; }

            FairingModule = (ModuleProceduralFairing)GetModule("ModuleProceduralFairing");
            if (null == FairingModule) { return; }

            if (FairingModule.xSections.Count <= 0)
            {
                Debug.Log("RendererFix: Jettison Detected");
                isJettisoned = true;

                MethodInfo PMethod = part.GetType().GetMethod("CreateRendererLists", BindingFlags.NonPublic | BindingFlags.Instance);
                if (null != PMethod)
                {
                    PMethod.Invoke(part, null);
                }
            }
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
