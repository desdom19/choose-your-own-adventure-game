# 04: Armor Protection in the dragon fight

**What to build:** Armor now protects the player in the dragon fight (spec: `.scratch/merchant/spec.md`).

- **Damage:** when the dragon's attack lands and the player fails to dodge, the damage is reduced by the player's **Protection** (3 with Armor, 0 without), and never goes below 0.
- **Combat log:** shows how much damage the Armor absorbed and how much the player took. When the whole hit is absorbed, it shows a separate "absorbed the whole hit" line.
- **Combat stats:** the stats at the start of the fight show the player's Protection.
- **Without Armor:** the fight works and reads exactly as it does today.
- Dodging still depends only on Agility.

**Blocked by:** 03 (Merchant on the south path, selling Armor).

**Status:** ready-for-agent

- [ ] Damage taken equals the damage rolled minus Protection, never below 0. Health and defeat checks use the damage actually taken.
- [ ] Dragon-attack tests with the scripted Die (forced hit, failed dodge, set damage roll), for a player with Armor:
  - [ ] Damage roll 5 → 2 damage taken.
  - [ ] Damage roll 3 → 0 damage taken, and the "absorbed the whole hit" message appears.
  - [ ] Damage roll 2 → 0 damage taken, never negative, and Health Points don't go up.
- [ ] Without Armor, damage and combat log output are the same as before, checked by a test.
- [ ] A player with Armor whose Health Points reach 0 is still defeated.
- [ ] The Armor-absorbed message uses a template that doesn't depend on the Armor name, e.g. "Armor (shadow leathers): 3 damage absorbed." The generic word for Armor is translated once per language, so the sentence is correct for every Armor name in every language.
- [ ] Combat stats show a translated Protection label and value, 0 without Armor.
- [ ] The new keys (Armor absorbed, Armor absorbed the whole hit, Protection label) exist in all four languages with the same placeholders as English, checked by the parity test.
- [ ] Dodging is unchanged: it still depends only on the player's Agility.
- [ ] All existing tests pass.

## Comments

**Implementation note (commit 600e3bb):** No new key was added for the combat-stats Protection label. The combat stats already reuse the character summary's `stats_*` labels (`stats_name`, `stats_strength`, `stats_health` and so on), so the Protection line reuses the existing `stats_protection` key. That key is already translated in all four languages ("Protection: ", "Proteccion: ", "Protection: ", "Protezione: ") and is covered by the parity test. Only `armor_absorbed` and `armor_absorbed_whole_hit` are new.
