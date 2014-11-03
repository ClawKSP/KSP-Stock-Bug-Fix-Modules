KSP-Stock-Bug-Fix-Modules

=========================



Stand alone fixes for common stock KSP bugs. These modules are meant to be fully stock compatible. My aim is to be able to remove them from the game at any time without causing a problem for the stock saves.





Bug fixes included so far

=========================



+ ChuteQuickloadFixer.dll - Fixes "Tiny Chutes" or disappearing chutes on quickload

--

+ AnchoredDecouplerFix.dll - Fixes radial decouplers not wanting to decouple correctly at high speed (leading to boosters striking the core stack)

--

+ EVAEjectionFix.dll - Fixes the bug that causes a kerbal to be ejected away from a capsule upon EVA.

--

+ SymmetryActionFix.dll - Retains action groups for symmetric parts when they are removed and replaced in the editor.
*** Please use caution with this. It's done in a way that shouldn't break anything, but symmetry itself is sometimes flaky in KSP. ***
Please read a very important note below.
This fixes the problem where action groups are lost on symmetric parts when they are removed and replaced.
Please bear in mind that this fix ONLY fixes the loss of action groups on symmetric parts. It does not fix any other symmetry bugs in the editor. As such, there are some symmetry bugs that cause this fix to fail. If you place a part and the action groups don't transfer, try lifting the part (or part tree) and placing again.

Of specific note, if you pick up a part that has children, the children become disassociated from their siblings. KSP fixes itself, but my module fails before that happens. Also, symmetry within symmetry is still buggy in stock, and my action group fix will suffer from those bugs. For example: If you lift a part, change the symmetry multiplier, then place the part back on, the module will fail. Again, if you hit one of these bugs, lift the part and place it again.

You can watch the ALT+F2 debug log. My module doesn't throw any warnings. So if you see warnings, it's the stock editor fixing itself. My module shouldn't break anything new, but I didn't want people to think that symmetry is completely fixed with this module.

--

+ LargeCraftLaunchFix.dll - Fixes launch pad and runway explosions when launching large vessels.





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
v0.1.4 (2 Nov 14) - Initial release of LargeCraftLaunchFix.dll
v0.1.3 and v0.1.3a (1 Nov 14) - Initial release of SymmetryActionFix.dll
v0.1.2a (27 Oct 14) - Removed "Reset" message from ChuteQuickloadFixer.dll and recompiled for .NET 3.5, updated readme to accomodate releases
v0.1.2 (21 Oct 14) - Reworked AnchoredDecouplerFix to better handle struts and prevent decouplers from ripping off. (Should work like pre KSP v0.24.2)
v0.1.1a (21 Oct 14) - Updated error handling in EVAEjectionFix to prevent log spam and kerbal lockup with incompatible mod
v0.1.1 (19 Oct 14) - Release of EVAEjectionFix.dll and reduced some log spam with ChuteQuickloadFixer and AnchoredDecouplerFix



v0.1 (17 Oct 14) - Initial release, includes ChuteQuickloadFixer and AnchoredDecouplerFix
