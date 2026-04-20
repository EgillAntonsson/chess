# Chess

A chess engine built in C# for Unity. The codebase demonstrates testability-first design, clean separation of concerns, and automated test infrastructure using the Unity Test Runner.

![tests pass, not all shown](~Documentation/Images/TestRunner.png)

## Architecture

### Board / ChessBoard layering
Testability was the primary motivation for this separation. `Board` is a pure static class: every method takes its inputs and returns its outputs with no side effects, so it can be unit-tested directly — no Unity runtime required, no setup cost, and no state leaking between tests. `ChessBoard` owns the mutable game state (`Tile[,]`, player dictionaries) and delegates all logic to `Board`, keeping state mutations explicit and centralised.

### Null-free domain model
The domain types are designed so that `null` does not arise in normal use. Value types (`readonly record struct`) cannot be null, record class hierarchies are constructed with required parameters, empty board positions are represented by a `Tile` (not `null`), and empty collections return an empty array instead of `null`. This eliminates null-checks from game logic and makes invalid states unrepresentable.

### Clone-on-write immutability
`Tile[,]` is cloned on every mutation (move, capture, promotion). Methods return the new board array rather than modifying one in place. This eliminates shared-state bugs and makes it straightforward to reason about each game transition independently. The trade-off is garbage-collection pressure (due to new board array creations), which is acceptable given the small board size.

### Variant configurability via Rules
`Rules` is an open class with virtual properties and methods: `BoardAtStart`, `MoveDefinitionByType`, `PromotionPosition`, `EndConditions`, and others. A chess variant is a subclass that overrides only what differs — piece movement, board layout, promotion rules, or win conditions — without touching core logic. The engine is agnostic to standard vs. variant chess.

### Value types for domain primitives
`Position`, `Piece`, `EndCondition`, and `PromotionAxis` are `readonly record struct`. They get structural equality, immutability, and stack allocation for free, removing boilerplate and making comparisons and collection lookups correct by default.

### Named records for move concepts
`InPassingMove` and `CastlingMove` replace anonymous tuples in method signatures. Naming these concepts makes the signatures self-documenting and ties the code to the chess domain rather than to implementation structure.

## Testing

### Test-focused development
Tests are written and confirmed failing before fixes, refactors, and in most cases the implementation.

### Three-layer test architecture
The test suite follows the testing pyramid, with each layer isolated to what it owns:

- **`BoardTest`** — pure unit tests against `Board`, a static class with no side effects. Board state is constructed from a [string fixture](#board-state-fixtures) and passed in directly; no Unity runtime is needed, and no state leaks between tests.
- **`ChessBoardTest`** — integration tests against `ChessBoard`, verifying that mutable state is managed correctly across move sequences: castling eligibility after the king or rook has moved, en passant expiry after one turn, pinned pieces with no legal moves.
- **`GameTest`** — end-to-end tests through `Game.MovePiece`, covering the full move pipeline: multi-turn promotion sequences, en passant offered and taken, checkmate and stalemate resolution.

Keeping the layers separate means board logic can be verified without game state, and game-level behaviour can be verified without reimplementing board rules in the test.

### Unity Test Runner / NUnit
All tests run in EditMode via the Unity Test Runner and are executed automatically on every push via GitHub Actions CI. Parameterised cases use `[TestCaseSource]` with generated names, and individual test methods follow a behaviour-driven naming style (e.g. `Pinned_piece_has_no_legal_moves`), so the intent of any failing test is readable without opening the test body.

### Board state fixtures
`BoardTileString` is a library of named board positions encoded as compact strings. Each fixture describes a precise game state — castling available, en passant ready, king in check — and is shared across test classes. New scenarios can be added without touching test logic.

```csharp
public static string Can_castle_king_side()
{
    return @"
R2 N2 B2 Q2 K2 B2 N2 R2
P2 P2 P2 -- -- -- P2 P2
-- -- -- -- -- P2 -- --
-- -- -- P2 P2 -- -- --
-- -- -- -- -- -- -- --
-- -- -- B1 P1 N1 -- --
P1 P1 P1 P1 -- P1 P1 P1
R1 N1 B1 Q1 K1 -- -- R1
";
}
```

### Code coverage report
The report was generated after the test run and lists the coverage ratio of the `Chess.Runtime` assembly. You can view it in your web browser by clicking [this link](CodeCoverage/Report/index.html).

![Code Coverage Report: Coverage](~Documentation/Images/CoverageReport_coverage.png)

In the web browser, you can click to drill down to a specific method and see the coverage ratio.

![Code Coverage Report: Tile drilldown](~Documentation/Images/CoverageReport_tile.png)

## Next steps

In priority order:

1. **Game-over UI** — currently the end state (win/draw) is only logged to the console. A proper UI overlay would complete the gameplay loop visually.
2. **CI green build** — the GitHub Actions workflow is in place but requires a paid Unity license to configure the secrets. Once activated, tests run automatically on every push.
3. **Visual polish** — flatten the tile highlight cube so it reads as a surface highlight rather than a 3D object, and improve the promotion selection UI.
4. **Migrate to Input System** — the project uses the legacy Input Manager, which Unity 6 has marked for deprecation. Migrating to the new Input System package would future-proof the input handling.
5. **Migrate legacy UI.Text to TextMeshPro** — four UI text components still use the deprecated `UnityEngine.UI.Text`. Replacing them with `TextMeshProUGUI` would complete the TMP migration.

## Detailed images

Full test suite and coverage report detail:

![TestRunner_all_tests](~Documentation/Images/TestRunner_all_tests.png)

![Code Coverage Report: full](~Documentation/Images/CoverageReport_full.png)
