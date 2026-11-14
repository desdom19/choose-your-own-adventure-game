# Spec: Make the Final Fight one module

Status: ready-for-agent

## Problem Statement

The Final Fight, from the Monster's path narrative to the ending text, has no single owner in the code. It's split across five places:

- `Game`'s private encounter method shows the narrative, the red intro, the Monster's stats and the attack-or-retreat prompt.
- `Combat` runs the Rounds.
- `Player` and `Monster` each have their own copy of a Strike.
- `Game`'s end-game method prints the ending.

This causes four problems for a maintainer:

- **It's hard to follow.** Answering "what happens when the player retreats mid-fight?" means tracing `Game` → `Combat` → back to `Game`. The answer comes back as a `bool` plus a separate retreat flag, and a code comment is needed to explain how to read them together.
- **The rules drift.** The Strike exists twice, once on `Player` and once on `Monster`, and the copies already differ. The player's Health Points stop at 0, but a Monster's can go negative, and the player sees "Smolderfang's health points are now -2." Only one copy checks for missing weapon art. Each new feature (the Goblin, Difficulty, the defense-roll-of-20 rule, Armor) has meant editing three or four of these modules.
- **Rolls are split across two dice.** The player's Strike rolls on the player's own `Die`, but picks the Monster's taunt with the Monster's `Die`. So the order of rolls in a fight is spread across two objects.
- **The tests work around the structure.** Three test files call `Game`'s private encounter method by reflection. Others copy `Game`'s logic by hand (create `Combat`, start it, then call end-game with the retreat flag), so a change to that logic in `Game` wouldn't be caught. `Combat` exposes public lowercase fields and one-line methods that only exist for tests.

## Solution

A single **Final Fight** module owns the whole encounter, from "you see the Monster" to "the adventure ends". That covers the path narrative, the intro, the stats, the attack-or-retreat choice, every Round, both sides' Strikes, the taunts and replies, and the ending text. It takes the player, the Monster, the messages and one `Die`, and returns one outcome: **Won**, **Lost** or **Retreated**.

`Game` keeps only the Crossroads. It picks the Monster for the path, runs the Final Fight and ends the adventure. `Combat` is deleted.

The player sees no change. Every line the game prints stays exactly the same in every language, until a final, separate change makes a Monster's Health Points stop at 0 like the player's.

## User Stories

### Players: nothing changes

1. As a player, I want the Dragon fight to print exactly the same text as before in every language, so that the refactor doesn't change the game I know.
2. As a player, I want the Goblin fight to print exactly the same text as before in every language, so that the refactor doesn't change the game I know.
3. As a player, I want the path narrative, the Monster's red intro and its stats to appear in the same order as before, so that meeting a Monster feels the same.
4. As a player, I want the attack-or-retreat prompt to keep accepting `a`/`attack` and `r`/`retreat` and to ask again after an invalid answer, so that the controls don't change.
5. As a player, I want retreating before the fight to keep showing the retreat ending ("You come home poor but alive… The End."), so that this ending stays the same.
6. As a player, I want retreating mid-fight to keep showing the mid-fight retreat text ("You hastily retreat licking your wounds."), so that this message stays the same.
7. As a player, I want the combat stats display for both sides to look the same as before, so that I can read the fight the same way.
8. As a player, I want my Armor's Protection to keep reducing a Monster's damage exactly as before, so that what I bought from the Merchant still pays off.
9. As a player, I want my Magical Weapon to keep being the weapon I strike with, so that what I bought from the Merchant still pays off.
10. As a player, I want a defense roll of 20 to keep always getting through, for both sides, so that the fight rules don't change.
11. As a player, I want Monster taunts and damage replies to keep varying and to keep mentioning my race where they did before, so that the fight keeps its personality.
12. As a player, I want the Monster-specific victory and defeat endings to stay the same, so that the ending still matches the Monster I fought.
13. As a player, I want every Final Fight to still end the adventure, whether I win, lose or retreat, so that it's still a true Final Fight.
14. As a player, I want Difficulty to keep changing Monster stats exactly as before, so that the difficulty I chose still means the same thing.

### Players: the one deliberate change

15. As a player, I want a Monster's Health Points never to show below 0, just like mine, so that the numbers shown make sense.

### Maintainers

16. As a maintainer, I want one module that owns the Final Fight from the path narrative to the ending, so that I can read the whole fight in one place.
17. As a maintainer, I want the Final Fight to return one outcome (Won, Lost or Retreated), so that I don't have to combine a `bool` with a separate retreat flag.
18. As a maintainer, I want `Game` to only pick the Monster and run the Final Fight, so that the Crossroads code stays about the Crossroads.
19. As a maintainer, I want one Strike rule shared by the player and the Monster, so that a rule such as the defense-roll-of-20 rule, Protection or stopping Health Points at 0 is written once and can't drift.
20. As a maintainer, I want every roll in a Final Fight to come from one `Die`, so that the order of rolls is in one place and a scripted roll list reads in fight order.
21. As a maintainer, I want the fight's stats displays, taunts and damage replies to live in the Final Fight, so that a Monster only describes itself and the Final Fight decides what to show and when.
22. As a maintainer, I want `Combat` deleted, so that there's no shallow module between `Game` and the fight's rules.
23. As a maintainer, I want the unused retreat branch in the end-game code deleted, so that it no longer looks like there's a third way to retreat.
24. As a maintainer, I want the term "Final Fight" used in the code, and the glossary terms **Round** and **Strike** used for its parts, so that the code and the glossary agree.
25. As a maintainer, I want a future Monster to get the whole Final Fight for free once it has its stats, weapon and text, so that new Monsters stay cheap to add.
26. As a maintainer, I want a future Final Fight rule to need a change in only one module, so that features like Armor don't spread across four files.

