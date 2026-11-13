# 01: Difficulty changes Monster stats

**What to build:** Every Monster can be built with a **Difficulty**: Easy, Normal or Hard. After a Monster rolls its stats, as it does today, the Difficulty changes Strength, Agility, Health Points and weapon maximum damage by a fixed amount: Easy −3, Normal 0, Hard +3. Then the limits keep each stat usable:

- Strength 1–20
- Agility 1–20
- Health Points at least 1
- Weapon maximum damage at least 1

The amounts and limits live in one small Difficulty type, and the shared Monster logic applies them, so the Dragon and the Goblin don't each repeat Difficulty code. A Monster built without a Difficulty gets Normal, so the game and every existing test behave exactly as today. Nothing in the game chooses a Difficulty yet; ticket 02 adds the prompt.

See `.scratch/difficulty/spec.md` (Numbers, Modules, Testing Decisions → Monster stats).

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] A Difficulty type with the values Easy, Normal and Hard holds the amounts (−3 / 0 / +3) and the stat limits as named constants in one place. It does no console I/O and rolls no dice.
- [ ] The Dragon and the Goblin each take an optional Difficulty, defaulting to Normal, and apply it after rolling their stats. The shared logic lives in Monster.
- [ ] Every Difficulty uses the same number of die rolls as today. Difficulty never adds, removes or changes a roll.
- [ ] Only the Monster's own weapon gets the changed weapon maximum damage. No other weapon is affected.
- [ ] `ScriptedDie` tests, for both the Dragon and the Goblin:
  - [ ] Normal gives the rolled stats and today's weapon maximum damage (Dragon 12, Goblin 9).
  - [ ] Easy gives the rolled stats −3 and weapon maximum damage −3. Hard gives the rolled stats +3 and weapon maximum damage +3.
  - [ ] Every limit is reached at least once: Easy with low rolls gives Strength 1, Agility 1 and Health Points 1. Hard with high rolls gives Strength 20 and Agility 20. A Hard Health Points roll of 20 gives 23, because Health Points have no upper limit.
  - [ ] Building a Monster without a Difficulty gives the same result as Normal.
- [ ] One full Combat run on Hard (or Easy) with scripted dice shows the stats after Difficulty in the combat stats display and uses them in the fight.
- [ ] The Goblin comment saying its Agility is never below 6 is reworded: that's true on Normal, but on Easy its Agility can be as low as 3.
- [ ] Every existing test passes with its expected text unchanged.
- [ ] Code comments and names use **Difficulty**, never "mode", "level" or "setting".
