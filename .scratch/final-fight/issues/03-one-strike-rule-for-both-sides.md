# 03: One Strike rule for both sides

**What to build:** The player's and the Monster's **Strike** copies merge into one Strike rule inside the Final Fight, so each fight rule is written once and can't drift (spec: `.scratch/final-fight/spec.md`). For the player, nothing changes.

- **The Strike rule:**
  1. The attack roll hits if it's ≤ the attacker's Strength.
  2. The defender dodges if their defense roll is ≤ their Agility, except that a roll of 20 never dodges.
  3. The damage roll is against the weapon's maximum damage, minus the defender's Protection. A Monster's Protection is 0.
- **Rolls:** the rolls happen in the same order as today, so scripted roll lists don't change.
- **Each side keeps its own text:**
  - **The player's Strike:**
    - the Monster's attack intro, and the weapon art;
    - on a dodge, a taunt that mentions the player's race;
    - after damage, a damage reply or the Monster's defeated narrative.
  - **The Monster's Strike:**
    - the weapon art, only if it has some;
    - Armor absorption text;
    - the player's defeated narrative.
- **Health Points below 0:** the merge doesn't change any output. A Monster's Health Points can still go below 0 after this ticket, and only the player's stop at 0. Ticket 04 changes that.

**Blocked by:** 02 (The Strikes move into the Final Fight, with one Die).

**Status:** ready-for-agent

- [ ] Hitting, the dodge with the roll-of-20 rule, and damage minus Protection are each written once in the Final Fight and used by both sides.
- [ ] Each side's text, weapon-art handling and Armor absorption text are unchanged.
- [ ] The Dragon and Goblin full-text output tests, the Difficulty full-combat test and the Strike tests from ticket 02 all pass with their expected strings and roll lists unchanged, in all four languages. That includes the Dragon line "health points are now -2".
- [ ] All tests pass.
