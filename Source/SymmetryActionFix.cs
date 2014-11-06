/*
 * This module written by Claw (with an incredible amount of help from NathanKell). Please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.25.0
 *
 * SymmetryActionFix v0.1.1
 * 
 * Beware, this plugin doesn't fix any symmetry issues except keeping action groups for symmetric parts.
 * 
 * Change Log:
 * 
 * v0.1.1 - Added some error checking to ensure parts don't become mismatched.
 * v0.1.0 - Initial release
 * 
 */
using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SymmetryActionFix : UnityEngine.MonoBehaviour
    {

        public void Awake()
        {
            // Do nothing for now
            // Debug.LogWarning("SymmetryActionFix.Awake()");
        }

        public void Start()
        {
            Debug.Log("SymmetryActionFix.Start()");
            GameEvents.onPartAttach.Add(onPartAttach);
        }

        void onPartAttach(GameEvents.HostTargetAction<Part, Part> AttachedPart)
        {
            Debug.Log("Attach: Host = " + AttachedPart.host.name + " |  Target = " + AttachedPart.target.name
                + "  |  Symmetry Count = " + AttachedPart.host.symmetryCounterparts.Count);

            if (0 == AttachedPart.host.symmetryCounterparts.Count)
            {
                // This part has no mirrored parts. No need to copy the action groups.
                return;
            }

            if (null == AttachedPart.host.symmetryCounterparts[0].parent)
            {
                // This part is the first part. No need to copy the action groups.
                // Debug.LogError("Null Parent 0");
                return;
            }

            UpdatePartAndChildren(AttachedPart.host);
        }

        void UpdatePartAndChildren (Part UpdatePart)
        {
            if (0 == UpdatePart.symmetryCounterparts.Count)
            {
                // This part has no mirrored parts. No need to copy the action groups.
                return;
            }

            Part SourcePart = UpdatePart.symmetryCounterparts[0];

            if (SourcePart.Modules.Count != UpdatePart.Modules.Count)
            {
                Debug.LogError("SymmetryActionFix: Part Copy Error. Module Mismatch.");
                return;
            }

            // Loop through all the modules. Action groups are stored inside the PartModules
            for (int IndexModules = 0; IndexModules < UpdatePart.Modules.Count; IndexModules++)
            {
                if (SourcePart.Modules[IndexModules].Actions.Count != UpdatePart.Modules[IndexModules].Actions.Count)
                {
                    Debug.LogError("SymmetryActionFix: Actions Mismatch in Module " + SourcePart.Modules[IndexModules].moduleName);
                    return;
                }
                // Loop through all the Actions for this module.
                for (int IndexActions = 0; IndexActions < SourcePart.Modules[IndexModules].Actions.Count; IndexActions++)
                {
                    
                    // Copy the Action's triggers.
                    // Debug.LogWarning("Module/Action " + UpdatePart.Modules[IndexModules].Actions[IndexActions].name);
                    UpdatePart.Modules[IndexModules].Actions[IndexActions].actionGroup =
                        SourcePart.Modules[IndexModules].Actions[IndexActions].actionGroup;
                    
                }
            }

            for (int IndexChild = 0; IndexChild < UpdatePart.children.Count; IndexChild++)
            {
                // Go through all the children parts and copy the actions.
                UpdatePartAndChildren (UpdatePart.children[IndexChild]);
            }
        }

        public void OnDestroy()
        {
            Debug.Log("SymmetryActionFix.OnDestroy()");
            GameEvents.onPartAttach.Remove(onPartAttach);
        }
    }
}
