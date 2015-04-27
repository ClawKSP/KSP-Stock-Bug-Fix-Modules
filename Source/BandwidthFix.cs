/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * BandwidthFix - Written for KSP v1.00
 * 
 * - Fixes the bandwidth display for transmitters.
 * ModuleDataTransmitter.GetInfo() uses packetSize * packetInterval instead of packetSize / packetInterval
 * 
 * Change Log:
 * - v01.00    Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class BandwidthFix : MonoBehaviour
    {
        public static bool Fixed = false;

        public void Start()
        {
            if (Fixed) { return; }

            Debug.Log("BandwidthFix.Start(): v01.00");

            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                AvailablePart currentAP = PartLoader.LoadedPartsList[indexParts];
                Part currentPart = currentAP.partPrefab;

                //Debug.LogWarning("BWFix.Start(): " + currentPart.name);

                for (int indexModules = 0; indexModules < currentPart.Modules.Count; indexModules++)
                {
                    if ("ModuleDataTransmitter" == currentPart.Modules[indexModules].moduleName)
                    {
                        for (int indexInfo = 0; indexInfo < currentAP.moduleInfos.Count; indexInfo++)
                        {                            
                            if ("Data Transmitter" == currentAP.moduleInfos[indexInfo].moduleName)
                            {
                                Debug.LogWarning("BandwidthFix: Fixing " + currentPart.name);

                                ModuleDataTransmitter TransmitterModule = (ModuleDataTransmitter)currentPart.Modules[indexModules];
                                TransmitterModule.packetInterval = 1 / TransmitterModule.packetInterval;
                                currentAP.moduleInfos[indexInfo].info = TransmitterModule.GetInfo();
                                TransmitterModule.packetInterval = 1 / TransmitterModule.packetInterval;
                            }
                        }
                    }
                }
            }
            Fixed = true;
        }
    }
}
