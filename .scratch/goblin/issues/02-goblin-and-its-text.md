# 02: The Goblin and its text

**What to build:** A second Monster, the **Goblin**, ready for a **Final Fight** (spec: `.scratch/goblin/spec.md`). It has a different risk profile from the Dragon: it dodges more often but hurts less. It isn't reachable from the Crossroads yet (that's 03), so this ticket is checked through tests.

- **Name:** a goblin name of the implementer's choosing (for example "Gritknuckle"), the same in every language.
- **Stats:** Strength d20, Agility 5 + d15 (always 6–20), Health Points d20.
- **Weapon:** "rusty cleaver", maximum damage 8, with its own ASCII art, translated for display like other weapons.
- **Personality:** 4 defense taunts (any may include the player's race as a placeholder) and 4 damage replies, picked at random.
- **Encounter text:** a west path narrative, an intro line naming the Goblin, a stats intro, a stats header and a combat stats header.
- **Combat text:** the player's attack intro against the Goblin, and the Goblin's defeated narrative.
- **Endings:** a Goblin victory ending and a Goblin defeat ending. Retreat uses the shared retreat text.
- **Languages:** all text in English, Spanish, French and Italian.
- **Merchant items:** Armor's Protection and a Magical Weapon work against the Goblin through the shared Monster behavior from 01.

**Blocked by:** 01 (Monster refactor, with the Dragon's output unchanged).

**Status:** ready-for-agent

- [ ] Goblin tests with the scripted Die:
  - [ ] Stats fall in their ranges. A d15 roll of 1 gives Agility 6, and a roll of 15 gives Agility 20.
  - [ ] Name, weapon type "rusty cleaver" and maximum damage 8.
  - [ ] Taunts and damage replies come from the Goblin's own lists and never the Dragon's.
  - [ ] Exact attack output for a hit, a miss and a dodge.
  - [ ] Armor absorption for a partial hit and for a whole hit, and Health Points never below 0.
- [ ] Full scripted Combat runs against a Goblin, with player choices fed through the console, end in a win, a loss and a mid-fight retreat.
- [ ] The Goblin victory and defeat endings are shown exactly, checked in all four languages with captured console output.
- [ ] Every new key exists in all four languages with the same placeholders as English, checked by the existing parity test.
- [ ] No Goblin text mentions the Dragon, and no raw `[key]` text appears in a Goblin fight.
- [ ] The Dragon's output is unchanged, and all existing tests pass.
