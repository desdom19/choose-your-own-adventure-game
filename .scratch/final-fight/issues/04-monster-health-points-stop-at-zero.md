# 04: A Monster's Health Points stop at 0

**What to build:** A Monster's Health Points never show below 0, just like the player's, so the numbers make sense (spec: `.scratch/final-fight/spec.md`). This is the only change the player sees in the whole Final Fight effort. Today a Dragon fight can print "Smolderfang's health points are now -2."

- **The rule:** the Strike rule stops the defender's Health Points at 0, whichever side the defender is.
- **Endings:** a Monster at 0 Health Points is still defeated, and the victory ending is unchanged.

**Blocked by:** 03 (One Strike rule for both sides).

**Status:** done

- [x] After a Strike that deals more damage than the Monster has left, the Monster's Health Points are 0 and the "health points are now" line shows 0, in all four languages.
- [x] The player's Health Points still stop at 0, as before.
- [x] The one expected Dragon line "health points are now -2" becomes 0. No other expected text changes.
- [x] A new test through `FinalFight.Run` shows a Monster's Health Points stopping at 0 for both the Dragon and the Goblin.
- [x] All tests pass.
