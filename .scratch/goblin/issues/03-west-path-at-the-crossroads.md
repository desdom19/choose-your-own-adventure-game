# 03: West path at the Crossroads

**What to build:** Going west from the **Crossroads** leads to the Goblin's **Final Fight** (spec: `.scratch/goblin/spec.md`).

- **Choosing west:** `w` or `west`, ignoring case and surrounding spaces. `e` still means exit, and `east` is still invalid.
- **Crossroads text:** the prompt describes three paths and tells the player to enter `w` for west. The invalid-choice message lists `n`, `s`, `w` and `e`. Both are updated in all four languages.
- **Encounter:** taking the west path runs the shared encounter flow with the Goblin: west path narrative, the Goblin's intro in red, its stats intro and stats, then the attack-or-retreat prompt, which asks again on invalid input.
- **Outcomes:** attacking starts combat, and retreating shows the retreat ending. Whatever happens, the adventure ends after the encounter, as it does on the north path.
- **Preparing:** the player can visit the Merchant first and then go west with what they bought.
- **Creation:** the Goblin is created when the game starts, as the Dragon is.

**Blocked by:** 02 (The Goblin and its text).

**Status:** ready-for-agent

- [ ] Path validation accepts `w`, `west`, `W` and ` west `, still accepts `n`, `s` and `e`, and still rejects `east`. The existing test asserting `west` is invalid is reversed.
- [ ] A `StartGame` test feeds the console English, a new character, `w`, then `r`. It checks that the west path narrative, the Goblin intro and the retreat ending appear, and that the game ends.
- [ ] The Crossroads prompt and invalid-choice message mention west and `w` in all four languages, and their keys still pass the parity test.
- [ ] North still leads to the Dragon, south still leads to the Merchant and back to the Crossroads, and exit still ends the game.
- [ ] All existing tests pass.
