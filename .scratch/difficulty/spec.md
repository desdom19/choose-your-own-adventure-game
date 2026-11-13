# Spec: Difficulty setting

Status: ready-for-agent

## Problem Statement

Every Final Fight is balanced the same way for every player. The Dragon and the Goblin roll their stats from fixed dice, so a new player who wants a gentler first adventure and a returning player who wants a real challenge get the same Monsters. There's no way to make the game easier or harder without changing the code.

## Solution

When a player starts a new adventure, the game asks them to choose a **Difficulty**: Easy, Normal or Hard. Difficulty changes only how strong the **Monsters** are. On Easy, every Monster stat is 3 lower than it rolled. On Hard, every stat is 3 higher. On Normal, nothing changes, so the game plays and prints exactly as it does today from that point on. Some limits keep every Monster able to fight and be fought.

The player sees the effect in the Monster stats the game already shows. The choice is fixed for the whole adventure, and it doesn't change character creation, Gold, the Merchant, Armor or Magical Weapons. All new text is in English, Spanish, French and Italian.

## User Stories

### Choosing a Difficulty

1. As a player, I want to choose a Difficulty when I start a new adventure, so that the challenge suits me.
2. As a player, I want to choose between Easy, Normal and Hard, so that the options are simple and familiar.
3. As a player, I want the Difficulty prompt right after I choose to start an adventure and before I pick my race, so that I set the challenge before I invest in a character.
4. As a player, I want to choose by number (1 Easy, 2 Normal, 3 Hard), so that the prompt works the same in every language and doesn't clash with letter commands like `e` for exit or `n` for north.
5. As a player, I want the number I type to ignore surrounding spaces, so that ` 2 ` works like `2`.
6. As a player, I want an invalid choice to show the "Invalid choice" message and the prompt again, so that a typo doesn't pick a Difficulty for me.
7. As a player, I want pressing Enter with nothing typed to ask me again, so that I always pick a Difficulty on purpose rather than getting a hidden default.
8. As a player, I want one line confirming the Difficulty I picked, so that I know my choice was taken.
9. As a player, I want the game to stop cleanly if input runs out at the Difficulty prompt, so that it never loops forever, just like the other prompts.
10. As a player who chooses Exit at the main menu, I don't want to be asked for a Difficulty, so that leaving stays quick.

### What Difficulty changes

11. As a player on Easy, I want every Monster's Strength to be 3 lower, so that it hits me less often.
12. As a player on Easy, I want every Monster's Agility to be 3 lower, so that it dodges my attacks less often.
13. As a player on Easy, I want every Monster's Health Points to be 3 lower, so that I can defeat it faster.
14. As a player on Easy, I want every Monster's weapon maximum damage to be 3 lower, so that its hits hurt less.
15. As a player on Hard, I want every Monster's Strength, Agility, Health Points and weapon maximum damage to be 3 higher, so that the Final Fight is a real challenge.
16. As a player on Normal, I want Monsters exactly as they are today, so that the standard game is unchanged.
17. As a player, I want the Dragon and the Goblin to change by the same amounts, so that the Goblin stays the slippery, weaker-hitting Monster on every Difficulty.
18. As a player, I want Monster stats still rolled each game, so that no two fights are the same on any Difficulty.

### Limits

19. As a player on Easy, I want a Monster's Strength never to drop below 1, so that it can still hit me.
20. As a player on Easy, I want a Monster's Agility never to drop below 1, so that its stats stay meaningful.
21. As a player on Easy, I want a Monster's Health Points never to drop below 1, so that it doesn't count as defeated before I've hit it.
22. As a player on Easy, I want a Monster's weapon maximum damage never to drop below 1, so that its weapon still works.
23. As a player on Hard, I want a Monster's Strength and Agility never to go above 20, so that the stats shown stay on the same 1–20 scale as the d20 they're tested against.
24. As a player on Hard, I want a Monster with Agility 20 still to be hurt on a defense roll of 20, so that the existing "a 20 always gets through" rule still holds.

### Seeing the effect

25. As a player, I want the Monster stats shown at the encounter to be the values after Difficulty, so that I judge the risk on what I'll actually face.
26. As a player, I want the combat stats at the start of a Final Fight to show the same values after Difficulty, so that the numbers match the fight.
27. As a player, I want only one value per stat, with no "base" and "modified" breakdown, so that the stats stay easy to read.
28. As a player, I don't want Difficulty mentioned anywhere after the confirmation line, so that the rest of the game reads as it does today.

### What Difficulty doesn't change

