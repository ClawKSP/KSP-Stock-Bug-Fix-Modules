/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleCargoBayFix - Written for KSP v1.00
 * 
 * - Fixes a bug where closing a cargo bay shields any previously held parts.
 * 
 * Change Log:
 * - v01.00    Initial Release
 * 
 */


using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{
    public class MCBFix : PartModule
    {

        private bool ClosedAndChecked = false;
        ModuleCargoBay CargoBayModule;

        public void Start ()
        {
            Debug.Log("ModuleCargoBayFix.Start(): v01.00");

            GameEvents.onVesselGoOffRails.Add(OffRails);
        }

        public void OffRails (Vessel VesselToFix)
        {
            CargoBayModule = (ModuleCargoBay)part.Modules["ModuleCargoBay"];

            if (CargoBayModule != null)
            {
                ClosedAndChecked = false;
            }
        }

        public void FixedUpdate ()
        {
            if (CargoBayModule == null) { return; }

            if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (CargoBayModule.ClosedAndLocked() == true)
                {
                    if (ClosedAndChecked == false)
                    {
                        Debug.LogWarning("MCBFix.FixedUpdate(): Resetting Cargo Bay");

                        CargoBayModule.ClearConnectingParts();
                        ClosedAndChecked = true;

                        MethodInfo CBMethod = CargoBayModule.GetType().GetMethod("EnableShieldedVolume", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (CBMethod != null)
                        {
                            CBMethod.Invoke(CargoBayModule, new object[] { });
                        }
                    }
                }
                else
                {
                    ClosedAndChecked = false;
                }
            }
        }

        public void OnDestroy()
        {
            GameEvents.onVesselGoOffRails.Remove(OffRails);
        }
    }
}
