# Chess


## Engineering Practices

- **TDD**: tests are written and confirmed failing before applying fixes or refactors.
- **Focused commits**: each commit addresses a single concern, keeping history readable and diffs reviewable.
- **Functional-first design**: core logic (`Board`) is a pure static class operating on immutable cloned state; `ChessBoard` owns mutable state and coordinates operations.