29. As a player, I want character creation to work the same on every Difficulty, so that my character is rolled fairly.
30. As a player, I want my Gold rolled the same way on every Difficulty, so that the Merchant is equally open to me.
31. As a player, I want the Merchant's prices and stock to be the same on every Difficulty, so that shopping works as I know it.
32. As a player, I want Armor Protection and Magical Weapon damage the same on every Difficulty, so that what I buy is worth the same.
33. As a player, I want the combat rules (hit, dodge, damage, retreat) the same on every Difficulty, so that I don't have to learn new rules.
34. As a player, I want my Difficulty fixed for the whole adventure, so that the challenge I chose is the one I get.

### Languages

35. As a Spanish, French or Italian player, I want the Difficulty prompt, its options and the confirmation line in my language, so that the new step doesn't switch to English.
36. As a player in any language, I never want to see a raw message key such as `[difficulty_prompt]`, so that the game always looks finished.

### Maintainers

37. As a maintainer, I want the Difficulty amounts and limits defined in one place, so that rebalancing means changing one file.
38. As a maintainer, I want every Difficulty to roll the same number of dice, so that scripted-dice tests line up the same way on every Difficulty.
39. As a maintainer, I want Normal to be the default when no Difficulty is given, so that existing code and tests that build Monsters keep working unchanged.
40. As a maintainer, I want a new Monster to get Difficulty for free, so that future Final Fights don't need their own Difficulty code.

## Implementation Decisions

### Domain terms

Use **Difficulty**, with the values **Easy**, **Normal** and **Hard**, as defined in `GLOSSARY.md`. Don't use "mode", "level" or "setting" for it in code, comments or player-facing text. Keep using **Monster**, **Final Fight**, **Gold**, **Merchant**, **Armor**, **Protection** and **Magical Weapon**.

### Numbers

These are tuning values. Keep them as named constants in one place.

| Difficulty | Change to every Monster stat |
|---|---|
| Easy | −3 |
| Normal | 0 |
| Hard | +3 |

The change applies to Strength, Agility, Health Points and weapon maximum damage. It's applied after the stats are rolled, and then the limits apply:

| Stat | Limit |
|---|---|
| Strength | 1–20 |
| Agility | 1–20 |
| Health Points | at least 1 |
| Weapon maximum damage | at least 1 |

The limits only matter when a Difficulty pushes a stat outside its range. Normal never reaches them with today's dice.

Roughly what this does:

| | Easy | Normal | Hard |
|---|---|---|---|
| Chance a Monster's attack hits | ~39% | ~53% | ~66% |
| Average Monster Health Points | ~7.8 | 10.5 | 13.5 |
| Dragon claws max damage | 9 | 12 | 15 |
| Goblin rusty cleaver max damage | 6 | 9 | 12 |
| Goblin Agility range | 3–17 | 6–20 | 9–20 |

On Easy, about 1 Monster in 5 starts with 1 Health Point (d20 rolls of 1–4).

### Modules

- **Difficulty (new):** a small type with the three values Easy, Normal and Hard. It's the one place that holds the amount for each Difficulty and the stat limits, and it's what applies them to a Monster's rolled stats. It does no console I/O and rolls no dice.
- **Monster, Dragon and Goblin (changed):**
  - Each Monster's constructor takes an optional Difficulty, defaulting to Normal.
  - The Monster rolls its stats exactly as today, then the Difficulty is applied. Difficulty never adds, removes or changes a die roll.
  - The shared logic belongs in Monster, so the Dragon and the Goblin only roll their stats and don't each repeat Difficulty code.
  - The weapon's maximum damage is changed on the Monster's own weapon, which isn't shared with anything else.
  - The stats display and combat stats already read the Monster's current values, so they show the values after Difficulty without changes.
  - The comment on the Goblin saying its Agility is never below 6 must be reworded, because on Easy it can be as low as 3. It's still never below 6 on Normal.
- **Game (changed):**
  - After the player chooses "1" at the main menu, and before character creation starts, the game asks for a Difficulty.
  - The prompt lists `1. Easy`, `2. Normal`, `3. Hard` in the player's language and reads a trimmed answer. `1`, `2` and `3` pick a Difficulty. Anything else, including empty input, prints the existing "Invalid choice" message and asks again.
  - Reading the answer uses the same console input helper as the other prompts, so the game stops when input runs out.
  - After a valid choice, one confirmation line names the Difficulty in the player's language.
  - The Dragon and the Goblin are built after the Difficulty is chosen, inside the new-adventure branch, with that Difficulty. Today they're built before the language prompt. They still use the game's one real Die.
  - Nothing else in the game flow changes: the main menu, character creation, the Crossroads, the Merchant, the encounter and combat.
- **Messages and language data (changed):** new keys in all four languages for:
  - the Difficulty prompt
  - the three Difficulty names
  - the confirmation line, with the chosen Difficulty's name as a placeholder

  Each language keeps the same placeholders as English. New text is ASCII only, in the same style as the existing Spanish, French and Italian text (ADR 0001).

### Unchanged

