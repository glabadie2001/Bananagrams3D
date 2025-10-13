flowchart BT
Bag(Bag)
BagLetterEntry(BagLetterEntry)
Board(Board)
Board(Board)
BoardPlaneGenerator(BoardPlaneGenerator)
BoardRenderer(BoardRenderer)
BoardState(BoardState)
DragEndEvent(DragEndEvent)
DragStartEvent(DragStartEvent)
EventManager(EventManager)
GameConfig(GameConfig)
GameConstants(GameConstants)
GameEvent(GameEvent)
GameManager(GameManager)
GameRules(GameRules)
GameZone(GameZone)
Grid(Grid)
GridGameZone(GridGameZone)
GridSystem(GridSystem)
Hand(Hand)
HandRenderer(HandRenderer)
IDraggable(IDraggable)
IGridZoneRenderer(IGridZoneRenderer)
IReadOnlyGrid(IReadOnlyGrid)
ISwappable(ISwappable)
IZoneRenderer(IZoneRenderer)
InputManager(InputManager)
LetterData(LetterData)
LinearGameZone(LinearGameZone)
Tile(Tile)
TileFactory(TileFactory)
UIManager(UIManager)
Visual(Visual)
WordData(WordData)

Board  -->  GameConfig 
Board  -->  GameConstants 
Board  -..->  GameRules 
Board  -->  GameRules 
Board  -->  GameZone 
Board  -..-|>  GridGameZone 
Board  -->  GridGameZone 
Board  -->  GridSystem 
Board  -->  IGridZoneRenderer 
Board  -->  IZoneRenderer 
Board  -->  LetterData 
Board  -->  WordData 
BoardRenderer  -..-|>  IGridZoneRenderer 
BoardRenderer  -->  IReadOnlyGrid 
BoardRenderer  -->  LetterData 
BoardRenderer  -->  Tile 
BoardRenderer  -->  TileFactory 
BoardState  -..->  LetterData 
BoardState  -->  LetterData 
DragEndEvent  -..-|>  GameEvent 
DragEndEvent  -..->  IDraggable 
DragEndEvent  --*  IDraggable 
DragStartEvent  -..-|>  GameEvent 
DragStartEvent  -->  GameManager 
DragStartEvent  --*  IDraggable 
DragStartEvent  -->  IDraggable 
DragStartEvent  -..->  IDraggable 
EventManager  -..->  EventManager 
EventManager  -->  GameEvent 
GameConstants  -->  GameConfig 
GameConstants  -..->  GameConfig 
GameConstants  -->  GameConfig 
GameManager  -->  Bag 
GameManager  -..->  Bag 
GameManager  -..->  Board 
GameManager  -->  Board 
GameManager  -->  DragStartEvent 
GameManager  -->  EventManager 
GameManager  -..->  GameConfig 
GameManager  -->  GameConstants 
GameManager  -..->  GameManager 
GameManager  -->  GameRules 
GameManager  -..->  GameRules 
GameManager  -->  GameZone 
GameManager  -->  GridGameZone 
GameManager  -->  IDraggable 
GameManager  -->  ISwappable 
GameManager  -->  IZoneRenderer 
GameManager  -->  InputManager 
GameManager  -->  LetterData 
GameManager  -->  LinearGameZone 
GameManager  -..->  LinearGameZone 
GameManager  -..->  Tile 
GameManager  -->  Tile 
GameRules  -->  BagLetterEntry 
GameRules  -->  LetterData 
GameZone  -->  IZoneRenderer 
GameZone  -->  IZoneRenderer 
Grid  -->  GameConfig 
Grid  -->  GameConstants 
GridGameZone  -->  GameZone 
GridGameZone  -..-|>  GameZone 
GridGameZone  -->  GridSystem 
GridGameZone  -->  IGridZoneRenderer 
GridGameZone  -->  IGridZoneRenderer 
GridGameZone  -..-|>  IReadOnlyGrid 
GridGameZone  -->  IZoneRenderer 
GridSystem  -->  GameConstants 
GridSystem  -->  Grid 
Hand  -->  GameConfig 
Hand  -->  GameConstants 
HandRenderer  -->  GridSystem 
HandRenderer  -..-|>  IZoneRenderer 
HandRenderer  -->  LetterData 
HandRenderer  -->  Tile 
HandRenderer  -->  TileFactory 
IGridZoneRenderer  -->  IReadOnlyGrid 
IGridZoneRenderer  -..-|>  IZoneRenderer 
ISwappable  -->  GameZone 
InputManager  -..->  InputManager 
LetterData  -..->  GameZone 
LetterData  -->  GameZone 
LetterData  --*  GameZone 
LetterData  -..-|>  ISwappable 
LetterData  --*  LetterData 
LetterData  -->  LetterData 
LetterData  -..->  Tile 
LetterData  -->  Tile 
LinearGameZone  -->  GameZone 
LinearGameZone  -..-|>  GameZone 
LinearGameZone  -->  IZoneRenderer 
Tile  -->  Board 
Tile  -->  GameConfig 
Tile  -->  GameManager 
Tile  -->  GameZone 
Tile  -->  GridGameZone 
Tile  -..-|>  IDraggable 
Tile  -->  InputManager 
Tile  -->  LetterData 
Tile  -->  LetterData 
Tile  -..->  LetterData 
TileFactory  -->  LetterData 
TileFactory  -->  Tile 
UIManager  -->  Board 
UIManager  -->  GameManager 
UIManager  -..->  UIManager 
UIManager  -->  WordData 
Visual  -->  GameConfig 
Visual  -->  GameConstants 
WordData  --*  LetterData 
WordData  -->  LetterData 
WordData  -..->  LetterData 
