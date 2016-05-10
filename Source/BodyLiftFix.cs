/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * BodyLiftFix - Written for KSP v1.1.2
 * 
 * - Fixes missing body lift in KSP v1.1.2
 * 
 * Change Log:
 * - v01.00  ( 9 May 16)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class BodyLiftFix : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("BodyLiftFix: v01.00");

            if (PhysicsGlobals.Instance != null)
                PhysicsGlobals.BodyLiftCurve = PhysicsGlobals.GetLiftingSurfaceCurve("BodyLift");
        }
    }
}