- From the confirmation line onwards, a Normal adventure prints exactly what it prints today and uses the same dice in the same order.
- Player, character creation, Gold, the Merchant, Armor, Protection, Magical Weapons, Combat and the hit, dodge and damage rules don't change.

## Testing Decisions

### What makes a good test

- Tests check behavior a player or caller can see: a Monster's effective stats, exact printed text, and where a prompt appears. They don't check how Difficulty is stored or how the code is split internally.
- Use the two existing seams. Don't add a new one.
  - **The `Die` seam**, using `ScriptedDie`, at the Dragon and Goblin constructors. The constructors also take a Difficulty.
  - **The console at `Game.StartGame`**, for the prompt.
- Passing a `Die` into `Game` was rejected in the Goblin spec and stays rejected. Tests through `StartGame` therefore can't pin exact Monster stats, and they don't try to.

### Monster stats (the `Die` seam)

For both the Dragon and the Goblin, with scripted rolls:

- Normal gives exactly the rolled stats and today's weapon maximum damage (Dragon 12, Goblin 9).
- Easy gives the rolled stats minus 3 and weapon maximum damage minus 3.
- Hard gives the rolled stats plus 3 and weapon maximum damage plus 3.
- Every limit is reached at least once:
  - Easy with low rolls gives Strength 1, Agility 1 and Health Points 1.
  - Hard with high rolls gives Strength 20 and Agility 20.
  - Health Points have no upper limit, so a Hard roll of 20 gives 23.
- Building a Monster without a Difficulty gives the same result as Normal.
- Each Difficulty uses the same number of scripted rolls.
- One full `Combat` run on a non-Normal Difficulty, so the effective stats show up in the combat stats display and are the ones used in the fight.

### The prompt (the console at `StartGame`)

- The Difficulty prompt appears after the main menu choice "1" and before the race prompt.
- `1`, `2` and `3` are each accepted, and the matching confirmation line appears.
- Invalid input and empty input show "Invalid choice" and the prompt again.
- When input runs out at the Difficulty prompt, the game stops (`EndOfStreamException`), as at the other prompts.
- Choosing Exit at the main menu never shows the Difficulty prompt.

### Existing tests

- Every existing test must keep passing with its expected text unchanged. They're the regression check that Normal is today's game.
- Only two input scripts change, each gaining one line for the Difficulty choice (`2` for Normal) after the main menu "1":
  - the Crossroads west-path test
  - the "input ends at the path prompt" test
- The existing Monster, combat and Final Fight output tests build Monsters without a Difficulty, so they get Normal and need no change.

### Languages

The existing parity test already checks that Spanish, French and Italian have every English key with the same placeholders. It covers the new keys and needs no change.

### Prior art

- **Monster stats:** the Dragon and Goblin tests, which build Monsters from `ScriptedDie` and check exact stats.
- **Combat with exact output:** the Dragon and Goblin Final Fight output tests.
- **`StartGame` prompts:** the Crossroads test and the input-ended tests, which feed the console and capture its output.

## Out of Scope

- Changing Gold, Merchant prices or Merchant stock by Difficulty. This is a possible follow-up if Hard doesn't feel hard enough.
- Changing the player's stats, character creation, Armor Protection or Magical Weapon damage by Difficulty.
- Changing the combat rules (hit, dodge, damage, retreat) by Difficulty.
- Different amounts for different Monsters or different stats.
- Changing the Difficulty during an adventure, or remembering it between runs of the program.
- Showing the Difficulty in the character summary, combat stats or endings.
- Letter shortcuts or typed names (`easy`, `hard`) at the Difficulty prompt.
- Passing a `Die` into `Game`.
- Restoring accents or setting the console encoding (an ADR 0001 follow-up).

## Further Notes

- **No ADR is needed.** The amounts and limits are tuning, the change is additive, and it's easy to undo. That matches how the Merchant spec handled prices.
- **Why the Health Points limit matters.** It's about correctness, not just balance. Combat checks a Monster's Health Points after every player attack, hit or miss. A Monster starting at 0 would count as defeated after the player's first attack, even a miss.
- **Why Difficulty never changes the dice.** Today's tests script exact roll sequences. A Difficulty that changed the number of rolls, or a die's sides, would break that and make Normal hard to keep identical.
- **Design considered and rejected:** Difficulty also changing starting Gold and Merchant prices. It would only affect players who go south, it would undo the Merchant's tuning (how many players can afford each item), and it would spread Difficulty across Player, Merchant and Monster.
- **Planned tickets:** two tickets, each with its own tests:
  1. The Difficulty type and its use in every Monster, with Normal as the default and no change to any existing output.
  2. The Difficulty prompt in `StartGame`, the confirmation line, the new text in four languages, building the Monsters after the choice, and the two updated input scripts.

  Ticket 2 depends on 1.