### Tests

27. As a maintainer, I want tests to run a Final Fight through the same single call that `Game` uses, so that the tests cross the real seam.
28. As a maintainer, I want no test to call a private method by reflection, so that tests don't break when internals move.
29. As a maintainer, I want no test to copy `Game`'s fight logic by hand, so that a bug in how the fight is run gets caught.
30. As a maintainer, I want tests to be able to assert on the outcome (Won, Lost, Retreated) directly, so that a rule test doesn't depend on comparing text.
31. As a maintainer, I want the existing full-text output tests to keep passing with unchanged expected strings through the refactor, so that they prove the refactor didn't change any text.
32. As a maintainer, I want Strike behavior (hit, miss, dodge, damage, Armor absorption, defeat) tested through short scripted Final Fights, so that these tests survive the two Strike copies being merged.
33. As a maintainer, I want the only expected-text change in the whole effort to be the "-2" health line becoming "0", in its own change, so that it's clearly a deliberate change in what the player sees.

## Implementation Decisions

### The Final Fight module

- A new module named `FinalFight` (the glossary term "Final Fight") owns the whole encounter. It absorbs:
  - `Game`'s private encounter method (path narrative, red intro, stats intro and Monster stats, and the attack-or-retreat prompt with its re-ask on invalid input);
  - `Combat`'s Round loop and its combat stats display for both sides;
  - `Game`'s end-game method (victory and defeat text);
  - the player's and the Monster's attack methods, along with their roll helpers (attack roll, whether the attack hits, damage roll);
  - the Monster's own stats display and its random taunt and damage-reply picks.
- **Interface:** one static call, `FinalFight.Run(player, monster, messages, die)`, which returns a three-way outcome: Won, Lost or Retreated. This follows the same pattern as `Merchant.Visit(player, messages)`, the other place the player goes from the Crossroads.
- The outcome doesn't distinguish a retreat before the fight from one during it. The Final Fight picks the right retreat text itself.
- `Game` calls `Run` with its own `Die` and ignores the outcome, because every outcome ends the adventure. The outcome is there for tests and future callers.

### What stays the same

- **Output:** the Final Fight still writes directly to the console, including the red intro. A seam for input and output is out of scope.
- **Input:** player choices are still read through the existing console input helper, which throws when input ends.
- **Dice:** one `Die`, passed in by `Game`, makes every roll in the fight: both sides' attack, defense and damage rolls, and the taunt and reply picks. `Player` keeps its own `Die` only for character creation and Gold. Tests already pass a single `ScriptedDie` to both sides, so the existing scripted roll lists keep their order.
- **Monster:** it keeps its stats, its weapon, Difficulty's adjustments and all its text keys (path narrative, intro, stats intro and headers, attack intro, defeated narrative, taunt and reply key lists, victory and defeat). The Dragon and the Goblin stay as they are.
- **Retreat:** both retreat texts stay. `retreat` is printed when the player retreats at the encounter prompt, and `retreat_combat` when the player retreats between Rounds. The end-game code's retreat branch, which only tests reach, is deleted.
- **No-weapon message:** the "player has no weapon" message is kept as it is, even though the game can't currently reach it.
- **Player stats display:** the player's own stats display, used in character creation, doesn't change.

### Strike

- After the code has moved, the two Strike copies are merged into one Strike rule, private to the Final Fight. In order: the attack roll ≤ the attacker's Strength hits; the defender dodges on a defense roll ≤ their Agility, except that a roll of 20 never dodges; then the damage roll against the weapon's maximum damage, reduced by the defender's Protection (a Monster's Protection is 0).
- Each side keeps its own text. The player's Strike shows the Monster's attack intro and weapon art, a taunt with the player's race on a dodge, and a damage reply or the Monster's defeated narrative. The Monster's Strike shows weapon art only when it has some, Armor absorption text, and the player's defeated narrative.
- The merge must not change any output. So after the merge, a Monster's Health Points can still go below 0, and only the player's stop at 0.

### Monster Health Points stop at 0

- After the merge, as a separate change, a Monster's Health Points stop at 0 the same way the player's do. This is the only change the player can see in the whole effort.

### Delivery order

