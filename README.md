# BEARS VERSUS ZOMBIES
![Demo GIF](images/demo.gif)

The objective of the game is to protect the school bus from waves of zombies and survive.

**Victory Conditions:**
- All zombies are eliminated
- All players survive
- The bus remains intact

Players can move around the map. The player’s character automatically targets the nearest enemy and opens fire on them.

For every enemy killed, players earn money. Money can be used to purchase mines or turrets.

A turret automatically targets enemies and fires at them. The turret has a limited lifespan and disappears after its time expires. 

![Turret Image](images/turret.png)

A mine kills all enemies within a certain radius.

![Turret Image](images/mine.png)

## Main Menu

In tha main menu you can choose either a single-player game or create a session for online multiplayer. In the main menu, you can select a region for online connection and set the language. 
The selected region and language settings are saved using the save manager.

![Architecture Schema](images/main-menu.png)

In the lobby menu, you can either create a new session or join an existing one

![Architecture Schema](images/lobby-menu.png)

## Technologies Used  

- Photon Fusion 2 (Host Mode)
- Addressables
- R3
- ZLinq
- A* Pathfinding Project Pro
- Magic Light Probes
- DOTween
- Animora UI Animation
- Odin Inspector/Validator
- I2 Localization

**AI and Movement:**  
- **Zombie AI:** Utilizes the FSM (Finite State Machine) addon and **A\* Pathfinding Project Pro** for pathfinding.  
- **Player Movement:** Powered by the **SimpleKCC** (Kinematic Character Controller) addon for smooth and responsive movement.

## Project Sctucture
The project structure follows a modular architecture. Independent modules that can be reused across different projects are located in the `Assets/Modules` folder.
The `Assets/Game/Scripts` folder contains the integration code that links the modules together, as well as project-specific code.
Sections directly related to the game project itself are located in the `Assets/Game` folder.

- Assets
  - Game
    - Configs
    - Input
    - Materials
    - Prefabs
    - Scripts
    - etc
  - Modules
    - Animation
    - AssetsManagement
    - etc

## Project Architecture

![Architecture Schema](images/game-architecture-schema.png)

The project architecture follows a partial component-based style with the use of Zenject.

In the main menu scene, dependency injection is handled exclusively via Zenject. In the gameplay scene, however, dependencies are injected from two sources:

- GameContextService
- Zenject ProjectContext

GameContextService is used to access components based on SimulationBehaviour and NetworkBehaviour, while Zenject ProjectContext is used to access global components.

This approach for the gameplay scene was chosen due to the architectural characteristics of Photon Fusion 2.

The framework provides two core classes for building networked games:

- NetworkBehaviour
- SimulationBehaviour

These classes are tightly coupled with Fusion's internal execution cycle and network buffer synchronization. Both inherit from MonoBehaviour.

Since the framework inherently requires a traditional component-based approach, the decision was made to use Zenject only for global dependency injection, while injecting local dependencies through the GameContextService

Under the hood, GameContextService also utilizes Fusion’s built-in service locator - NetworkRunner.GetSingleton

### Game Loader

Responsible for initializing the game and loading the main menu. It receives the loading tree as a dependency and executes it. It is a Don't Destroy On Load object, and its presence in the scene structure indicates whether the game has been initialized or not.

### Game Facade

Responsible for various aspects of the game's interaction with the Photon Fusion 2 framework. Since Fusion 2 is a large and complex framework, the `GameFacade` was implemented as a single `partial` class for easier maintenance. 

Each part of the class handles a specific aspect of the interaction with Photon:

- GameFacade
  - GameFacade_AppSettings
  - GameFacade_Components
  - GameFacade_HostMigration
  - GameFacade_Lobby
  - GameFacade_Regions
  - GameFacade_Shutdown
  - GameFacede_Connection
  - GameFacede_Disconnect

### Network Runner

This is a preconfigured prefab with network components. It must be instantiated anew each time a new game is created or when connecting to a lobby. Reusing it across different connections is not allowed due to limitations of the framework itself.

### Network Scene Manager

This is an extension of the standard NetworkSceneManagerDefault class.

The class implements additional functionality:
- Registration of all SimulationBehaviour instances in the NetworkRunner
- Initialization of the PlayersService

### Poolable Network Object Provider

This is an extension of the standard NetworkObjectProviderDefault class.

The class adds the following functionality:
- Ability to spawn objects from a pool
- Dependency injection into the spawned objects using Zenject ProjectContext

### Players Service

Responsible for:

- Spawning players in the scene when they join the game
- Reconnecting a player to their previous character upon rejoining
- Providing access to any player's NetworkObject

### Game Context Service

Provides access to any local SimulationBehaviour or NetworkBehaviour and serves as a service locator for gameplay components within the scene.

The service is initialized by components present in the scene. If a required scene component is not found, it falls back to NetworkRunner.GetSingleton.

Relying solely on `NetworkRunner.GetSingleton` is not feasible, as it only returns components attached to the same GameObject as the NetworkRunner. Since NetworkRunner is not bound to the scene hierarchy, there arose a need for GameContextService, which provides access to both local scene components and those available through NetworkRunner.