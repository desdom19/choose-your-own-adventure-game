# Spec: Goblin encounter on the west path

Status: ready-for-agent

## Problem Statement

At the Crossroads the player can go north to fight the Dragon, go south to the Merchant, or exit. That's the only Final Fight in the game, so every adventure ends the same way. If the player types a direction such as west, the game just says "Invalid choice". The world feels smaller than the Crossroads suggests, and once a player knows the Dragon there's nothing else to face.

## Solution

Going west from the Crossroads leads to a **Goblin**, a second **Monster** with its own **Final Fight**. The fight works the same way as the Dragon fight: a story passage, the Goblin's introduction, its stats, then a choice to attack or retreat. Every outcome ends the adventure, just like the Dragon: the player wins, loses, or retreats home.

The Goblin has a different risk profile from the Dragon. It's slippery, so it dodges more often, but its weapon hurts less. The Goblin has its own name, weapon, taunts, damage replies and victory and defeat endings. Armor and Magical Weapons bought from the Merchant work against it exactly as they do against the Dragon. All new text is in English, Spanish, French and Italian.

## User Stories

### Choosing the west path

1. As a player, I want the Crossroads to offer a west path, so that I have a second Final Fight to choose.
2. As a player, I want to type `w` or `west` to go west, so that it works the same way as `n`/`north` and `s`/`south`.
3. As a player, I want the west choice to ignore case and surrounding spaces, so that `W` or ` west ` also work.
4. As a player, I want `e` to keep meaning exit, so that the controls I already know don't change.
5. As a player, I want `east` to stay an invalid choice, so that nothing unexpected happens if I type it.
6. As a player, I want the Crossroads prompt to describe three paths and tell me to enter `w` for west, so that I know the west path exists.
7. As a player, I want the invalid-choice message at the Crossroads to list `w` as well as `n`, `s` and `e`, so that it tells me every valid option.
8. As a player, I want to be able to visit the Merchant first and then go west, so that I can prepare for the Goblin just as I can for the Dragon.

### Meeting the Goblin

9. As a player, I want a short story passage when I take the west path, so that meeting the Goblin feels like part of the adventure.
10. As a player, I want the Goblin to introduce itself by name in red text, so that the moment is as dramatic as meeting the Dragon.
11. As a player, I want to see the Goblin's stats before I decide what to do, so that I can judge the risk the same way I can with the Dragon.
12. As a player, I want the Goblin's stats to show its name, Strength, Agility, Health Points, weapon and maximum damage, so that I see the same information I get for the Dragon.
13. As a player, I want to choose to attack or retreat after seeing the Goblin's stats, so that I'm never forced into the fight.
14. As a player, I want an invalid answer at the attack-or-retreat prompt to ask me again, so that a typo doesn't decide the fight for me.

### The Goblin's character

15. As a player, I want the Goblin to have its own name, so that it feels like a distinct character rather than a reskinned Dragon.
16. As a player, I want the Goblin to fight with a rusty cleaver, shown with its own ASCII art, so that its attacks look different from the Dragon's claws.
17. As a player, I want the Goblin to taunt me in its own voice when it dodges my attack, so that the fight has personality.
18. As a player, I want the Goblin to reply in its own voice when I damage it, so that hitting it feels rewarding.
19. As a player, I want the Goblin's taunts and replies to vary from hit to hit, so that the fight doesn't feel repetitive.
20. As a player, I want a Goblin taunt to be able to mention my race, like the Dragon's taunts, so that the insults feel personal.

### The Goblin's risk profile

21. As a player, I want the Goblin's stats to be rolled each game, so that no two Goblin fights are the same.
22. As a player, I want the Goblin to dodge my attacks more often than the Dragon does, so that the fight rewards persistence.
23. As a player, I want the Goblin's Agility never to be very low, so that it always feels slippery.
24. As a player, I want the Goblin's weapon to deal less maximum damage than the Dragon's, so that the Goblin is less deadly per hit.
25. As a player, I want the Goblin's Strength and Health Points to be rolled the same way as the Dragon's, so that the two Final Fights are equally fair overall.

### Fighting the Goblin

