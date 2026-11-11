# Spec: Merchant encounter on the south path

Status: ready-for-agent

## Problem Statement

When a player goes south from the Crossroads, nothing happens. The story says they got bored and went home, but the game puts them back at the Crossroads anyway, so the text and the game disagree. The south path gives no reason to take it and no way to get ready for the dragon. A character's stats are fixed once they're rolled, and how the dragon fight goes depends only on those rolls and the occupation's starting weapon.

## Solution

Going south leads the player to a **Merchant**, who sells two kinds of item for **Gold**:

- **Armor** made for the player's occupation, which gives **Protection** against each hit from the dragon.
- A **Magical Weapon** made for the player's occupation, which replaces their starting weapon and deals more maximum damage.

Gold is rolled when the character is created. Most players can afford one item, and only a lucky roll pays for both. The player can buy one item, both, or nothing, then goes back to the Crossroads, where they can head north and fight the dragon with what they bought. All new text is in English, Spanish, French and Italian.

## User Stories

### Gold

1. As a player, I want to roll for Gold during character creation, so that my starting wealth is set by the dice like my other stats.
2. As a player, I want the Gold roll to use the same "Type 'roll'" prompt as my other stats, so that character creation feels consistent.
3. As a player, I want Gold rolled after Health Points, so that the familiar stat rolls come first.
4. As a player, I want my Gold to be between 10 and 200 in steps of 10, so that amounts are easy to read and compare with prices.
5. As a player, I want my race not to change my Gold, so that the Merchant is equally open to every race.
6. As a player, I want my Gold shown in my character summary, so that I know what I can afford before I set out.

### Reaching the Merchant

7. As a player, I want the south path to lead to a Merchant, so that going south is worth doing.
8. As a player, I want a short story passage that introduces the Merchant, so that the meeting feels like part of the adventure.
9. As a player, I want the old "nothing happens, you return home bored" text gone, so that the story no longer contradicts what the game does.
10. As a player, I want to return to the Crossroads after leaving the Merchant, so that I can then go north and use what I bought against the dragon.
11. As a player, I want a line of story text when I leave the Merchant, so that I know I'm back at the Crossroads.
12. As a player, I want to meet the Merchant every time I go south, so that I can come back if I left without buying.

### Shopping

13. As a player, I want the Merchant to show my current Gold, so that I can decide what to buy.
14. As a player, I want the Merchant to offer my occupation's Armor and Magical Weapon, each with its price, so that I know exactly what I'm buying.
15. As a player, I want to choose by number (1 Armor, 2 Magical Weapon, 3 Leave), so that the menu works the same in every language.
16. As a player, I want to buy Armor for 70 Gold, so that I take less damage from the dragon.
17. As a player, I want to buy my Magical Weapon for 90 Gold, so that I deal more damage to the dragon.
18. As a player, I want to buy both items if I have enough Gold, so that a lucky Gold roll pays off.
19. As a player, I want to keep shopping after a purchase, so that I can buy the other item without leaving and coming back.
20. As a player, I want to leave without buying anything, so that the choice is always mine.
21. As a player, I want my Gold reduced by the exact price when I buy something, so that I can trust the totals.
22. As a player, I want items I've already bought removed from the offer on later visits, so that I'm not offered something I own.
23. As a player, I want the Merchant to say there's nothing left to sell once I own both items, and send me back to the Crossroads, so that I don't get stuck in an empty shop.
24. As a player, I want to see both items and their prices even when I can't afford them, so that I know what's on offer.
25. As a player, I want a "not enough Gold" message when I pick something I can't afford, so that I understand why nothing happened.
26. As a player, I want my Gold and equipment left unchanged after a refused purchase, so that a failed attempt costs me nothing.
27. As a player, I want to stay in the shop after a refused purchase, so that I can choose something else or leave.
28. As a player, I want an "already have that" message if I pick an item I own, so that I don't pay twice.
29. As a player, I want the usual "Invalid choice" message and the menu again if I type something else, so that a typo doesn't throw me out of the shop.
30. As a player who rolled too little Gold to buy anything, I still want to meet the Merchant, so that being poor is part of the story and not a skipped scene.

