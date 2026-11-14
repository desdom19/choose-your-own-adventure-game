# 02: The Strikes move into the Final Fight, with one Die

**What to build:** Both sides' **Strikes** become private parts of the Final Fight, and every roll in a fight comes from the one `Die` passed to `Run` (spec: `.scratch/final-fight/spec.md`). For the player, nothing changes.

- **What moves in, unchanged:**
  - The player's attack and the Monster's attack, as two private Strikes. They stay as two separate copies; ticket 03 merges them.
  - The roll helpers each side uses: the attack roll, whether it hits, and the damage roll.
  - The Monster's random taunt and damage-reply picks.
  - The Monster's own stats display.
- **One `Die`:** every roll in a fight comes from the `Die` given to `Run`: both sides' attack, defense and damage rolls, and the taunt and reply picks. `Player` keeps its own `Die` only for character creation and Gold.
- **What stays on Monster:** its stats, its weapon, Difficulty's adjustments and all its text keys. The Dragon and the Goblin don't change.
- **No-weapon message:** the "player has no weapon" message is kept exactly as it is.
- **What stays on Player:** its own stats display, which is used in character creation, doesn't change.

**Blocked by:** 01 (The Final Fight runs the encounter, the Rounds and the ending).

**Status:** done

- [x] The player and the Monster no longer have public attack, attack-roll, hit-check or damage-roll methods.
- [x] The Monster no longer has public taunt-pick, reply-pick or stats-display methods.
- [x] Every roll in a Final Fight comes from the `Die` passed to `Run`.
- [x] Where a test already gave a single `ScriptedDie` to both sides, its roll list doesn't change.
- [x] The direct attack tests are rewritten as short scripted Final Fights through `Run`, keeping their exact expected text. They cover:
  - hit, miss and dodge, for each side;
  - a defense roll of 20 getting through;
  - Armor absorption, for a partial hit and a whole hit;
  - each side being defeated.
- [x] The two Difficulty tests that checked a Monster's attack roll directly become Final Fights through `Run`, and still show that Difficulty doesn't change the attack roll.
- [x] The Dragon and Goblin full-text output tests and the Difficulty full-combat test pass with their expected strings unchanged, in all four languages.
- [x] Character creation still rolls stats and Gold with the player's own `Die`, and its tests pass unchanged.
- [x] All tests pass.