26. As a player, I want combat with the Goblin to work exactly like combat with the Dragon, so that I don't have to learn new rules.
27. As a player, I want to see both my stats and the Goblin's stats when combat starts, so that I know where I stand.
28. As a player, I want my Armor's Protection to reduce the Goblin's damage, so that buying Armor helps on the west path too.
29. As a player, I want my Magical Weapon to be the weapon I use against the Goblin, so that buying it helps on the west path too.
30. As a player, I want to choose to attack again or retreat after each round, so that I control how long I risk the fight.
31. As a player, I want my Health Points never to drop below 0, so that the numbers shown make sense.

### Ending the adventure

32. As a player, I want a Goblin-specific victory ending when I defeat it, so that the ending matches the Monster I beat.
33. As a player, I want a Goblin-specific defeat ending when it defeats me, so that the ending doesn't mention a dragon I never met.
34. As a player, I want retreating from the Goblin, before or during combat, to end the adventure with the same retreat text as the Dragon, so that retreating means the same thing everywhere.
35. As a player, I want the game to end after the Goblin fight whatever happens, so that it's a true Final Fight.

### Languages

36. As a Spanish, French or Italian player, I want all Goblin text in my language, so that the west path is as playable as the rest of the game.
37. As a Spanish, French or Italian player, I want the new Crossroads prompt and invalid-choice text in my language, so that I can find the west path.

### The Dragon stays the same

38. As a player who already knows the Dragon fight, I want every line it prints to stay exactly the same in every language, so that adding the Goblin doesn't change the Dragon.
39. As a player, I want the north and south paths to behave exactly as before, so that the Merchant and the Dragon are unaffected.

### Maintainers

40. As a maintainer, I want the Dragon and the Goblin to share one Monster concept, so that a change to combat, such as the Armor work, is made only once.
41. As a maintainer, I want adding a future Monster to need only its stats, weapon and text, so that new Final Fights are cheap to add.

## Implementation Decisions

### Monster

- Introduce **Monster** as a shared concept that the Dragon and the Goblin both are. It holds a name, Strength, Agility, Health Points and a weapon, and it rolls its stats with the `Die` it's given, so tests can script the rolls.
- Combat behavior that is the same for every Monster belongs to Monster. That covers its attack roll, whether the attack hits (roll ≤ Strength), the player's dodge roll against their Agility, the damage roll against the weapon's maximum damage, Armor absorption, health never going below 0, picking a random taunt or damage reply, and showing its stats.
- Each Monster supplies only what differs: its name, how its stats are rolled, its weapon and ASCII art, and the message keys for its own text.
- Its own text covers the path narrative, the intro line, the stats intro and the stats header. It also covers its header in the combat stats display, the player's attack intro line, its defeated narrative, its 4 defense taunts and 4 damage replies, and its victory and defeat endings.

### Message keys

- Messages that differ only by the Monster's name, and already take the name as a placeholder, become generic Monster keys. Examples: the player hits or misses, the Monster defends, the Monster's health, and the Monster attacks, hits, misses or deals damage.
- Messages that mention the Dragon in their text without a placeholder become per-Monster keys, so the Dragon keeps its exact wording. Examples: the "attack the dragon" intro, the defeated narrative, the stats and combat headers, and the stats intro.
- The existing victory and defeat text becomes the Dragon's endings, and the Goblin gets its own. The `retreat` and `retreat_combat` messages and the attack-or-retreat prompts are shared by every Monster unchanged.
- Every key exists in English, Spanish, French and Italian with the same placeholders.

### Combat and Player

- `Combat` fights a Monster rather than a Dragon. Its flow doesn't change: the player attacks first, then the Monster attacks, then the player chooses to attack again or retreat.
- The player's attack targets a Monster, and the Monster's Agility decides whether it dodges. Nothing else about the player's attack changes.
- Ending the game takes the Monster that was fought, so the right victory or defeat text is shown. Retreat text is the same for every Monster.

### Goblin

