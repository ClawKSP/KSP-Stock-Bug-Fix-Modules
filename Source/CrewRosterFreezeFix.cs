/*
 * This module written by Claw.. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.90.0
 *
 * CrewRosterFreezeFix v0.1.0
 * 
 * Change Log:
 * 
 * v0.1.0 (20 Dec 14) - Initial Release
 */

using UnityEngine;
using KSP;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class CRFFix : MonoBehaviour
    {
        // Probably not needed anymore, but left in here because KSP was starting it multiple times at one point.
        private static bool Started = false;

        public void Start ()
        {
            if (true == Started) return;

            Debug.Log("CRFFix.Start()");
            Started = true;

            GameEvents.onKerbalTypeChange.Add(TypeChange);

            GameEvents.onGUIAstronautComplexSpawn.Add(new EventVoid.OnEvent(this.ComplexEntry));
            GameEvents.onGUIAstronautComplexDespawn.Add(new EventVoid.OnEvent(this.ComplexExit));
        } // Start()

        public void ComplexEntry ()
        {
            // This isn't needed any more for the new, cleaner approach. But left here in case it's needed later.
            // Debug.LogWarning ("Entered!!!!");
        } // ComplexEntry()

        public void TypeChange(ProtoCrewMember KerbalCrew, ProtoCrewMember.KerbalType TypeFrom, ProtoCrewMember.KerbalType TypeTo)
        {
            // Captures when Kerbals experience a TypeChange. If the TypeChange is from Crew to Applicant, the kerbal is being fired.
            // Upon firing, the kerbal is set to "Missing" to indicate that the kerbal was modified during this visit to the complex.
            // ComplexExit() can then search through the flags. If the type is from Applicant to Crew, the flag is set to "Available"
            // to ensure it doesn't get saved incorrectly if a kerbal was fired, then rehired.
            
            // Debug.Log("CRFFix.TypeChange() From: " + TypeFrom + " To: " + TypeTo);

            if (ProtoCrewMember.KerbalType.Crew == TypeFrom
                && ProtoCrewMember.KerbalType.Applicant == TypeTo)
            {
                // Debug.Log("CRFFix: Setting Kerbal Status to Missing");
                KerbalCrew.rosterStatus = ProtoCrewMember.RosterStatus.Missing;
            }
            else if (ProtoCrewMember.KerbalType.Applicant == TypeFrom
                && ProtoCrewMember.KerbalType.Crew == TypeTo)
            {
                // Debug.Log("CRFFix: Setting Kerbal Status to Available");
                KerbalCrew.rosterStatus = ProtoCrewMember.RosterStatus.Available;
            }
        } // TypeChange()

        public void ComplexExit()
        {
            // Grab the list of Applicants who are marked as "missing." This checks the UTaR date to see if it is
            // in the past or future. If it's in the past, then the kerbal was hired previously. If it's in the future,
            // the kerbal was hired and fired during the last Astronaut Complex visit. Only the Applicants who have a
            // UTaR in the past (previously hired) need to be archived into "Unowned" type.

            Debug.LogWarning("CRFFix.ComplexExit()");

            // Grab the list of Applicants who are listed as Missing
            IEnumerator<ProtoCrewMember> enumerator = HighLogic.CurrentGame.CrewRoster.Kerbals(ProtoCrewMember.KerbalType.Applicant,
                new ProtoCrewMember.RosterStatus[] {ProtoCrewMember.RosterStatus.Missing}).GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    ProtoCrewMember CrewToCheck = enumerator.Current;
                    //Debug.LogWarning("Applicant Name = " + CrewToCheck.name
                    //    + " || UTaR = " + CrewToCheck.UTaR
                    //    + " || Time = " + HighLogic.CurrentGame.UniversalTime
                    //    + " || Status = " + CrewToCheck.rosterStatus);

                    if (CrewToCheck.UTaR < HighLogic.CurrentGame.UniversalTime)
                    {
                        Debug.Log("CRFFix: Fired Kerbal Detected and set to Unowned");
                        CrewToCheck.type = ProtoCrewMember.KerbalType.Unowned;
                        CrewToCheck.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                        CrewToCheck.UTaR = double.PositiveInfinity;
                    }
                    
                }
            }
            finally
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }

            // Grab the list of Applicants who are listed as Missing
            enumerator = HighLogic.CurrentGame.CrewRoster.Kerbals(ProtoCrewMember.KerbalType.Crew,
                new ProtoCrewMember.RosterStatus[] { ProtoCrewMember.RosterStatus.Available }).GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    ProtoCrewMember CrewToCheck = enumerator.Current;
                    if (CrewToCheck.UTaR > HighLogic.CurrentGame.UniversalTime)
                    {
                        Debug.Log("CRFFix: Newly Hired Kerbal Detected");
                        CrewToCheck.UTaR = HighLogic.CurrentGame.UniversalTime;
                    }

                }
            }
            finally
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }

        } // ComplexExit()

        public void Destroy()
        {
            Debug.LogWarning("CRFFix.Destroy()");

            if (true == Started)
            {
                GameEvents.onGUIAstronautComplexSpawn.Remove(new EventVoid.OnEvent(this.ComplexEntry));
                GameEvents.onGUIAstronautComplexDespawn.Remove(new EventVoid.OnEvent(this.ComplexExit));

                GameEvents.onKerbalTypeChange.Remove(TypeChange);

                Started = false;
            }
        } // Destroy()

    } // CRFFix

} // ClawKSP
