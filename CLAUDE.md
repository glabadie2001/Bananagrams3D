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

**GameManager** (`Scripts/GameManager.cs`)
- Central singleton coordinator managing game state
- Handles tile placement validation and state transitions
- Manages hand size (default 21 tiles) and tile distribution
- Coordinates between Board, HandManager, and rule systems

**Board System** (`Scripts/Board.cs`)
- Separates logical board state (`BoardState`) from visual representation
- Manages 20x20 grid with world-to-grid coordinate transformations  
- Handles tile placement validation and word scanning
- Uses `BoardPlaneGenerator` for visual grid representation

**Tile System** (`Scripts/Tile.cs`, `Scripts/TileFactory.cs`)
- Unified `Tile` component works in both Hand and Board locations
- `TileFactory` handles centralized tile creation with proper setup
- Tiles track their `TileLocation` (Hand/Board) for context-aware behavior
- Supports hover effects and visual state management

**Hand Management** (`Scripts/HandManager.cs`)
- Manages player's hand tiles with automatic positioning
- Handles 7-tiles-per-row layout with configurable spacing
- Provides bounds checking for drag-drop operations
- Singleton pattern for global access

**Drag & Drop** (`Scripts/DragDropController.cs`)
- Handles tile movement between hand and board
- Uses Unity's new Input System for modern input handling
- Implements grid snapping and visual preview during drag
- Coordinates with GameManager for state validation

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
- Contains `LetterData` structs with letter, value, and material
- Expandable bag system for tile distribution management

### Supporting Systems

**GridSystem** (`Scripts/GridSystem.cs`)
- World-to-grid coordinate transformations
- Hand positioning calculations
- Grid snapping utilities

**InputManager** (`Scripts/InputManager.cs`) 
- Wrapper around Unity's Input System
- Provides camera ray utilities for 3D mouse interactions

**Bag<T>** (`Scripts/ADT/Bag.cs`)
- Generic collection for random tile drawing
- Used for tile reserve management

## Key Design Patterns

- **Singleton Pattern**: GameManager, HandManager for global state access
- **Factory Pattern**: TileFactory for centralized object creation  
- **Separation of Concerns**: Board logic separate from visual representation
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

### Creating New Tile Behaviors
1. Extend `Tile` class or create new components
2. Use `TileFactory` for consistent object creation
3. Update `TileLocation` enum if adding new contexts
4. Coordinate with `DragDropController` for interaction handling