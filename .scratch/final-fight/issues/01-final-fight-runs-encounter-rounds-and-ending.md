# 01: The Final Fight runs the encounter, the Rounds and the ending

**What to build:** One **Final Fight** module owns the whole encounter, from the Monster's path narrative to the ending text, so a fight can be read in one place and tested through one call (spec: `.scratch/final-fight/spec.md`). For the player, nothing changes: every line the Dragon and Goblin fights print stays the same in every language.

- **Interface:** `FinalFight.Run(player, monster, messages, die)` returns one outcome: **Won**, **Lost** or **Retreated**. It's a static call, like `Merchant.Visit`. A retreat before the fight and a retreat during it both return Retreated.
- **What moves in, unchanged:**
  - The encounter from `Game`: path narrative, red intro, stats intro and the Monster's stats, and the attack-or-retreat prompt, which asks again on an invalid answer.
  - `Combat`'s Round loop and its combat stats display for both sides.
  - The victory and defeat text from `Game`'s end-game code.
- **Retreat text:** `retreat` is still printed for a retreat at the encounter prompt, and `retreat_combat` for a retreat between Rounds.
- **`Game`:** at the north and west paths, `Game` only picks the Monster and calls `Run` with its own `Die`. It ignores the outcome, because every outcome ends the adventure.
- **Deleted:**
  - `Combat`.
  - `Game`'s private encounter method and its end-game method, including the retreat branch that only tests reached.
- **Not moved yet:** each side's Strike still runs through the player's and the Monster's public attack methods. Their direct tests stay as they are until ticket 02.
- **Input and output:** the Final Fight still writes to the console, and reads choices through the existing console input helper.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

- [ ] `FinalFight.Run` exists and returns Won, Lost or Retreated, and `Game` calls it for both the north and west paths.
- [ ] `Combat` is deleted, and so are `Game`'s encounter and end-game methods.
- [ ] No test calls a private method by reflection.
- [ ] No test builds the fight by hand from `Combat` and end-game calls. Those tests call `Run` instead, typing `a` at the encounter prompt where they used to start at the combat stats.
- [ ] The Dragon and Goblin full-text output tests and the Difficulty full-combat test pass with their expected strings unchanged, in all four languages.
- [ ] New tests through `Run` cover:
  - Won;
  - Lost;
  - Retreated at the encounter prompt, with no dice rolled;
  - Retreated between Rounds;
  - an invalid answer at each prompt asks again.
- [ ] The end-game tests that only checked nothing throws are replaced by tests of each ending's exact text and its outcome.
- [ ] The tests that only checked `Combat`'s public fields or its retreat flag's default value are deleted.
- [ ] Input ending mid-fight still stops the game.
- [ ] All tests pass.
