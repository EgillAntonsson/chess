# 2D view toggle — implementation plan

Adds a runtime-toggleable 2D view alongside the existing 3D view, demonstrating that the View layer is decoupled from the domain model. Code freeze: Tuesday 2026-05-05 14:00. Interview: Wednesday 2026-05-06 14:00.

## Confirmed decisions

1. **Toggle owner:** new `ViewModeController` MonoBehaviour. Holds the two mappers, the two camera poses, the default-mode field, and the toggle button hook.
2. **Camera:** single `Main Camera` with two serialized poses (`pose3D`, `pose2D`) and an `orthographic` flag per pose. Swap is an instant transform/projection assignment. (Animated pan is deferred to the polish step.)
3. **2D piece prefabs:** one prefab per `(PieceType, PlayerId)` (12 total), each containing a `SpriteRenderer` with a placeholder white square sprite (tinted per player) + a `TextMeshPro` child showing K/Q/R/B/N/P. Future asset swap = drop new sprites onto the 12 prefabs; no code or scene changes.
4. **Default mode:** 3D (matches the README's current showcase).
5. **UI:** single `Button` anchored top-right of the existing Canvas. Label flips between "2D" / "3D".

## Architectural posture (must hold across every step)

- **Domain layer is untouched.** `Chess.Runtime` has zero references and stays that way. No new files in `Scripts/Runtime/` outside `View/`.
- **No game-state coupling.** Toggling the view never reads or mutates `Game`/`ChessBoard`/`Player`. The `Game` instance and `Tile[,]` are unaware of which view is active.
- **Reskin is a view-only operation.** `ChessBoardView` already takes the mapper per-call (`TileView.InjectTile(tile, mapper)`); the swap is "rebuild all piece GOs with a different mapper", nothing more.
- **The 3D board cubes (`TileView`) are reused for both views** — clicks and `MarkTile` highlights work identically in 2D.

## Step 1 — 2D piece prefabs + second mapper asset

**Goal:** 2D piece visuals exist as drop-in replacements for the 3D ones, verified in-editor.

**Work:**
- Create `Assets/Prefabs/2D/PieceBase.prefab` — empty GO with a child `SpriteRenderer` (white-square placeholder sprite) and a child `TextMeshPro` (3D, not UGUI — must render in world space above the sprite). Local position centred on a 1×1 tile, slightly above the cube top so it z-orders correctly under both perspective and orthographic cameras.
- Create 12 prefab variants of `PieceBase` under `Assets/Prefabs/2D/`:
  - `Pawn1.prefab`, `Knight1.prefab`, `Bishop1.prefab`, `Rook1.prefab`, `Queen1.prefab`, `King1.prefab` (player 1, light tint, "P/N/B/R/Q/K")
  - same six for player 2 (dark tint)
- Create `Assets/ScriptableObjects/PiecePrefabMapper2D.asset` (instance of existing `PiecePrefabMapper`), wired to the 12 new prefabs in the same `PieceType[]` order as the 3D mapper.

**Verification (in-editor only, not committed):**
- Temporarily drag `PiecePrefabMapper2D` onto `ChessBoardView.piecePrefabMapper` in the scene, enter Play mode → confirm letters render on each starting tile, click → highlight + move works, captures + promotions render correctly.
- Revert the inspector field, exit Play mode.

**Presentable state:** Run the game with the existing 3D view — unchanged. The 2D mapper exists as an unused asset, ready for Step 2.

**Estimated commits:** 1–2 (one for the 12 prefabs + base prefab, one for the mapper asset, or bundled).

---

## Step 2 — `ViewModeController` + runtime toggle (interview-ready milestone)

**Goal:** A button on the Canvas toggles between 3D and 2D at runtime. Camera repositions, pieces re-skin, game state is untouched.

**Work:**

1. **`ChessBoardView.Reskin(PiecePrefabMapper newMapper)`** — new public method:
   ```csharp
   public void Reskin(PiecePrefabMapper newMapper)
   {
       piecePrefabMapper = newMapper;
       foreach (var tv in tileViewByPosition.Values)
           tv.InjectTile(tv.Tile, newMapper);
   }
   ```
   Reuses the existing `InjectTile` path (which already destroys the old child piece GO and instantiates the new one). No other changes to `ChessBoardView`.

2. **`ViewModeController.cs`** (new file in `Scripts/Runtime/View/`):
   - Serialized fields:
     - `ChessBoardView chessBoardView`
     - `Camera mainCamera`
     - `PiecePrefabMapper mapper3D`, `mapper2D`
     - `CameraPose pose3D`, `pose2D` (small serializable struct: position, euler, orthographic bool, ortho size, fov)
     - `Button toggleButton`
     - `TextMeshProUGUI toggleLabel`
     - `ViewMode defaultMode = ViewMode.ThreeD`
   - `Start()` — applies `defaultMode` (sets camera pose, calls `chessBoardView.Reskin(...)`, updates label), wires `toggleButton.onClick` → `ToggleView`.
   - `ToggleView()` — flips current mode, applies pose + reskin + label.
   - `enum ViewMode { ThreeD, TwoD }`
   - **No reference to `Game`/`ChessBoard`/`GameController`.** Verify the file's `using` list to be sure.

3. **Scene wiring (`GameScene.unity`):**
   - Add empty GO `ViewModeController` at root with the new component, assign all serialized fields.
   - Configure `pose3D` from the current camera transform (perspective, position 3.5, 7.6, −0.5, the existing rotation).
   - Configure `pose2D` as orthographic, top-down (e.g. position 3.5, 10, 3.5; rotation 90,0,0; ortho size ~4.5 so the 8×8 board fills view comfortably).
   - Add a `Button` to the existing Canvas, anchor top-right, default text "2D".
   - Wire the button reference into `ViewModeController`.

**Verification:**
- Play mode: starts in 3D, looks identical to before.
- Click "2D" → camera snaps to top-down, pieces become letter sprites, button label flips to "3D".
- Click "3D" → reverse.
- Make a move in either view → game continues; toggle mid-game → state preserved, captured pieces gone, promoted pieces correct.
- Toggle during opponent's check highlight → highlight color persists (it lives on the tile renderer, not the piece).
- Run the existing test suite (`Chess.Test.EditMode`) → all green; no domain code changed.

**Presentable state:** This is the interview-ready milestone. The point you can demonstrate: "the View toggle is a 50-line MonoBehaviour that knows nothing about chess — it asks `ChessBoardView` to re-skin, and moves the camera. The domain doesn't even know it happened, and the compiler enforces that."

**Estimated commits:** 2–3 (Reskin method; ViewModeController.cs + scene wiring; small follow-up tweaks).

---

## Step 3 — Polish (only if time before code freeze)

Each item below is independently shippable; pick the highest-value ones first.

- **README update.** Replace the "2D view with toggle" item under *Next steps* with a short *2D view* section: one paragraph noting that the toggle is a pure View concern (no domain changes), with a screenshot of each view. Bump the architecture diagram if the new component is worth surfacing.
- **2D camera framing.** Tune the orthographic size and offset so the board fills the screen exactly with a small margin. (Trivial inspector tweak; do this only after taking the screenshot if the README is updated.)
- **2D piece visual pass.** Pick a readable TMP font, increase letter contrast vs. tile colour, ensure the placeholder sprite tint is clearly distinguishable per player. Still no real sprite assets — just placeholder polish.
- **Animated camera transition.** Replace the instant pose swap with a coroutine (or `Update`-driven lerp) that pans the camera over a configurable duration. Add a `ViewTransitionConfig` ScriptableObject (`durationSeconds`, `easing`) and reference it from `ViewModeController`. Pieces still vanish/appear instantly; the camera animates.
- **Tile-highlight flatness.** The README already lists this — out of scope for this branch, leave as-is.
- **Android build smoke-test (bonus).** Build to phone with the current legacy Input Manager and verify tap-to-select works via `OnMouseDown`'s touch fallback (Unity treats touch-0 as mouse under the legacy system). If broken, spike a small fix — usually a missing `PhysicsRaycaster` or `EventSystem` config. **Do not migrate to the Input System on this branch:** `OnMouseDown` is the only input hook in the codebase (`TileView.cs:48`), so a regression there breaks the demo with no easy rollback. Migration is already listed as its own *Next steps* item in the README and stays there.

**Presentable state:** Whatever subset of the above is done; each is incremental and can be stopped at any commit.

---

## Risk and fallback

- **Highest risk:** TextMeshPro 3D rendering quirks when switched between perspective and orthographic cameras (face-camera billboarding, scale, occlusion vs. the tile cube). Mitigation: lift the TMP slightly above the tile (`y = 0.51` on a unit cube), set its rotation to face up so the same orientation reads correctly from both cameras. If TMP fights us, fall back to an unlit `SpriteRenderer` with pre-baked letter textures (one per piece type) — same prefab structure, less runtime nuance.
- **If running out of time after Step 1:** the 3D view ships as-is; mention the 2D-prefab work in the README as an in-progress branch. The committed Step 1 work is harmless and demonstrably architecture-respecting.
- **If running out of time after Step 2:** this is the natural stopping point. Step 3 is optional polish.

## Stop conditions

After each step, evaluate:
- Is the existing 3D view still pixel-identical when running with `defaultMode = ThreeD`?
- Did any file under `Scripts/Runtime/` (excluding `View/`) change? (Should be **no** for the entire branch.)
- Did any test under `Scripts/Tests/` need to change? (Should be **no**.)

If yes to the first and no to the others → architecture posture held; safe to ship for the interview.