### Armor

31. As a Fighter, I want enchanted plate armor, so that my Armor suits my occupation.
32. As a Magician, I want a warded robe, so that my Armor suits my occupation.
33. As a Thief, I want shadow leathers, so that my Armor suits my occupation.
34. As an Archer, I want an elven cloak, so that my Armor suits my occupation.
35. As a player of any occupation, I want my Armor to give Protection 3, so that every player gets the same value for the price.
36. As a player wearing Armor, I want 3 taken off the damage of each dragon hit that lands, so that my Armor matters in the fight.
37. As a player wearing Armor, I want damage never to go below 0, so that Armor can't heal me.
38. As a player wearing Armor, I want the combat log to say how much damage my Armor absorbed, so that I can see it working.
39. As a player wearing Armor, I want a separate message when my Armor absorbs the whole hit, so that a fully blocked hit feels like a win.
40. As a player without Armor, I want combat to work and read exactly as it does today, so that nothing changes if I skip the Merchant.
41. As a player, I want my dodge chance still to depend only on my Agility, so that Armor and Agility do different jobs.

### Magical Weapons

42. As a Fighter, I want to buy a flaming sword with max damage 16 (long sword 12 + 4), so that I hit harder.
43. As a Magician, I want to buy a chain lightning spell with max damage 16 (lightning bolt spell 12 + 4), so that I hit harder.
44. As a Thief, I want to buy a venom dagger with max damage 10 (dagger 6 + 4), so that I hit harder.
45. As an Archer, I want to buy an elven bow with max damage 12 (long bow 8 + 4), so that I hit harder.
46. As a player, I want every Magical Weapon to add the same +4, so that the strength order of occupations stays the same and the price is fair to everyone.
47. As a player, I want my Magical Weapon to have its own ASCII art, so that my attacks look different after the upgrade.
48. As a player, I want my Magical Weapon to fully replace my starting weapon, so that it's the one I attack with.
49. As a player, I want my attack messages to name my Magical Weapon, so that the story matches my equipment.

### Showing stats

50. As a player, I want my character summary to always show my Gold, so that I know what I can spend.
51. As a player who bought Armor, I want my character summary to show its name and Protection, so that I can see my equipment.
52. As a player without Armor, I don't want empty Armor lines in my character summary, so that it stays tidy.
53. As a player, I want the combat stats at the start of the dragon fight to show my Protection, so that I know my defenses before I attack.
54. As a player who bought a Magical Weapon, I want my character summary and combat stats to show its name and new maximum damage, so that I can see the upgrade.

### Languages

55. As a Spanish, French or Italian player, I want all the Merchant's text in my language, so that the new encounter doesn't switch to English.
56. As a player in any language, I want Armor and Magical Weapon names translated, so that my equipment reads naturally.
57. As a player in any language, I want the "Armor absorbed damage" message to be grammatically correct whatever my Armor is called, so that it never reads wrongly.
58. As a player in any language, I never want to see a raw message key such as `[merchant_greeting]`, so that the game always looks finished.

### Maintainers

59. As a maintainer, I want each occupation's starting weapon, Magical Weapon and Armor defined together in one place, so that they can't drift apart.
60. As a maintainer, I want tests that can force dice results, so that Protection, Gold and purchases can be checked exactly and not just as ranges.
61. As a maintainer, I want a test that fails when a new message key or placeholder is missing from any language, so that a translation mistake is caught before release.
62. As a maintainer, I want prices, the Gold roll and Protection to be easy to find and change, so that the game can be rebalanced later.

## Implementation Decisions

### Domain terms

Use the glossary terms throughout code and text: **Crossroads**, **Merchant**, **Gold**, **Armor**, **Protection**, **Magical Weapon**. Don't use "shop", "vendor", "coins", "defense" or "upgraded weapon" for these concepts.

### Numbers

These are tuning values. Keep them as named constants that are easy to find.

