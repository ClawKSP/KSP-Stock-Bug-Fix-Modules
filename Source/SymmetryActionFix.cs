/*
 * This module written by Claw (with an incredible amount of help from NathanKell). Please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.90.0
 *
 * SymmetryActionFix v0.1.4
 * 
 * This plugin manages the symmetric sibilings so that they retain action gropus when pulled off
 * and replaced. As of v0.1.4, this module also manages symmetric siblings better in that they now
 * stay grouped together properly even in recursive symmetric builds. This fixes some of the building
 * symmetry on symmetry bugs.
 * 
 * Change Log:
 * 
 * v0.1.4 (6 Jan 15) - Fixed several serious bugs. SAFix is much more robust and cleans up some more stock symmetry bugs
 * v0.1.3 (29 Dec 14) - Updated, recompiled, and tested for KSP v0.90.0
 * v0.1.2 - Updated to handle symmetry within symmetry
 * v0.1.1 - Added some error checking to ensure parts don't become mismatched.
 * v0.1.0 - Initial release
 * 
 */
using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SAFix : UnityEngine.MonoBehaviour
    {

        public void Awake()
        {
            // Do nothing for now
            // Debug.LogWarning("SSAFix.Awake()");
        }

        public void Start()
        {
            Debug.Log("SAFix.Start()");
            GameEvents.onPartAttach.Add(onPartAttach);
        }

        void onPartAttach(GameEvents.HostTargetAction<Part, Part> AttachedPart)
        {
            Debug.Log("SAFix.onPartAttach(): Host = " + AttachedPart.host.name + " |  Target = " + AttachedPart.target.name
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

            UpdatePartAndChildren(AttachedPart.host.symmetryCounterparts[0], AttachedPart.host,
                AttachedPart.host.symmetryCounterparts[0].symMethod);

            Debug.LogWarning("symmetryMode: " + EditorLogic.fetch.symmetryMode);
            if (EditorLogic.fetch.symmetryMode > 7)
            {
                EditorLogic.fetch.symmetryMode = 7;
            }
        }

        void UpdatePartAndChildren(Part SourcePart, Part UpdatePart, SymmetryMethod SymMethod)
        {
            if (null == SourcePart || null == UpdatePart)
            {
                // Null parts were passed to the updater. This should not happen, but just in case...
                return;
            }

            if (0 == UpdatePart.symmetryCounterparts.Count)
            {
                // This part has no mirrored parts. No need to copy the action groups.
                return;
            }

            if (SourcePart.Modules.Count != UpdatePart.Modules.Count)
            {
                Debug.LogWarning("SAFix: Part Copy Error. Module Count Mismatch.");
                return;
            }

            CheckAndAddCounterpart(SourcePart, UpdatePart);
            PropagateCounterparts(SourcePart);
            if(SourcePart.symMethod == SymmetryMethod.Radial)
            {
                SymMethod = SymmetryMethod.Radial;
            }
            // Debug.LogWarning("SymmMethod: " + SymMethod);
            SourcePart.symMethod = SymMethod;
            UpdatePart.symMethod = SymMethod;

            // Loop through all the modules. Action groups are stored inside the PartModules
            for (int IndexModules = 0; IndexModules < UpdatePart.Modules.Count; IndexModules++)
            {
                if (SourcePart.Modules[IndexModules].Actions.Count != UpdatePart.Modules[IndexModules].Actions.Count)
                {
                    Debug.LogWarning("SAFix: Actions Mismatch in Module. Actions not copied for Module: " + SourcePart.Modules[IndexModules].moduleName);
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
                UpdatePartAndChildren(SourcePart.children[IndexChild], UpdatePart.children[IndexChild], SymMethod);
            }
        } // UpdatePartAndChild

        private void CheckAndAddCounterpart(Part SourcePart, Part UpdatePart)
        {
            if (SourcePart == UpdatePart) { return; }

            for (int IndexCounterparts = 0; IndexCounterparts < SourcePart.symmetryCounterparts.Count; IndexCounterparts++)
            {
                if (SourcePart.symmetryCounterparts[IndexCounterparts] == UpdatePart) { return; }
            }

            SourcePart.symmetryCounterparts.Add(UpdatePart);
        } // CheckAndAddCounterpart

        private void PropagateCounterparts(Part SourcePart)
        {
            if (null == SourcePart) { return; }

            for (int IndexCounterparts = 0; IndexCounterparts < SourcePart.symmetryCounterparts.Count; IndexCounterparts++)
            {
                Part MirrorPart = SourcePart.symmetryCounterparts[IndexCounterparts];

                MirrorPart.symmetryCounterparts.Clear();
                MirrorPart.symmetryCounterparts.Add(SourcePart);

                MirrorPart.symmetryCounterparts.AddRange(SourcePart.symmetryCounterparts);
                MirrorPart.symmetryCounterparts.Remove(MirrorPart);

                //Debug.LogWarning("Counterparts: " + SourcePart.symmetryCounterparts.Count
                //    + " || Mirror Counterparts: " + MirrorPart.symmetryCounterparts.Count);
            }

        } // PropagateCounterparts

        public void OnDestroy()
        {
            Debug.Log("SAFix.OnDestroy()");
            GameEvents.onPartAttach.Remove(onPartAttach);
        } // OnDestroy
    }
}
