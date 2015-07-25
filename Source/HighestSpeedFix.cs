/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * HighestSpeedFix - Written for KSP v1.0
 * - Uncaps the Highest Speed Achieved value in the Flight Log (doesn't max at 750 m/s).
 * 
 * Change Log:
 * - v01.00  (23 Jul 15) Initial release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;
using System;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class HighestSpeedFix : MonoBehaviour
    {
        private FlightLogger flightLogger;

        private FieldInfo[] FlightLoggerFields;

        // private int geeForceIndex = 4;
        private int highestSpeedIndex = 6;
        //private int highestLandSpeedIndex = 7;
        private double highestSpeed = 0d;
        //private double highestLandSpeed = 0d;

        public void Start()
        {
            Debug.Log("HighestSpeedFix.Start(): v01.00");

            GetLogger();

            if (flightLogger != null)
            {
                FlightLoggerFields[highestSpeedIndex].SetValue(flightLogger, 0d);
                //FlightLoggerFields[highestLandSpeedIndex].SetValue(flightLogger, 0d);
            }
        }

        private void GetLogger ()
        {
            FlightLogger FL = new FlightLogger();

            FieldInfo[] tempFL = FL.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Static);

            //Debug.LogWarning("tempFL: " + tempFL.Length);

            //FieldInfo t = FL.GetType().GetField("", BindingFlags.NonPublic | BindingFlags.Static);
            //if (t != null)
            //{
            //    Debug.LogWarning("Found it");
            //}

            for (int indexTemp = 0; indexTemp < tempFL.Length; indexTemp++)
            {
                if (tempFL[indexTemp].FieldType == typeof(FlightLogger))
                {
                    flightLogger = (FlightLogger)tempFL[indexTemp].GetValue(FL);
                    FlightLoggerFields = flightLogger.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
            DestroyObject(FL);
        }

        public void FixedUpdate()
        {
            
        }

        public void LateUpdate()
        {
            if (flightLogger == null)
            {
                GetLogger();
            }
            else
            {
                highestSpeed = Math.Max((FlightGlobals.ship_velocity + Krakensbane.GetFrameVelocity()).magnitude, highestSpeed);
                //highestLandSpeed = Math.Max((FlightGlobals.ship_srfVelocity - Vector3d.Project(FlightGlobals.ship_srfVelocity, FlightGlobals.upAxis)).magnitude, highestLandSpeed);

                FlightLoggerFields[highestSpeedIndex].SetValue(flightLogger, highestSpeed);
                //FlightLoggerFields[highestLandSpeedIndex].SetValue(flightLogger, highestLandSpeed);
            }
        }
    }
}
