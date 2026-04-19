# Chess

A chess engine built in C# for Unity. The codebase demonstrates testability-first design, clean separation of concerns, and automated test infrastructure using the Unity Test Runner.

## Architecture

### Board / ChessBoard layering
Testability was the primary motivation for this separation. `Board` is a pure static class: every method takes its inputs and returns its outputs with no side effects, so it can be unit-tested directly — no Unity runtime required, no setup cost, and no state leaking between tests. `ChessBoard` owns the mutable game state (`Tile[,]`, player dictionaries) and delegates all logic to `Board`, keeping state mutations explicit and centralised.

### Clone-on-write immutability
`Tile[,]` is cloned on every mutation (move, capture, promotion). Methods return the new board array rather than modifying one in place. This eliminates shared-state bugs and makes it straightforward to reason about each game transition independently. The trade-off is GC pressure, which is acceptable given the small board size.

### Variant configurability via Rules
`Rules` is an open class with virtual properties and methods: `BoardAtStart`, `MoveDefinitionByType`, `PromotionPosition`, `EndConditions`, and others. A chess variant is a subclass that overrides only what differs — piece movement, board layout, promotion rules, or win conditions — without touching core logic. The engine is agnostic to standard vs. variant chess.

### Value types for domain primitives
`Position`, `Piece`, `EndCondition`, and `PromotionAxis` are `readonly record struct`. They get structural equality, immutability, and stack allocation for free, removing boilerplate and making comparisons and collection lookups correct by default.

### Named records for move concepts
`InPassingMove` and `CastlingMove` replace anonymous tuples in method signatures. Naming these concepts makes the signatures self-documenting and ties the code to the chess domain rather than to implementation structure.

## Testing

### Three-layer test architecture
The test suite follows the testing pyramid, with each layer isolated to what it owns:

- **`BoardTest`** — pure unit tests against `Board`, a static class with no side effects. Board state is constructed from a string fixture and passed in directly; no Unity runtime is needed, and no state leaks between tests.
- **`ChessBoardTest`** — integration tests against `ChessBoard`, verifying that mutable state is managed correctly across move sequences: castling eligibility after the king or rook has moved, en passant expiry after one turn, pinned pieces with no legal moves.
- **`GameTest`** — end-to-end tests through `Game.MovePiece`, covering the full move pipeline: multi-turn promotion sequences, en passant offered and taken, checkmate and stalemate resolution.

Keeping the layers separate means board logic can be verified without game state, and game-level behaviour can be verified without reimplementing board rules in the test.

### Unity Test Runner / NUnit
All tests run in EditMode via the Unity Test Runner and are executed automatically on every push via GitHub Actions CI. Parameterised cases use `[TestCaseSource]` with generated names, and individual test methods follow a behaviour-driven naming style — `Pinned_piece_has_no_legal_moves`, `En_passant_not_available_after_one_turn_has_passed` — so the intent of any failing test is readable without opening the test body.

### Board state fixtures
`BoardTileString` is a library of named board positions encoded as compact strings. Each fixture describes a precise game state — castling available, en passant ready, king in check — and is shared across test classes. New scenarios can be added without touching test logic.

### Test-focused development
Tests are written and confirmed failing before fixes or refactors are applied.
