### PROJECT OVERVIEW
Unity 6 (6000.3.7f1) project focused on thermometer-related gameplay. It uses VContainer for Dependency Injection and a custom State Machine for game flow.

### ARCHITECTURAL RULES
- **Dependency Injection (VContainer)**:
  - Register services in `LifetimeScope` (e.g., `RootScope`).
  - Prefer constructor injection for classes.
  - Use `Lifetime.Singleton` for managers and services.
  - Entry points should implement `IStartable`, `ITickable`, etc.
- **State Machine**:
  - The game flow is managed by `GameStateMachine`.
  - States should implement `IGameState` (or similar interface) and be registered in `RootScope`.
  - Transitions are triggered via `_gameStateMachine.Enter<TState>()`.
- **Data Management**:
  - Use `PlayerDataManager` for all player-related persistent data.
  - Persistent data is stored using `PlayerPrefs` and JSON (Newtonsoft.Json).
  - Use deferred saving: set a flag (e.g., `_saveRequested`) and perform the actual save in `Tick()` to avoid frequent I/O.
- **Asynchronous Programming**:
  - Use `UniTask` instead of `Task` or `Coroutines` for async operations.
  - Use `.Forget(Debug.LogException)` for fire-and-forget tasks.

### CODING STYLE
- **Naming Conventions**:
  - PascalCase for Classes, Methods, Properties, and Events.
  - camelCase with underscore prefix for private fields (e.g., `_myField`).
  - UPPER_CASE for constants and `readonly static` fields (e.g., `PLAYER_DATA_KEY`).
  - Namespaces must match the folder structure (e.g., `Infrastructure.Player`).
- **Structure**:
  - Use `[SerializeField]` for fields exposed to the Inspector.
  - Group fields/properties logically.
  - Events should follow the `On[Action]` naming pattern (e.g., `OnPlayerDataSetup`).
- **Formatting**:
  - Use standard C# formatting rules (K&R style braces).
  - Keep classes focused (Single Responsibility Principle).

### EDITOR TOOLS
- **Menu Items**:
  - Custom menu items should be placed in `General/Tools/EditorTools/Editor/`.
  - Use `[MenuItem]` for utility functions like clearing saves or fast scene switching.
- **Scene Management**:
  - Use `EditorSceneManager` for editor-only scene operations.

### PROJECT STRUCTURE
- `Assets/Core/`: Game logic and systems (Levels, etc.).
- `Assets/Infrastructure/`: Framework-level code (DI Scopes, State Machine, Save System).
- `General/Tools/`: Utility classes.
- `General/Extensiosn/`: Extensions.