# 02: Gold at character creation

**What to build:** When a player creates a character, they roll for **Gold** as the last step, after Health Points. It uses the same "Type 'roll'" prompt as the other stats (spec: `.scratch/merchant/spec.md`).

- Gold is d20 × 10 (10–200), and race doesn't change it.
- The character summary always shows the player's Gold.
- All new text is in English, Spanish, French and Italian.

**Blocked by:** 01 (Prefactor: test dice seam and one place for each occupation's equipment).

**Status:** ready-for-agent

- [ ] A newly constructed Player has 0 Gold.
- [ ] Character creation asks the player to type 'roll' for Gold after the Health Points roll. Other input shows the existing "type 'roll'" reminder.
- [ ] Gold equals the d20 result × 10. A test with the scripted Die checks that a roll of 7 gives 70 Gold.
- [ ] Race doesn't change Gold. A test checks this with a Halfling, since Halflings get a Strength modifier.
- [ ] Gold is always between 10 and 200 and a multiple of 10.
- [ ] The character summary shows a translated Gold label and the amount, in all four languages.
- [ ] The Gold prompt and label keys exist in all four languages.
- [ ] The key/placeholder parity check covers new keys in Spanish and French as well as Italian, so any key missing in any language fails a test.
- [ ] The Gold formula (d20 × 10) is a named constant that's easy to find.
- [ ] All existing tests pass.
