# Technical Requirements: Base Implementation (Thermometer Puzzle)

## 1. Overview

Mobile puzzle game where players fill thermometers on a grid to satisfy row and column constraints.

## 2. Puzzle Mechanics & Rules

- **Grid**: A 2D grid of size N x M.
- **Coordinate System**: (0,0) is the bottom-left corner.
- **Thermometers**:
    - Shapes consisting of connected cells.
    - Each thermometer has a designated **Bulb** (start) and **Tip** (end).
    - **Filling Rule**: A cell can only be filled if every previous cell in the thermometer (closer to the bulb) is
      already filled.
- **Constraints**:
    - Each **Row** has a target number of filled cells.
    - Each **Column** has a target number of filled cells.
- **Win Condition**: All row and column constraints are met, and all thermometer filling rules are respected.

## 3. Data Structures

### 3.1. Level Definition (`LevelConfig`)

- `int Width`: Number of columns.
- `int Height`: Number of rows.
- `int[] RowConstraints`: Array of required filled cells per row.
- `int[] ColumnConstraints`: Array of required filled cells per column.
- `List<ThermometerData> Thermometers`: Definition of all thermometers in the level.

### 3.2. Thermometer Data

- `List<Vector2Int> Cells`: Ordered list of cell coordinates. `Cells[0]` is the bulb.

### 3.3. Player Progress

- `CellState[,] CellStates`: 2 dimensional array of whether each cell in the grid is filled (persisted via
  `LevelService`). Saving this array is not required. Progress should NOT be saved between sessions.
- CellState is enum with values:
    - `Empty`
    - `Filled`
    - `CrossedOut`.

## 4. Systems

### 4.1. Level Service (`LevelService`)

- Responsible for loading `LevelConfig`.
- Translating `LevelConfig` into a runtime `LevelModel`.
- Maintains player progress by updating `CellStates`.
- Maps thermosters to thermometer cells.

### 4.2. Level Model (`LevelModel`)

- Holds the current state of the puzzle.
- Methods:
    - `bool CanToggleCell(Vector2Int coord)`: Checks if a cell can be filled/emptied based on thermometer rules.
    - `void ToggleCell(Vector2Int coord)`: Changes the state of a cell.
    - `bool IsSolved()`: Validates current state against constraints.

### 4.3. Validation Logic

- **Row/Column Validation**: Count filled cells in each row/column and compare with constraints.
- **Immediate Feedback**: Provide visual feedback when a row or column constraint is satisfied (e.g., change label color, show checkmark, or play an animation).
- **Thermometer Validation**: Ensure no "gaps" exist (e.g., cell 3 is filled but cell 2 is not).

### 4.4. Input System

- Detect taps on grid cells.
- Interface with `LevelModel` to attempt filling/emptying.
- Visual feedback for invalid moves or satisfied constraints.

## 5. UI Requirements

- **Canvas-Based Implementation**: The entire game grid and UI should be implemented within a Unity Canvas (UI System).
- **Grid Display**: Rendering the grid, thermometers (bulb shape vs body), and cell states.
- **Constraint Labels**: Displaying target counts for rows and columns.
- **Progress Indicators**: Changing label color or showing a checkmark when a constraint is met.

## 6. Integration

- **VContainer**: Register `LevelService`, `LevelModel`, and `InputHandler` in `RootScope`.
- **State Machine**:
    - `GameCoreState`: Load the asset and initialize, than active gameplay and input processing.
- **Persistence**: Level progress is not saved. Only the index of current level

## 7. Out of Scope

- Procedural puzzle generation.
- Hint system.
- Undo/Redo (to be considered for future).
