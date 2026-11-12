# 01: Monster refactor, with the Dragon's output unchanged

**What to build:** The Dragon becomes the first **Monster**, so that a second Monster can be added later without copying combat code (spec: `.scratch/goblin/spec.md`). For the player, nothing changes: the north path, the encounter, combat and every ending read exactly as they do today, in every language.

- **Monster:** a shared concept that holds a name, Strength, Agility, Health Points and a weapon, and rolls its stats with the `Die` it's given. Combat behavior that is the same for every Monster lives here:
  - the attack roll, and whether it hits (roll ≤ Strength)
  - the player's dodge roll against their Agility
  - the damage roll against the weapon's maximum damage
  - Armor absorption, with Health Points never going below 0
  - picking a random taunt or damage reply
  - showing its stats
- **Dragon:** a Monster that supplies only what differs: its name, how its stats are rolled, its claws, and the message keys for its own text.
- **Combat and the player's attack:** `Combat` fights a Monster, and the player's attack targets a Monster. The Monster's Agility decides whether it dodges. The flow doesn't change.
- **Messages:**
  - Messages that differ only by the Monster's name, and already take it as a placeholder, become generic Monster keys: the player hits or misses, the Monster defends, the Monster's health, and the Monster attacks, hits, misses or deals damage.
  - Messages that name the dragon in their wording without a placeholder become Dragon-specific keys that the Dragon supplies: the attack intro, the defeated narrative, the stats and combat headers, the stats intro, the intro, the path narrative, taunts and replies.
- **Endings:** ending the game takes the Monster that was fought. The current victory and defeat text becomes the Dragon's. Retreat text is shared by every Monster.
- **Encounter flow:** the north path encounter (narrative, red intro, stats intro and stats, attack-or-retreat prompt) works for any Monster, so a west path can reuse it later.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

- [ ] Monster exists, the Dragon is a Monster, and Combat and the player's attack work with any Monster.
- [ ] The Dragon's stats are still rolled with a d20 for Strength, Agility and Health Points, and it still uses claws with a maximum damage of 12.
- [ ] The text the game prints for the Dragon (encounter, combat, endings) is the same, character for character, in English, Spanish, French and Italian.
- [ ] The existing Dragon, Combat, Player and Game tests pass with their expected strings unchanged. Only references to renamed message keys, members or methods are updated.
- [ ] Every renamed or new key exists in all four languages with the same placeholders as English, checked by the existing parity test.
- [ ] Endings take the Monster that was fought. Dragon victory and defeat text is unchanged, and retreat text is unchanged.
- [ ] No key is left unused, and no raw `[key]` text appears in a Dragon fight.
- [ ] All existing tests pass.
