/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * Mk3StrengthFix - Written for KSP v1.00
 * 
 * - Fixes the traction and breaking torque for wheels.
 * 
 * Change Log:
 * - v01.01  (2 May 15)    Reworked loading procedure and recompiled for KSP v1.0.2
 * - v01.00  (27 Apr 15)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Mk3StrengthFix : MonoBehaviour
    {

        public void Start ()
        {
            Debug.Log("Mk3StrengthFix.Start(): v01.01");

            if (null == PartLoader.LoadedPartsList) { return; }

            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                if (null == PartLoader.LoadedPartsList[indexParts].partPrefab) { continue; }

                Part currentPart = PartLoader.LoadedPartsList[indexParts].partPrefab;

                string partName = currentPart.name;

                switch (partName)
                {
                    case "mk3CargoBayL":
                    case "mk3FuselageLF.100":
                    case "mk3FuselageLFO.100":
                        currentPart.breakingForce = 400;
                        currentPart.breakingTorque = 400;
                        Debug.Log("Fixing: " + partName + " | Force: " + currentPart.breakingForce + " | Torque: " + currentPart.breakingTorque);
                        break;

                    case "adapterMk3-Mk2":
                    case "adapterMk3-Size2":
                    case "adapterMk3-Size2Slant":
                    case "adapterSize3-Mk3":
                    case "mk3CargoBayM":
                    case "mk3CrewCabin":
                    case "mk3FuselageLF.50":
                    case "mk3FuselageLFO.50":
                        currentPart.breakingForce = 300;
                        currentPart.breakingTorque = 300;
                        Debug.Log("Fixing: " + partName + " | Force: " + currentPart.breakingForce + " | Torque: " + currentPart.breakingTorque);
                        break;

                    case "mk3Cockpit.Shuttle":
                    case "mk3CargoBayS":
                    case "mk3FuselageLF.25":
                    case "mk3FuselageLFO.25":
                    case "mk3FuselageMONO":
                        currentPart.breakingForce = 200;
                        currentPart.breakingTorque = 200;
                        Debug.Log("Fixing: " + partName + " | Force: " + currentPart.breakingForce + " | Torque: " + currentPart.breakingTorque);
                        break;
                }
            }
        }

    }
}
