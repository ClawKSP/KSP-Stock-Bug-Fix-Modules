/*
 * This module written by Claw (with an incredible amount of help from NathanKell). Please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * SymmetryActionFix Written for KSP v1.0
 * 
 * This plugin manages the symmetric sibilings so that they retain action gropus when pulled off
 * and replaced. As of v0.1.4, this module also manages symmetric siblings better in that they now
 * stay grouped together properly even in recursive symmetric builds. This fixes some of the building
 * symmetry on symmetry bugs.
 * 
 * Change Log:
 * 
 * v01.03  (1 Jul 15)  - Recompiled for KSP v1.0.4
 * v01.02 (9 May 15)   - Added ability to copy procedural fairings.
 * v01.01 (1 May 15)   - Recompiled and tested for KSP v1.0.2
 * v01.00 (26 Apr 15)  - Updated for KSP v1.0
 * v0.1.5 (28 Feb 15)  - Fixed engine icons coming apart in stage sequence. Added debug highlighting and action key toggle.
 *   Set SPH to default to mirror symmetry for singular parts. Standardized debug log entries.
 * v0.1.4a (12 Jan 15) - Fixed the SPH so that building one wing first, then copying with mirror symmetry doesn't
 *   cause parts to misattach to the wings. SPH defaults to mirror and the VAB defaults to radial (as in pre v0.90).
 * v0.1.4 (6 Jan 15) - Fixed several serious bugs. SAFix is much more robust and cleans up some more stock symmetry bugs
 * v0.1.3 (29 Dec 14) - Updated, recompiled, and tested for KSP v0.90.0
 * v0.1.2 - Updated to handle symmetry within symmetry
 * v0.1.1 - Added some error checking to ensure parts don't become mismatched.
 * v0.1.0 - Initial release
 * 
 */
