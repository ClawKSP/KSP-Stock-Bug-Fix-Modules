/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * HighlighterOff - Written for KSP v1.0.5
 * 
 * - Disables highlighting for parts in flight mode.
 * 
 * Change Log:
 * - v01.01    Recompiled for v1.0.5. Integrated into StockPlus. Renamed from HighlighterOff to InFlightHighlightOff
 * - v01.00    Initial Release
 * 
 */

using UnityEngine;
using KSP;


namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class InFlightHighlightOff : MonoBehaviour
    {

        //private bool isActive = false;

        public void Start()
        {
            Debug.LogWarning("InFlightHighlightOff.Start(): version 1.01");
            GameEvents.onVesselGoOffRails.Add(OffRails);
        }

        //public void FixedUpdate()
        //{
        //    if (StockBugFixPlusController.inFlightHighlightOff != isActive)
        //    {
        //        isActive = StockBugFixPlusController.inFlightHighlightOff;
        //        for (int indexVessels = 0; indexVessels < FlightGlobals.Vessels.Count; indexVessels++)
        //        {
        //            OffRails(FlightGlobals.Vessels[indexVessels]);
        //        }
        //    }
        //}

        public void OffRails(Vessel vesselToFix)
        {
            if (StockBugFixPlusController.plusActive && StockBugFixPlusController.inFlightHighlightOff)
            {
                Debug.Log("InFlightHighlightOff.OffRails(): Disabling Highlight");
                for (int indexParts = 0; indexParts < vesselToFix.Parts.Count; indexParts++)
                {
                    vesselToFix.Parts[indexParts].highlightType = Part.HighlightType.Disabled;
                    vesselToFix.Parts[indexParts].SetHighlight(false, false);
                }
            }
            else
            {
                Debug.Log("HighlighterOff.OffRails(): Enabling Highlight");
                for (int indexParts = 0; indexParts < vesselToFix.Parts.Count; indexParts++)
                {
                    vesselToFix.Parts[indexParts].highlightType = Part.HighlightType.OnMouseOver;
                }
            }

        }

        public void OnDestroy()
        {
            Debug.Log("HighlighterOff.OnDestroy()");
            GameEvents.onVesselGoOffRails.Remove(OffRails);
        }
    }
}
