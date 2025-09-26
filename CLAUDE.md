# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 3D implementation of Bananagrams, a word-building tile game. The project uses Unity 6000.1.5f1 with the Universal Render Pipeline (URP) and includes the Odin Inspector for enhanced editor functionality.

## Build and Development Commands

### Unity Operations
- Open project in Unity Editor 6000.1.5f1 or newer
- Build through Unity Editor: File > Build Settings > Build
- Run tests through Unity Test Runner: Window > General > Test Runner

### Key Dependencies
- Unity Input System (1.14.0) - for modern input handling
- Universal Render Pipeline (17.1.0) - for rendering
- Odin Inspector (Sirenix) - for enhanced editor inspectors and serialization
- Unity Test Framework (1.5.1) - for unit testing

## Architecture Overview

### Core Game Systems

**GameManager** (`Scripts/Managers/GameManager.cs`)
- Central singleton coordinator managing game state
- Handles tile placement validation and state transitions
- Uses `LinearGameZone<LetterData>` for hand management instead of dedicated HandManager
- Manages reserve bag and discard pile using `Bag<LetterData>` and `List<LetterData>`
- Coordinates between Board, hand zone, and rule systems

**GameZone System** (`Scripts/GameZone.cs`, `Scripts/LinearGameZone.cs`, `Scripts/GridGameZone.cs`)
- **GameZone<T>**: Abstract base class for all game areas (hand, board, etc.)
- **LinearGameZone<T>**: Sequential storage with capacity limits (used for player hand)
- **GridGameZone<T>**: 2D grid storage with coordinate indexing (used for board)
- Uses renderer pattern (`IZoneRenderer<T>`, `IGridZoneRenderer<T>`) for visual separation
- Supports Odin Inspector integration with validation and debugging tools

**Board System** (`Scripts/Board/Board.cs`)
- Inherits from `GridGameZone<LetterData>` for unified zone management
- Manages 20x20 grid with world-to-grid coordinate transformations
- Handles tile placement validation and word scanning
- Uses `IGridZoneRenderer<LetterData>` for visual representation

**Tile System** (`Scripts/Board/Tile.cs`, `Scripts/Board/TileFactory.cs`)
- Unified `Tile` component works in both Hand and Board locations
- `TileFactory` handles centralized tile creation with proper setup
- Tiles track their location context for behavior management
- Supports hover effects and visual state management

**Drag & Drop** (`Scripts/Board/DragDropController.cs`)
- Handles tile movement between hand and board zones
- Uses Unity's new Input System for modern input handling
- Implements grid snapping and visual preview during drag
- Coordinates with GameManager and GameZone system for state validation

### Configuration System

**GameConfig** (`Scripts/GameConfig.cs`)
- ScriptableObject-based configuration for all game settings
- Centralizes board dimensions, tile sizes, visual settings
- Create via: Assets > Create > Game > Config

**GameConstants** (`Scripts/GameConstants.cs`)
- Static wrapper providing compile-time access to configuration
- Organized into logical namespaces (Grid, Board, Hand, Visual)
- Provides fallback defaults if configuration not initialized

**GameRules** (`Scripts/GameRules.cs`)
- Defines letter distribution and tile prefabs
- Contains `LetterData` classes with letter, value, material, and locking functionality
- Expandable bag system for tile distribution management

**Data Structures** (`Scripts/LetterData.cs`, `Scripts/WordData.cs`)
- **LetterData**: Serializable class representing individual letters with scoring and locking
- **WordData**: Struct for word representation with scoring and text extraction from letter arrays

### Supporting Systems

**GridSystem** (`Scripts/Board/GridSystem.cs`)
- World-to-grid coordinate transformations
- Hand positioning calculations
- Grid snapping utilities

**InputManager** (`Scripts/Managers/InputManager.cs`)
- Wrapper around Unity's Input System
- Provides camera ray utilities for 3D mouse interactions

**Rendering System** (`Scripts/Renderers/`)
- **HandRenderer**: Handles visual rendering of linear zones (player hand)
- **BoardRenderer**: Handles visual rendering of grid zones (game board)
- Implements `IZoneRenderer<T>` and `IGridZoneRenderer<T>` interfaces

**Bag<T>** (`Scripts/ADT/Bag.cs`)
- Generic collection for random tile drawing
- Used for tile reserve management

**IReadOnlyGrid<T>** (`Scripts/ADT/IReadOnlyGrid.cs`)
- Interface for read-only grid access
- Implemented by `GridGameZone<T>` for safe external access to board data

## Key Design Patterns

- **Singleton Pattern**: GameManager for global state access
- **Generic Zone Pattern**: `GameZone<T>` system for unified area management (hand, board, etc.)
- **Renderer Pattern**: `IZoneRenderer<T>` separates logical zones from visual representation
- **Factory Pattern**: TileFactory for centralized object creation
- **Template Method Pattern**: Abstract GameZone with concrete LinearGameZone and GridGameZone
- **Configuration Objects**: ScriptableObjects for data-driven design
- **Event-Driven**: Tiles use events for decoupled communication

## Development Guidelines

### Code Organization
- Core game scripts in `Assets/Scripts/`
- Configuration assets in `Assets/Resources/`
- Visual assets (materials, textures) organized by letter in `Assets/Resources/Materials/` and `Assets/Resources/Textures/`
- Third-party assets in `Assets/Vendor/` and `Assets/Plugins/`

### Odin Inspector Usage
- Use `[Button]` attributes for inspector methods (common in GameManager, Board)
- `[TableMatrix]` for 2D array visualization (BoardState.tiles)
- `[SerializeField]` with `[OdinSerialize]` for complex data structures
- Custom property drawers in `Assets/Editor/`

### Testing
- Unity Test Framework available for unit tests
- Test files should follow Unity's testing conventions
- Use Test Runner window for execution

## Common Workflows

### Adding New Letters
1. Modify `GameRules` ScriptableObject to add new `LetterData`
2. Create corresponding material in `Assets/Resources/Materials/`
3. Add texture files in `Assets/Resources/Textures/`
4. Update `startingBag` distribution as needed

### Modifying Board Size
1. Update `GameConfig` asset with new dimensions
2. Board will automatically use new dimensions on initialization
3. Consider impact on `BoardPlaneGenerator` for visual representation

### Creating New Game Zones
1. Inherit from `LinearGameZone<T>` for sequential storage or `GridGameZone<T>` for grid-based storage
2. Create corresponding renderer implementing `IZoneRenderer<T>` or `IGridZoneRenderer<T>`
3. Configure zone capacity and container in the inspector
4. Use Odin Inspector attributes for enhanced editor experience

### Creating New Tile Behaviors
1. Extend `Tile` class or create new components
2. Use `TileFactory` for consistent object creation
3. Coordinate with `DragDropController` and GameZone system for interaction handling
4. Consider how tiles interact with different zone types (linear vs grid)