using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class SAFix : UnityEngine.MonoBehaviour
    {

        private bool DebugHighlightActive = false;
        private KeyCode BoundKey = KeyCode.H;

        public void Awake()
        {
            // Do nothing for now
            // Debug.LogWarning("SSAFix.Awake()");
        }

        public void Start()
        {
            Debug.Log("SymmetryActionFix.Start(): v01.01");
            GameEvents.onPartAttach.Add(onPartAttach);

            if (null != GameDatabase.Instance.GetConfigNodes("SAFIX_HIGHLIGHT"))
            {
                ConfigNode CNBinding = new ConfigNode();
                CNBinding = GameDatabase.Instance.GetConfigNodes("SAFIX_HIGHLIGHT")[0];

                if (null != CNBinding)
                {
                    string BindingString = CNBinding.GetValue("primary");
                    if (!string.IsNullOrEmpty(BindingString))
                    {
                        KeyCode BoundKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), BindingString);
                        if (KeyCode.None == BoundKey)
                        {
                            BoundKey = KeyCode.H;
                        }
                    }
                    BindingString = CNBinding.GetValue("active");
                    if (!string.IsNullOrEmpty(BindingString))
                    {
                        bool Result;
                        DebugHighlightActive = System.Boolean.TryParse(BindingString, out Result);// (bool)System.Enum.Parse(typeof(bool), BindingString);
                        if (false == Result)
                        {
                            DebugHighlightActive = false;
                        }
                    }
                }
            }

            if (null != EditorLogic.fetch.ship
                && null != EditorLogic.fetch.ship.parts
                && 0 != EditorLogic.fetch.ship.parts.Count)
            {
                HighlighterHook(EditorLogic.fetch.ship.parts[0]);
            }
        }

        public void Update()
        {
            // MOD+H toggles the debug highlighter
            if (GameSettings.MODIFIER_KEY.GetKey() && Input.GetKeyDown(BoundKey))
            {
                DebugHighlightActive = !DebugHighlightActive;
                if (true == DebugHighlightActive)
                {
                    ScreenMessages.PostScreenMessage("Symmetry Highlight: ON", 3.0f, ScreenMessageStyle.UPPER_CENTER);
                }
                else
                {
                    ScreenMessages.PostScreenMessage("Symmetry Highlight: OFF", 3.0f, ScreenMessageStyle.UPPER_CENTER);
                }

                // Reset the highlighter based on the new selection
                if (null != EditorLogic.RootPart)
                {
                    DeactivateHighlight(EditorLogic.RootPart);
                    HighlighterHook(EditorLogic.RootPart);
                }
            }
        }

        public void OnDestroy()
        {
            Debug.Log("SAFix.OnDestroy()");
            GameEvents.onPartAttach.Remove(onPartAttach);
        } // OnDestroy

        void onPartAttach(GameEvents.HostTargetAction<Part, Part> AttachedPart)
        {
            Debug.Log("SAFix.onPartAttach(): Host = " + AttachedPart.host.name + " |  Target = " + AttachedPart.target.name
                + "  |  Symmetry Count = " + AttachedPart.host.symmetryCounterparts.Count);
            
            // If the target is the root part, attach the highlighter starting at the root.
            if (AttachedPart.target == EditorLogic.RootPart)
            {
                HighlighterHook(AttachedPart.target);
            }
            else
            {
                HighlighterHook(AttachedPart.host);
            }
            
            // Check if symmetry is set to 1x, and set default behavior according to SPH/VAB.
            if (0 == EditorLogic.fetch.symmetryMode)
            {
                // Debug.LogWarning("symmetryMode " + EditorLogic.fetch.symmetryMode);
                if (EditorDriver.editorFacility == EditorFacility.SPH)
                {
                    Debug.Log("SAFix.onPartAttach(): 1x symmetry in SPH detected. Mirror symmetry set.");
                    AttachedPart.host.symMethod = SymmetryMethod.Mirror;
                }
                else // EditorDriver.editorFacility == EditorFacility.VAB
                {
                    Debug.Log("SAFix.onPartAttach(): 1x symmetry in VAB detected. Radial symmetry set.");
                    AttachedPart.host.symMethod = SymmetryMethod.Radial;
                }
                return;
            }
            
            if (0 == AttachedPart.host.symmetryCounterparts.Count)
            {
                // This part has no symmetric siblings parts. No need to copy the action groups.
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

            // This is needed because the editor logic will bug out if the symmetry multiplier is left at more than 8x.
            // Debug.Log("SAFix.onPartAttach(): symmetryMode = " + EditorLogic.fetch.symmetryMode);
            if (EditorLogic.fetch.symmetryMode > 7)
            {
                EditorLogic.fetch.symmetryMode = 7;
            }
        }

        // Recurse through the master branch and the newly placed symmetric branch, copying over part info.
        void UpdatePartAndChildren(Part SourcePart, Part UpdatePart, SymmetryMethod SymMethod)
        {
            if (null == SourcePart || null == UpdatePart)
            {
                // Null parts were passed to the updater. This should not happen, but just in case...
                return;
            }

            // Fix the bug where staging icons split from each other.
            UpdatePart.originalStage = SourcePart.originalStage;

            // This shouldn't be needed anymore.
            if (0 == UpdatePart.symmetryCounterparts.Count)
            {
                // This part has no mirrored parts. No need to copy the action groups.
                return;
            }

            CheckAndAddCounterpart(SourcePart, UpdatePart);
            PropagateCounterparts(SourcePart);

            // Propagates symmetry method throughout the branch. If it ever hits radial symmetry, the rest of the 
            // branch needs to be listed as symmetry to prevent editor breakage with nested mirror symmetry.
            if (SourcePart.symmetryCounterparts.Count > 1)
            {
                SymMethod = SymmetryMethod.Radial;
            }
            //if(SourcePart.symMethod == SymmetryMethod.Radial)
            //{
            //    SymMethod = SymmetryMethod.Radial;
            //}

            // Debug.LogWarning("SymmMethod: " + SymMethod);
            SourcePart.symMethod = SymMethod;
            UpdatePart.symMethod = SymMethod;

            if (SourcePart.Modules.Count != UpdatePart.Modules.Count)
            {
                Debug.LogError("SAFix.onPartAttach(): Part Copy Error. Module Count Mismatch.");
                return;
            }

            // Loop through all the modules. Action groups are stored inside the PartModules
            for (int IndexModules = 0; IndexModules < UpdatePart.Modules.Count; IndexModules++)
            {
                if (SourcePart.Modules[IndexModules].Actions.Count != UpdatePart.Modules[IndexModules].Actions.Count)
                {
                    Debug.LogError("SAFix.UpdatePartAndChildren(): Actions Mismatch in Module. Action copy aborted at Module: "
                        + SourcePart.Modules[IndexModules].moduleName);
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

                if ("ModuleProceduralFairing" == SourcePart.Modules[IndexModules].moduleName)
                {
                    Debug.Log("SymmetryActionFix: Fixing Procedural Fairing");

                    ModuleProceduralFairing MPFSource = (ModuleProceduralFairing)SourcePart.Modules[IndexModules];
                    ModuleProceduralFairing MPFUpdate = (ModuleProceduralFairing)UpdatePart.Modules[IndexModules];

                    if (MPFSource.xSections.Count != 0)
                    {
                        MPFUpdate.xSections.AddRange(MPFSource.xSections);

                        MethodInfo MPFMethod = MPFUpdate.GetType().GetMethod("SpawnMeshes", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (MPFMethod != null)
                        {
                            MPFMethod.Invoke(MPFUpdate, new object[] { true });
                        }

                    }
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

            // Check if the update part already exists as a counterpart in the source
            for (int IndexCounterparts = 0; IndexCounterparts < SourcePart.symmetryCounterparts.Count; IndexCounterparts++)
            {
                if (SourcePart.symmetryCounterparts[IndexCounterparts] == UpdatePart) { return; }
            }

            SourcePart.symmetryCounterparts.Add(UpdatePart);
        } // CheckAndAddCounterpart

        private void PropagateCounterparts(Part SourcePart)
        {
            if (null == SourcePart) { return; }

            // Step through each counterpart in the master part
            for (int IndexCounterparts = 0; IndexCounterparts < SourcePart.symmetryCounterparts.Count; IndexCounterparts++)
            {
                Part Counterpart = SourcePart.symmetryCounterparts[IndexCounterparts];

                Counterpart.symmetryCounterparts.Clear(); // wipe out the old counterparts
                Counterpart.symmetryCounterparts.Add(SourcePart); // add the master part to the list

                Counterpart.symmetryCounterparts.AddRange(SourcePart.symmetryCounterparts); // copy the master part's list
                Counterpart.symmetryCounterparts.Remove(Counterpart); // remove the part from it's own counterpart list

                //Debug.LogWarning("Counterparts: " + SourcePart.symmetryCounterparts.Count
                //    + " || Mirror Counterparts: " + MirrorPart.symmetryCounterparts.Count);
            }

        } // PropagateCounterparts

        // Recurse through a branch and attach the highlighter function to each part.
        public void HighlighterHook(Part AttachedPart)
        {
            // Remove the highlighter whenever called. Prevents highlighter stacking.
            AttachedPart.RemoveOnMouseEnter(ActivateHighlight);
            AttachedPart.RemoveOnMouseExit(DeactivateHighlight);

            if (true == DebugHighlightActive)
            {
                AttachedPart.AddOnMouseEnter(ActivateHighlight);
                AttachedPart.AddOnMouseExit(DeactivateHighlight);
            }

            if (null == AttachedPart.children) { return; }

            for (int IndexChildren = 0; IndexChildren < AttachedPart.children.Count; IndexChildren++)
            {
                HighlighterHook(AttachedPart.children[IndexChildren]);
            }
        }

        // Turns on highlighting for a given branch when moused over
        public void ActivateHighlight (Part HighlightPart)
        {
            HighlightPartAndChildren(HighlightPart, true, true, true);
        }

        // Turns off highlighting when mouse over is removed
        public void DeactivateHighlight (Part HighlightPart)
        {
            HighlightPartAndChildren(HighlightPart, true, true, false);
        }

        // Recurse through a branch and highlight the parts based on main vs. sibling branches
        // isParent = true means that the part is the first for a given branch (main or sibling branch)
        // isMainBranch = true means that the part is in the main moused over branch
        // Activate = true turns on highligting, false turns it off
        public void HighlightPartAndChildren (Part HighlightPart, bool isParent, bool isMainBranch, bool Activate)
        {
            if (null == HighlightPart) { return; }

            Color HighlightColor = Color.white;

            //if (true == inParentBranch) // In main parent branch
            //{
            //    if (true == isParent)
            //    {
            //        HighlightColor = Color.blue;
            //    }
            //    else
            //    {
            //        HighlightColor = Color.magenta;
            //    }
            //}
            //else // In sibling branch
            //{
            //    if (true == isParent)
            //    {
            //        HighlightColor = Color.blue;
            //    }
            //    else
            //    {
            //        HighlightColor = Color.cyan;
            //    }
            //}

            if (true == isMainBranch) // In main  branch
            {
               HighlightColor = Color.green;
            }
            else
            {
                HighlightColor = Color.blue;
            }
            if (true != isParent) // Subdue Children
            {
                HighlightColor *= 0.667f;
            }

            if (true == Activate)
            {
                HighlightPart.SetHighlightColor(HighlightColor);
            }
            else
            {
                HighlightPart.SetHighlightColor();
            }
            HighlightPart.SetHighlight(Activate, false);

            // highlight any children parts
            if (null != HighlightPart.children)
            {
                for (int IndexChildren = 0; IndexChildren < HighlightPart.children.Count; IndexChildren++)
                {
                    Part Child = HighlightPart.children[IndexChildren];
                    HighlightPartAndChildren(Child, false, isMainBranch, Activate);
                }
            }

            if (false == isParent || false == isMainBranch)
            {
                return; // If not the main branch parent, do not touch counterparts
            } 
            if (null != HighlightPart.symmetryCounterparts)
            {
                for (int IndexSibling = 0; IndexSibling < HighlightPart.symmetryCounterparts.Count; IndexSibling++)
                {
                    Part Sibling = HighlightPart.symmetryCounterparts[IndexSibling];
                    if (true == isParent)
                    {
                        HighlightPartAndChildren(Sibling, isParent, false, Activate);
                    }
                    
                }
            }
        }
        
        public void HighlightParent (Part Parent)
        {
            if (null == Parent) { return; }
            Debug.LogError("SAFix.HighlightParent()");

            Parent.SetHighlightColor(Color.blue);
            Parent.SetHighlight(true, false);

            if (null == Parent.symmetryCounterparts) { return; }

            for (int IndexSiblings = 0; IndexSiblings < Parent.symmetryCounterparts.Count; IndexSiblings++)
            {
                Part Sibling = Parent.symmetryCounterparts[IndexSiblings];
                Sibling.SetHighlightColor(Color.cyan);
                Sibling.SetHighlight(true, true);
            }
        }

        public void RemoveHighlight (Part HighlightPart)
        {
            if (null == HighlightPart) { return; }
            Debug.LogError("SAFix.RemoveHighlight()");

            HighlightPart.SetHighlightColor();
            HighlightPart.SetHighlight(false, false);

            if (null == HighlightPart.symmetryCounterparts) { return; }

            for (int IndexSiblings = 0; IndexSiblings < HighlightPart.symmetryCounterparts.Count; IndexSiblings++)
            {
                Part Sibling = HighlightPart.symmetryCounterparts[IndexSiblings];
                Sibling.SetHighlightColor();
                Sibling.SetHighlight(false, true);
            }
        }
    }
}
