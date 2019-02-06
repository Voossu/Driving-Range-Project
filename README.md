# Driving-Range-Project

The program implements a deep feed forward neural network controller for a vehicle from Unity Standard Assets. In addition, the program streamlined the approach of learning learning for a gaming car. Training is carried out on the basis of a genetic algorithm. 

Additional information can be found in the Docs folder. It is presented in Ukrainian.

https://www.youtube.com/watch?v=G7EHDXepJKw.


## Project Structure

- \Assets\[Common] - Assets of the game environment
  - \Cameras - Camera Scenarios
  - \Geometry - Game Environment Resources
  - \Icons - UI Icons
  - \Time - Time Managment Scripts 
  - \Vehicles - Car Implementation
- \Assets\[Utilitys] - Tools and extensions
  - Attributes - Unity Attribute Scripts
  - NNAgents - Neural Network Implementation 
  - UIExtend - GUI Primitives Scripts


## Vehicles 

The implementation of the game car logic has been reworked. Fixed a bug in the implementation of the mechanics of WayPoints, improved the mechanics for the final sequential route, changed the editor interface in several scripts, implemented the mechanics of control points, fixed a bug with the machine not moving after stopping, revised some mathematics in the car controller, added the ability to disable wheel animation. And many more changes are small and not quite small.


## Can be improved 

- Modify the respawn system (Planned to take a car controller script and a spawn script via Generic. But the design was defective).
- Implement a neural network agent based on Component logic (Similarly, the implementation of sensors logic).


## Prerequisites

- Unity3D 2019.1.0
- TextMesh Pro 1.3.0
- ProGrids 2.5.0
- ProBuilder 3.0.9
