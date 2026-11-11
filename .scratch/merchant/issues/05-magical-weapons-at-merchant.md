# 05: Magical Weapons at the Merchant

**What to build:** The Merchant now also sells each occupation's **Magical Weapon** as menu option `2`, for 90 Gold (spec: `.scratch/merchant/spec.md`).

- **The weapon:** it replaces the player's starting weapon, deals the starting max damage + 4, and has its own ASCII art.
- **The weapons for each occupation:**

  | Occupation | Starting weapon | Magical Weapon |
  |---|---|---|
  | Fighter | long sword (12) | flaming sword (16) |
  | Magician | lightning bolt spell (12) | chain lightning spell (16) |
  | Thief | dagger (6) | venom dagger (10) |
  | Archer | long bow (8) | elven bow (12) |

- **Buying both:** a player with enough Gold can buy both Armor and the Magical Weapon, and keeps shopping until they leave or nothing is left to sell. Once both are owned, the Merchant says there's nothing left and sends the player back to the Crossroads.
- **Where it shows up:** attacks, the character summary and combat stats all use the Magical Weapon's name, art and new max damage.
- All new text is in all four languages.

**Blocked by:** 03 (Merchant on the south path, selling Armor). Can be done alongside 04.

**Status:** ready-for-agent

- [ ] Each occupation's Magical Weapon is added to its single equipment definition. Its max damage is calculated as starting max damage + 4, not typed in separately.
- [ ] Each Magical Weapon has its own ASCII art, in the style of the existing weapon art.
- [ ] Buying replaces the player's weapon with a new Magical Weapon. The occupation's starting weapon definition doesn't change.
- [ ] Whether the Magical Weapon is for sale depends only on whether the player's weapon is already their occupation's Magical Weapon. The Merchant keeps no state between visits.
- [ ] Visit tests (scripted console input, captured output) cover each of these:
  - [ ] Buying the Magical Weapon takes 90 Gold and gives the right weapon at starting max damage + 4, for each of the four occupations.
  - [ ] With 89 Gold, buying is refused with "not enough Gold", and Gold and equipment don't change.
  - [ ] Picking the Magical Weapon when the player already owns it is refused with "already have that", and Gold doesn't change.
  - [ ] With 160 Gold, buying both leaves 0 Gold, both items are owned, and the visit ends with the "nothing left to sell" message and a return to the Crossroads.
  - [ ] After one purchase, the menu shows again with the item that's left until the player chooses Leave.
  - [ ] On a later visit, items the player owns aren't offered. When both are owned, the visit goes straight to "nothing left to sell".
- [ ] Option `2` is no longer treated as invalid input.
- [ ] Player attack messages, the character summary and combat stats show the Magical Weapon's translated name and new max damage.
- [ ] All four Magical Weapon names have display names in all four languages.
- [ ] The new keys (menu entry, purchase confirmation, nothing left to sell) exist in all four languages with the same placeholders as English, checked by the parity test.
- [ ] All existing tests pass.
