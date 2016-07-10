/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * KerbalEVAFix - Written for KSP v1.1.3
 * - Fixes non-responsive EVA lights
 * 
 * Change Log:
 * - v01.00  ( 9 Jul 16)  Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KEVAFixHook : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("KEVAFixHook.Start(): v01.00");
            GameEvents.onCrewOnEva.Add(Hook);
        }

        public void Hook(GameEvents.FromToAction<Part, Part> parts)
        {
            if (parts.to == null) { return; }
            Debug.Log("Hooking KerbalEVAFix");
            parts.to.AddModule("KEVAFix");
        }

        public void OnDestroy()
        {
            GameEvents.onCrewOnEva.Remove(Hook);
        }
    }

    public class KEVAFix : PartModule
    {
        public KerbalEVA KEVA;
        public bool lampOn;

        public void Start()
        {
            Debug.Log("KerbalEVAFix.Start(): v01.00");

            KEVA = (KerbalEVA) GetModule("KerbalEVA");

            if (KEVA != null)
            {
                lampOn = KEVA.lampOn;
            }
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

        public void Update()
        {
            if (KEVA == null)
            {
                KEVA = (KerbalEVA)GetModule("KerbalEVA");
                return;
            }
            if (GameSettings.EVA_Lights.GetKeyDown())
            {
                lampOn = !lampOn;
            }

            if (lampOn != KEVA.lampOn)
            {
                KEVA.lampOn = lampOn;
                KEVA.headLamp.SetActive(lampOn);
            }

        }
    }
}
