KSP-Stock-Bug-Fix-Modules

=========================


Stand alone fixes for common stock KSP bugs. These modules are meant to be fully stock compatible. My aim is to be able to remove them from the game at any time without causing a problem for the stock saves.





Bug fixes included so far

=========================


+ ChuteQuickloadFixer.dll - Fixes "Tiny Chutes" or disappearing chutes on quickload

--

+ AnchoredDecouplerFix.dll - Fixes radial decouplers not wanting to decouple correctly at high speed (leading to boosters striking the core stack)
  - Updated for compatibility with Kerbquake

--

+ EVAEjectionFix.dll - Fixes the bug that causes a kerbal to be ejected away from a capsule upon EVA.
  - Nullifies ladder slide bug for initial EVA.

--

+ KerbalDebrisFix.dll - Fixes kerbals turning into debris when crashing vehicles containing External Command Seats.
  - Also recovers kerbals who have been previous frozen by this bug.

--

+ CrewRosterFreezeFix.dll - Fixes a bug where firing a kerbal who had logged Achiements caused the game to lock up and corrupt the save.
  - Now handles firing kerbals who are MIA and works when managing kerbals from the Editors.

--

+ StickyLaunchPadFix.dll - Fixes the tier 2 launch pad, where certain rocket configurations "stick" to the launch pad.

--

+ SymmetryActionFix.dll - Retains action groups for symmetric parts when they are removed and replaced in the editor.
*** Please use caution with this. It's done in a way that shouldn't break anything, but symmetry itself is sometimes flaky in KSP. ***
Please read a very important notes below.
I was tired of bandaid fixing this patch, so I've attacked it on a more fundamental level. In addition to fixing the problem where action groups are lost on symmetric parts when they are removed and replaced, This fix now fixes loss of action groups when placing symmetric parts, prevents symmetric parts from becoming disassociated (where you pull off a part, and some of the mirrors stay). It also fixes some bugs with recursive symmetry. There are still problems with the editor building recursive symmetry that I haven't fixed.

So, symmetry within symmetry is still a bit buggy in stock. This fix can handle copying Action groups buried in symmetry within symmetry, but might still fail when encounering certain stock bugs.



=======


Depricated

--

+ [DEPRICATED FOR v0.90] LargeCraftLaunchFix.dll - Fixes launch pad and runway explosions when launching large vessels.
  - Now works through quickloads, scene transitions, and when approaching a craft on the pad/runway from outside physics range.
  - Functions correctly with Kerbal Joint Reinforcement installed.
  - DEPRICATED: This bug is now fixed in stock KSP.





=======




Installation

============



Just download the desired fix's .dll and place in the KSP/GameData directory. No other downloads, file changes, or add-ons are required.


Alternately, you can download the release and place the "StockBugFixModules" directory in the KSP/GameData directory.





License

=======



Covered under the CC-BY-NC-SA license. See the license.txt for more details.

(https://creativecommons.org/licenses/by-nc-sa/4.0/)





Change Log

==========
v0.1.7d (6 Jan 15) - Rebuild of SymmetryActionFix.dll. More robust at handling nested symmetry and collecting symmetric parts / action groups.
v0.1.7c (4 Jan 15) - Regressed SymmetryActionFix.dll again. Found further bugs that break new KSP symmetry features.
v0.1.7b (29 Dec 14) - Reinstated SymmetryActionFix.dll for KSP v0.90.0.705.
v0.1.7a (29 Dec 14) - Updated CrewRosterFreezeFix.dll to be a bit more robust. Reduced StickyLaunchPadFix.dll log spam.
v0.1.7 (23 Dec 14) - Initial release StickyLaunchPadFix.dll.
v0.1.6 (20 Dec 14) - Initial release CrewRosterFreezeFix.dll, regressed SymmetryActionFix.dll (broken in KSP v0.90), depricated LargeCraftLaunchFix.dll
v0.1.5c (10 Dec 14) - AnchoredDecouplerFix.dll updated for compatilibity with Kerbquake.
v0.1.5b (23 Nov 14) - EVAEjectionFix now nullifies ladder slide bug for initial EVA, and added minor error checking for KerbalDebrsFix.
v0.1.5a (22 Nov 14) - KerbalDebrisFix now properly recovers names for already frozen kerbals.
v0.1.5 (22 Nov 14) - Initial release of KerbalDebrisFix.dll
v0.1.4d (22 Nov 14) - LargeCraftLaunchFix.dll now functions correctly with Kerbal Joint Reinforcement installed.
v0.1.4c (6 Nov 14) - SymmetryActionFix.dll now handles action groups buried in symmetry within symmetry.
v0.1.4b (5 Nov 14) - Added some error checking to SymmetryActionFix.dll
v0.1.4a (3 Nov 14) - Updated LargeCraftLaunchFix.dll to be a bit more robust. Now works through quickloads, scene transitions, and coming within physics range.
v0.1.4 (2 Nov 14) - Initial release of LargeCraftLaunchFix.dll
v0.1.3 & v0.1.3a (1 Nov 14) - Initial release of SymmetryActionFix.dll
v0.1.2a (27 Oct 14) - Removed "Reset" message from ChuteQuickloadFixer.dll and recompiled for .NET 3.5, updated readme to accomodate releases
v0.1.2 (21 Oct 14) - Reworked AnchoredDecouplerFix to better handle struts and prevent decouplers from ripping off. (Should work like pre KSP v0.24.2)
v0.1.1a (21 Oct 14) - Updated error handling in EVAEjectionFix to prevent log spam and kerbal lockup with incompatible mod
v0.1.1 (19 Oct 14) - Release of EVAEjectionFix.dll and reduced some log spam with ChuteQuickloadFixer and AnchoredDecouplerFix
v0.1 (17 Oct 14) - Initial release, includes ChuteQuickloadFixer and AnchoredDecouplerFix
