# 02: The player chooses a Difficulty when starting an adventure

**What to build:** When the player chooses "1" at the main menu, the game asks them to choose a **Difficulty** before character creation starts. The prompt lists `1. Easy`, `2. Normal`, `3. Hard` in the player's language. A valid answer prints one confirmation line naming the chosen Difficulty. The Dragon and the Goblin are then built with that Difficulty, so a player can play a full Easy or Hard adventure and see the changed Monster stats at the encounter and at the start of combat. On Normal, everything from the confirmation line onwards prints and plays exactly as it does today. All new text is in English, Spanish, French and Italian.

See `.scratch/difficulty/spec.md` (Modules → Game, Messages and language data; Testing Decisions → The prompt, Existing tests).

**Blocked by:** 01 (Difficulty changes Monster stats)

**Status:** ready-for-agent

- [ ] The Difficulty prompt appears after the main menu choice "1" and before the race prompt. Choosing Exit at the main menu never shows it.
- [ ] The answer is trimmed. `1`, `2` and `3` pick Easy, Normal and Hard. Anything else, including empty input, prints the existing "Invalid choice" message and asks again.
- [ ] The answer is read through the same console input helper as the other prompts, so the game stops (`EndOfStreamException`) when input runs out at the Difficulty prompt.
- [ ] After a valid choice, one confirmation line names the chosen Difficulty in the player's language. Difficulty isn't mentioned anywhere else in the game.
- [ ] The Dragon and the Goblin are built after the Difficulty is chosen, inside the new-adventure branch, with that Difficulty. They still use the game's one real Die. No `Die` is passed into `Game`.
- [ ] There are new message keys for the prompt, the three Difficulty names and the confirmation line (with the Difficulty name as a placeholder). They're in all four languages, in ASCII only, with the same placeholders as English, and the existing parity test passes.
- [ ] No raw message key such as `[difficulty_prompt]` appears in any language.
- [ ] New `StartGame` tests with scripted console input check:
  - [ ] where the prompt appears
  - [ ] that each of `1`, `2` and `3` is accepted and shows its confirmation line
  - [ ] that invalid and empty input are asked again
  - [ ] that the game stops when input runs out at the prompt
- [ ] The Crossroads west-path test and the "input ends at the path prompt" test each gain one input line (`2`, for Normal) after the main menu "1". Their expected output doesn't change otherwise.
- [ ] Every other existing test passes unchanged.
- [ ] Player-facing text and code use **Difficulty**, never "mode", "level" or "setting".
