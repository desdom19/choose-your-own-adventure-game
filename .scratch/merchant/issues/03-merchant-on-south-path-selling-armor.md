# 03: Merchant on the south path, selling Armor

**What to build:** Going south from the **Crossroads** now leads to the **Merchant** (spec: `.scratch/merchant/spec.md`).

- **The scene:** a short story passage replaces the old "nothing happens, you return home bored" text. The Merchant shows the player's current Gold and a numbered menu: `1` Armor (70 Gold) and `3` Leave. Number `2` is kept for the Magical Weapon (ticket 05) and counts as invalid until then.
- **Buying:** the player can buy their occupation's **Armor**:

  | Occupation | Armor | Protection |
  |---|---|---|
  | Fighter | enchanted plate armor | 3 |
  | Magician | warded robe | 3 |
  | Thief | shadow leathers | 3 |
  | Archer | elven cloak | 3 |

- **Leaving:** the player goes back to the Crossroads with a short line of text, and can go south again or go north to the dragon.
- **Display:** the character summary shows Armor and Protection once the player owns Armor.
- **Not in this ticket:** Armor doesn't reduce damage yet; ticket 04 adds that.
- All new text is in all four languages.

**Blocked by:** 02 (Gold at character creation).

**Status:** ready-for-agent

- [ ] There's an Armor type with a name and a Protection value. Each occupation's Armor is added to that occupation's single equipment definition from ticket 01.
- [ ] A newly constructed Player has no Armor.
- [ ] Going south shows the new south-path text, runs a Merchant visit, then shows the back-at-the-Crossroads line and the path prompt again. The old south-path text key is replaced and not left unused.
- [ ] A Merchant visit takes a Player and Messages, and reads and writes the console. Inside, purchase logic with no console I/O returns bought, not enough Gold, or already owned. Gold and equipment change only on "bought".
- [ ] The Merchant keeps no state between visits. Whether Armor is for sale depends only on whether the Player already has Armor.
- [ ] Visit tests (scripted console input, captured output) cover each of these:
  - [ ] Buying Armor takes 70 Gold and gives the right Armor with Protection 3, for each of the four occupations.
  - [ ] With 69 Gold, buying Armor is refused with a "not enough Gold" message. Gold and equipment don't change, and the player stays in the shop until they choose Leave.
  - [ ] Picking Armor when the player already owns it shows an "already have that" message, and Gold doesn't change.
  - [ ] Invalid input, including `2` for now, shows the existing "Invalid choice" message and the menu again.
  - [ ] Leaving without buying changes nothing.
  - [ ] On a later visit, Armor isn't offered once the player owns it.
  - [ ] A player with 10 Gold still sees the full Merchant scene and can leave.
- [ ] The character summary shows a translated Armor name and Protection only when Armor is present. Without Armor, no empty Armor lines appear.
- [ ] All four Armor names have display translations in all four languages.
- [ ] Every new key (south-path text, greeting, Gold shown, menu, not enough Gold, already have that, purchase confirmation, leaving, summary labels) exists in all four languages with the same placeholders as English, checked by the parity test.
- [ ] The Merchant menu takes numbered input, so it works the same in every language.
- [ ] All existing tests pass.
