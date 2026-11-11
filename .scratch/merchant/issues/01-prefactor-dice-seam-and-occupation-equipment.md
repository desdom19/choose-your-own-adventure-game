# 01: Prefactor: test dice seam and one place for each occupation's equipment

**What to build:** This change prepares the code for the Merchant feature without changing how the game plays (spec: `.scratch/merchant/spec.md`).

- Tests can control dice results. The Die's roll can be overridden, and a Player can be given its own Die. When no Die is given, a Player creates a normal one as it does today.
- Each occupation's starting weapon (type, max damage, ASCII art) is defined in one place. Later tickets add each occupation's Magical Weapon and Armor there.
- To the player, the game looks and plays exactly the same.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

- [x] The Die's roll method can be overridden. A normal Die still returns 1 to the number of sides.
- [x] A Player can be created with a given Die. A Player created without one behaves exactly as today.
- [x] The test project has a reusable scripted test Die that returns preset rolls in order.
- [x] At least one test uses the scripted Die to check an exact result. For example, a Player attack with forced rolls deals a known amount of damage to the dragon.
- [x] Each occupation's starting weapon (long sword 12, lightning bolt spell 12, dagger 6, long bow 8, with their current ASCII art) comes from a single definition per occupation.
- [x] A test checks that each of the four occupations gets the correct starting weapon from character creation, or from the definition it uses.
- [x] Game behavior, text and the existing tests are unchanged, and all tests pass.
