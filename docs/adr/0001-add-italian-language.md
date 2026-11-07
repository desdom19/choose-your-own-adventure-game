# ADR 0001: Add Italian as a fourth language

- **Status:** Accepted
- **Date:** 2026-11-06
- **Decider:** Destine Dominguez
- **Implemented in:** commit `3fad733`

This ADR was written after the change was made. The alternatives below were reconstructed from the codebase, not taken from notes made at the time.

## Context

The game is a .NET console app that offers English, Spanish and French on a menu shown at startup.

All player-facing text lives in `language_data.json`, with one top-level section per language. Each section contains a `dictionary` of messages and five maps that convert player input to canonical values and canonical values to display text. `Messages.ReadDictionary()` loads a section by name, so it has no per-language logic.

Italian was chosen because it was assigned in the CIDM 4390 homework.

## Decision

Add Italian as a fully translated language, selected with option 4 on the language menu.

- **Text:** Add an `Italian` section to `language_data.json` with the same 69 dictionary keys and the same `{0}`-style placeholders as English, plus Italian versions of all five maps.
- **Code:** Change only the language menu in `Game.cs`, which needs a new prompt, a new option and a new `"4" => "Italian"` case. `Messages.cs` and the game logic don't change.
- **Tests:** Assert the welcome message and the race, occupation and weapon translations, and check that the Italian `dictionary` matches the English keys and placeholders. Three smoke tests also call `EndGame` for each ending in Italian. They only confirm that it doesn't throw, not which text it prints. No test covers menu option 4, because `StartGame` isn't tested.

## Alternatives considered

- **Keep three languages:** This avoids a fourth set of text to maintain, but wouldn't meet the CIDM 4390 homework assignment.
- **Move to .NET `.resx` resource files:** This is the standard .NET localization approach, but it would mean migrating the existing languages and rewriting `Messages` for a small console game.
- **Split each language into its own JSON file:** This makes individual files smaller, but changes the loading code and doesn't solve a current problem.

## Consequences

### Positive

- Italian-speaking players can play the whole game in their language.
- The change confirms that the JSON design works: a new language needed only a data section and a menu edit.
- The parity test will fail if an Italian key or placeholder drifts from English. A mismatch could otherwise show a raw `[key]` or break `string.Format`.

### Negative

- **More upkeep:** Every new or changed message now needs four versions, and `language_data.json` grew by about 100 lines.
- **Missing accents:** `language_data.json` is entirely ASCII, so Italian text has spelling errors such as `agilita` (agilità), `piu` (più) and `succedera` (succederà). This matches the existing `Espanol` and `Francais` style, but final accents are required in standard Italian spelling. The app doesn't set `Console.OutputEncoding`, so adding accents would need testing in the console.
- **Partly English commands:** Italian prompts still ask players to type `roll` and `e` (for "exit"), because `Player.cs` and `Game.cs` hardcode those inputs. `a`, `r`, `n` and `s` happen to suit Italian, but full words such as `attacca` aren't accepted.
- **Masculine-only text:** Phrases such as `Benvenuto`, `Sei sconfitto` and `Annoiato` always use the masculine form, because the player has no gender attribute.
- **Hardcoded menu:** Each new language still requires editing the prompt and `switch` in `Game.cs`. The title and `Enter choice:` prompt remain English only.

## Follow-ups

These aren't part of this decision and haven't been scheduled:

- Have a fluent Italian speaker review the text. The repo has no record of such a review.
- Restore Italian accents and set the console encoding to UTF-8, ideally alongside the same fix for Spanish and French.
- Extend the parity test to Spanish and French, whose current tests check only 11 required keys.
- Make the `EndGame` tests capture console output and assert the exact ending text in every language, using the `Console.SetOut` approach from `DragonTests.cs`.