| Value | Setting |
|---|---|
| Gold | d20 × 10, so 10–200. Race doesn't change it. |
| Armor price | 70 Gold |
| Magical Weapon price | 90 Gold |
| Protection | 3 for every Armor |
| Magical Weapon bonus | Starting weapon max damage + 4 |

With these numbers, about 70% of rolls can afford Armor, 60% the Magical Weapon, 25% both (a roll of 16 or more) and 30% nothing.

### Equipment for each occupation

| Occupation | Starting weapon | Magical Weapon | Armor |
|---|---|---|---|
| Fighter | long sword (12) | flaming sword (16) | enchanted plate armor (Protection 3) |
| Magician | lightning bolt spell (12) | chain lightning spell (16) | warded robe (Protection 3) |
| Thief | dagger (6) | venom dagger (10) | shadow leathers (Protection 3) |
| Archer | long bow (8) | elven bow (12) | elven cloak (Protection 3) |

- Each Magical Weapon has its own ASCII art in the style of the existing weapon art.
- The Magical Weapon replaces the player's weapon object outright, so it's a new weapon and not a changed copy.
- Each occupation's starting weapon, Magical Weapon and Armor are defined in one place. Starting weapons are assigned from the same definitions they already use, so a Magical Weapon's damage can't get out of step with its starting weapon.

### Modules

- **Armor (new):** a small value type with a name and a Protection value, built the same way as the existing Weapon type.
- **Player (changed):**
  - Gains Gold and an optional Armor, which is empty by default.
  - Gains an optional constructor parameter for its Die. When none is given it creates a normal Die, so game behavior doesn't change.
  - Character creation adds a Gold roll after Health Points.
  - The character summary always shows Gold, and shows Armor name and Protection only when Armor is present.
  - A newly created Player has 0 Gold and no Armor.
- **Die (changed):** its roll method can be overridden, so tests can supply a scripted Die. Game behavior doesn't change.
- **Dragon (changed):**
  - When a hit lands and isn't dodged, the damage rolled is reduced by the player's Protection, and never goes below 0.
  - Protection is 0 when the player has no Armor.
  - With Armor, the combat log shows the damage absorbed and the damage actually taken, or a separate "absorbed the whole hit" line when the result is 0. Without Armor, the output is exactly as today.
  - Health and defeat checks use the damage actually taken.
- **Combat (changed):** the combat stats display shows the player's Protection.
- **Merchant (new):** one main entry point, a visit, which takes the Player and Messages and reads and writes the console. Inside it are two parts:
  - **Purchase logic with no console I/O:** buying Armor and buying the Magical Weapon each return a result: bought, not enough Gold, or already owned. Gold and equipment change only when the result is "bought".
  - **The menu loop:**
    - Shows the player's Gold and the items still for sale with prices, then reads `1` / `2` / `3`.
    - Prints the message for each purchase result, and repeats until the player chooses Leave or nothing is left for sale.
    - Any other input prints the existing "Invalid choice" message and asks again.
    - "Already owned" applies if a player picks a number for an item that's no longer listed.
  - The Merchant keeps no state between visits. What's for sale is worked out from the Player's equipment: Armor is "owned" if the Player has Armor, and the Magical Weapon is "owned" if the Player's weapon is their occupation's Magical Weapon.
- **Game (changed):**
  - The south branch shows the new south-path text, runs a Merchant visit, then shows the "back at the Crossroads" line and returns to the path prompt.
  - The north and exit branches don't change.
  - Path input (`n` / `s` / `e`) doesn't change.
- **Messages and language data (changed):**
  - New dictionary keys in all four languages for: Gold roll prompt, summary labels (Gold, Armor, Protection), combat-stats Protection label, south-path text, Merchant greeting, Gold shown, menu, "not enough Gold", "already have that", purchase confirmations, "nothing left to sell", leaving/back-at-Crossroads text, Armor absorbed, and Armor absorbed the whole hit.
  - The four Magical Weapon names are added to the existing weapon display map.
  - The Armor names need their own display translation, either in a new Armor display map or alongside the weapon names. Either is fine as long as all four languages cover them.
  - The old south-path text key is replaced and not left unused.

### Message wording