1. Create `FinalFight`, moving the code in unchanged. Switch to one `Die` and retarget the tests. Delete `Combat`, the reflection calls in the tests, and the end-game retreat branch.
2. Merge the two Strike copies into one Strike rule, with no output change.
3. Make a Monster's Health Points stop at 0, and update the one expected "-2" line.

## Testing Decisions

### What makes a good test

- Tests check what a caller or player can see: the outcome `Run` returns, the exact printed text, and Health Points after the fight. They don't check how the Final Fight is arranged inside.
- **The interface is the test surface.** Every Final Fight test goes through `FinalFight.Run`, the same call `Game` uses. The Strike rule stays private and isn't exposed to tests through `internal` or reflection.
- Seams used are all existing ones, and no new seam is added:
  - **`FinalFight.Run`** as the single entry point.
  - **The `Die` seam**, with `ScriptedDie`, for exact rolls.
  - **The console**, with `Console.SetIn` to script the player's choices and `Console.SetOut` to capture output.

### Retargeting existing tests

- **The three files that call the private encounter method by reflection** (the Dragon tests and both Final Fight output tests) call `FinalFight.Run` instead. Their expected strings don't change.
- **Tests that copy `Game`'s fight logic by hand** (create `Combat`, start it, call end-game with the retreat flag) call `FinalFight.Run` instead. `Run` now also prints the path narrative, intro and stats, so these tests feed `a` at the encounter prompt. They keep their existing expected text for the part of the output they cover.
- **The ~45 direct calls to the player's or the Monster's attack methods, or to `Combat`'s methods** (in the Dragon, Goblin, Combat and Player tests) become short scripted Final Fights through `Run`. An example is "a Goblin with 1 Health Point, these rolls → Won, and this exact text".
- **The two Difficulty tests that call a Monster's attack roll directly** become short Final Fights through `Run`. They check that Difficulty doesn't change the attack roll.
- The `Combat` tests are deleted or rewritten as Final Fight tests. Tests that only checked `Combat`'s public fields or its retreat flag's default value are deleted, since that interface no longer exists.
- The end-game tests that only checked nothing throws are replaced by Final Fight tests that assert each ending's exact text and the matching outcome.

### New coverage

- `Run` returns Won, Lost and Retreated, including a retreat at the encounter prompt (which rolls no dice) and a retreat between Rounds.
- An invalid answer at the encounter prompt and between Rounds asks again.
- Each ending's text matches its outcome, for both Monsters.
- Every roll comes from the one `Die` passed to `Run`. A scripted roll list drives the whole fight, including taunt and reply picks.

### The safety net

- After steps 1 and 2, the full-text output tests (the Dragon and Goblin Final Fight output tests, and the Difficulty full-combat test) must pass with their expected strings unchanged in all four languages. Only how a test starts the fight may change.
- In step 3, the only expected-text change allowed is the Dragon fight's "health points are now -2" line becoming 0, plus a new test that a Monster's Health Points never go below 0.

### Prior art

- The Dragon and Goblin Final Fight output tests: exact multi-language output with `ScriptedDie` and console capture.
- The Crossroads tests: scripting console input.
- The input-ended tests: input ending mid-fight still stops the game.

## Out of Scope

- A seam for console input and output, the duplicated "ask until the answer is valid" loops, and the unused `Game` path, menu and combat choice validators and their tests. These belong to a separate input/output candidate.
- Turning Monsters into data and deriving their message keys. This is a separate candidate. The Dragon and the Goblin stay as they are.
- Changing how messages are loaded, or the language parity test.
- Character creation, Gold, the Merchant and the Crossroads routing, beyond `Game` calling `Run`.
- Any change to the Final Fight's text, apart from a Monster's Health Points stopping at 0.
- The missing "The End" after a mid-fight retreat, and Spanish and French `retreat` text not matching English. These are recorded as a separate issue.
- Deleting the unreachable no-weapon message.
- Any change to the rules: hit, dodge, damage, Protection, Difficulty or the order of Rounds.

## Further Notes

- **Domain terms:** `GLOSSARY.md` defines **Final Fight**, **Monster**, **Protection** and **Difficulty**, and now also **Round** (the player's Strike, the Monster's Strike, then the choice to attack again or retreat) and **Strike** (one side's attack roll, defense roll and damage reduced by Protection). Use these terms in names and comments, and avoid "combat", "battle" and "attack sequence".
- **ADR 0001** isn't affected. All text stays in `language_data.json` and no message keys change.
- **Retreat text drift:** in English, `retreat` ends the adventure with "The End." In Spanish and French it's a shorter "you retreated from the battle", and a mid-fight retreat has no ending line at all. This spec leaves it alone on purpose, and a separate issue covers it.
- **Tickets:** four tickets in `issues/`, each with its own tests. Delivery step 1 is split in two: 01 moves the encounter, the Rounds and the ending, and 02 moves the Strikes and switches to one `Die`. Then 03 merges the Strikes and 04 makes a Monster's Health Points stop at 0. Each ticket depends on the one before it.
- **Follow-on candidates** from the same architecture review: one input/output seam with a console adapter and a scripted adapter (which would let the Final Fight stop writing to the console directly), and Monsters as data.