- **Name:** a goblin name of the implementer's choosing (for example "Gritknuckle"), the same in every language, like Smolderfang.
- **Strength:** d20.
- **Agility:** 5 + d15, so always 6–20.
- **Health Points:** d20.
- **Weapon:** "rusty cleaver", maximum damage 9, with its own ASCII art. Its name is translated for display like other weapons.
- **Text:** 4 defense taunts (any may include the player's race as a placeholder) and 4 damage replies, in all four languages.

### Crossroads and the encounter

- The Crossroads accepts `w` and `west` (case-insensitive, trimmed) as the west path, alongside the existing choices. `e` still means exit, and `east` is still invalid.
- The Crossroads prompt and invalid-choice message are rewritten in all four languages to describe three paths and list `w`.
- The encounter flow is shared by both Monsters: path narrative, intro in red, stats intro and stats, then the attack-or-retreat prompt, re-asking on invalid input. Attacking starts combat, and retreating shows the retreat ending.
- Taking the west path always ends the adventure after the encounter, the same as the north path.
- The Goblin is created when the game starts, as the Dragon is.

### Unchanged

- Every line the Dragon fight prints stays exactly the same in every language.
- The Merchant, character creation and the south path don't change.

## Testing Decisions

### What makes a good test

- Tests check behavior a player or caller can see: stats in range, exact printed text, Health Points after a hit, which ending appears. They don't check how classes are arranged internally.
- Use the existing seams. Don't add a new one.
  - **The `Die` seam**, using `ScriptedDie`, for all Monster and combat behavior.
  - **The console at `Game.StartGame`**, for routing at the Crossroads.
- We considered passing a `Die` into `Game` so every outcome could be tested through `StartGame`, and rejected it. Character creation, both Monsters' stat rolls and combat would share one roll script, which would break whenever an extra roll was added.

### Monster refactor

- The existing Dragon and combat tests must keep passing. Their expected strings must not change. Only references to renamed message keys, members or methods may be updated.
- These tests are the regression check that the Dragon's output is unchanged in all four languages.

### Goblin

Tests use `ScriptedDie` for exact rolls and `Console.SetOut` to capture printed output.

- Its stats fall in their ranges, and its Agility is never below 6. That includes the lowest roll (1 on the d15 gives Agility 6) and the highest (15 gives 20).
- Its name, weapon type "rusty cleaver" and maximum damage of 9.
- Taunts and damage replies come from the Goblin's own lists and never the Dragon's.
- Exact attack output for a hit, a miss and a dodge.
- Armor absorption, for both partial and whole hits, and Health Points never going below 0.
- Full `Combat` runs against a Goblin ending in a win, a loss and a mid-fight retreat.
- The Goblin's victory and defeat endings, checked exactly in all four languages.

### Crossroads

- Path validation accepts `w`, `west`, `W` and ` west `, still accepts `n`, `s` and `e`, and still rejects `east`. The existing test that asserts `west` is invalid must be reversed.
- One `StartGame` test plays: English, create a character, `w`, `r`. It checks that the west path narrative, the Goblin intro and the retreat ending appear. Retreating at the encounter prompt never rolls a die, so the real `Die` inside `Game` doesn't make the test unpredictable.

### Languages

The existing parity test already checks that Spanish, French and Italian have every English key with the same placeholders. It covers the new keys automatically and needs no change.

### Prior art

- **Monster tests:** the Dragon tests, which use scripted rolls for exact attack output and Armor absorption, and drive the encounter by feeding console input.
- **Combat tests:** the existing combat tests.
- **`StartGame` test:** character creation tests, which feed the console.
- **Path validation tests:** the existing Game tests.

## Out of Scope

- An east path, and changing the exit key.
- More than one Monster per adventure, or fighting the Goblin and then going on to the Dragon. Every Final Fight ends the adventure.
- Carrying Health Points or rewards between fights.
- Treasure or Gold rewards for beating the Goblin.
- Changing the Dragon's stats, text or behavior.
- Changing the Merchant, its prices or its stock.
- Restoring accents in Spanish, French and Italian, or setting the console encoding (an ADR 0001 follow-up).
- Translating typed commands such as `roll` or `e` (an ADR 0001 limitation).

## Further Notes

- **Domain terms:** `GLOSSARY.md` defines **Monster**, **Final Fight**, **Dragon** and **Goblin**, and its **Crossroads** entry includes west. Use these terms in code comments and new message text where they fit.
- **ADR 0001 is partly out of date.** Its follow-up asking for parity checks in Spanish and French has already been done, so no work is needed for it here.
- **Planned tickets:** three tickets, each with its own tests:
  1. The Monster refactor, with no change to the Dragon's output.
  2. The Goblin and its text.
  3. The west path at the Crossroads and the encounter.

  Ticket 3 depends on 2, and 2 depends on 1.
- **Retreat text differs by language.** The Spanish `retreat` message is worded differently from English ("Te has retirado de la batalla."). It's reused as-is for the Goblin, and this feature doesn't change it.