- The Armor-absorbed message uses a template that doesn't depend on the Armor's name, e.g. "Armor (shadow leathers): 3 damage absorbed." The generic word for Armor is translated once per language. That keeps the sentence correct whether the Armor name is singular or plural, masculine or feminine.
- Messages that include values use the existing `{0}`-style placeholders. Each language keeps exactly the same placeholders as English.
- The new text stays ASCII only, in the same style as the existing Spanish, French and Italian text (see ADR 0001).

## Testing Decisions

### What makes a good test here

- Tests check behavior the player can see: Gold, equipment, Health Points and printed messages. They don't check private fields or how methods are split internally.
- Wherever the result depends on dice, the test uses a scripted Die so it can check exact numbers, not just ranges.
- Console-driven code is tested with scripted input and captured output, and the tests restore the console afterward.

### Test seams

1. **A Merchant visit** (new seam, the main one). Give it a Player, scripted console input and Messages, then check the Player's Gold, Armor and Weapon afterward plus the key output lines.
   - Buying Armor takes 70 Gold and gives the occupation's Armor with Protection 3. Check this for all four occupations.
   - Buying the Magical Weapon takes 90 Gold and gives the occupation's Magical Weapon at starting max damage + 4. Check this for all four occupations.
   - With 160 Gold, buying both leaves 0 Gold and both items owned, and the visit ends with "nothing left to sell".
   - With 69 Gold, buying Armor is refused with "not enough Gold". With 89 Gold, the same for the Magical Weapon. Gold and equipment don't change, and the player stays in the shop until they choose Leave.
   - Picking an item already owned is refused and Gold doesn't change.
   - Invalid input shows "Invalid choice" and the menu again.
   - Leaving without buying changes nothing.
   - On a second visit, owned items aren't offered.
   - A player with 10 Gold still gets the full Merchant scene and can leave.
2. **The dragon attacking the player** (existing seam; Dragon already takes a Die). Use a scripted Die to force a hit, a failed dodge and a set damage roll.
   - With Armor: damage 5 → 2 taken, 3 → 0, 2 → 0 (never negative). The absorbed and whole-hit messages appear.
   - Without Armor: damage and output are unchanged.
   - Defeat at 0 Health Points still works with Armor.
3. **Character creation** (existing method, made testable by injecting the Die). Use scripted input and dice.
   - A d20 result of 7 gives 70 Gold, and the range is 10–200.
   - Race doesn't change Gold.
   - The character summary always shows Gold, and shows Armor name and Protection only once Armor is present.
   - A new Player has 0 Gold and no Armor.
4. **Translations** (existing seam).
   - Every new key exists in English, Spanish, French and Italian with the same placeholders as English.
   - All four Magical Weapon names and all four Armor names have display names in every language.
   - No new key appears as a raw `[key]` in any language.

### Prior art

- Console output capture with `Console.SetOut` in the dragon tests.
- Loading each language's Messages in the game and dragon tests.
- The Italian key and placeholder parity test in the Messages tests. Extend that approach to the new keys in Spanish and French.

### Not tested

The full start-game flow (Crossroads → south → Merchant → Crossroads → north) stays untested, as it is today. Check it by playing the game by hand.

### Done when

All existing tests still pass.

## Out of Scope

- Any other use for Gold: dragon treasure, selling items, other shops.
- Other bonuses for each occupation, such as poison over time or harder dodges for the dragon.
- Separate balancing for Fighter and Magician, who stay identical.
- Other Merchant stock, haggling, random prices or restocking.
- Accents and a UTF-8 console in any language (an existing follow-up in ADR 0001).
- Translating the remaining English-only inputs (`roll`, `e`).
- Tests for the full start-game flow.
- Saving progress between game sessions.

## Further Notes

- No ADR is needed. The prices, the Gold roll and Protection are tuning and easy to change, and the module changes are additive.
- Because the Merchant keeps no state between visits, removing owned items works without any extra game state. It also means a new character starts with a fresh shop.
- Dodging still depends only on Agility. Armor affects only damage from hits that land.
- The game currently ends after the dragon fight whatever the outcome, so equipment and Gold don't carry over anywhere.
