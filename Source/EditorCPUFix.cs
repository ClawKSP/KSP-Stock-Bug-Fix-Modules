/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * EditorCPUFix - Written for KSP v1.1.2
 * 
 * Deactivates VAB and SPH lighting, ground crew, and vehicles.
 * 
 * Change Log:
 * - v00.01  (30 May 16)   Initial Release
 * 
 */

using UnityEngine;
using KSP;
using System.Collections;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class EditorCPUFix : MonoBehaviour
    {
        public void Start ()
        {
            Debug.Log("EditorCPUFix.Start(): v00.01");
            StartCoroutine(DisableDoodads());
        }


        IEnumerator DisableDoodads()
        {
            GameObject editorObject = null;
            string crewName = "VABCrew";

            while (EditorDriver.fetch == null)
                yield return null;

            if (EditorDriver.editorFacility == EditorFacility.SPH)
                crewName = "SPHCrew";

            if (GameSettings.SHOW_SPACE_CENTER_CREW)
            {
                while (editorObject == null)
                {
                    yield return null;
                    editorObject = GameObject.Find(crewName);
                }
                editorObject = GameObject.Find(crewName);
                if (editorObject) editorObject.SetActive(false);
            }

            editorObject = null;
            while (editorObject == null)
            {
                yield return null;
                editorObject = GameObject.Find("model_props");
            }
            editorObject.SetActive(false);

            if (EditorDriver.editorFacility == EditorFacility.SPH)
            {
                editorObject = GameObject.Find("model_sph_interior_lights_v16");
                if (editorObject != null) editorObject.SetActive(false);
                editorObject = GameObject.Find("Lighting_Baked");
                if (editorObject != null) editorObject.SetActive(false);
            }
            else // VAB
            {
                editorObject = GameObject.Find("model_vab_prop_truck_01");
                if (editorObject != null) editorObject.SetActive(false);
                editorObject = GameObject.Find("model_vab_elevators");
                if (editorObject != null) editorObject.SetActive(false);
                editorObject = GameObject.Find("VAB_Interior_BakeLights");
                if (editorObject != null) editorObject.SetActive(false);
                editorObject = GameObject.Find("model_vab_interior_lights_flood_v16");
                if (editorObject != null) editorObject.SetActive(false);
            }

        }
    }
}
