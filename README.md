# Particle_Performance_Test
Simple Unity particle simulation, testing the performance of individual game objects vs drawing each particle to a single object.

To test performance, there are three __Test States:__
- __Simulated__ - Simulates particles as custom classes- No gameObjects, only one Unity update loop running on the main handler. Requires gizmos to be enabled.
- __As Game Objects__ - Simulates each particle as their own gameObject. This runs identical code, with each particle operating within their own update loop. Does not require gizmos.
- __Rendered Objects__ - Simulates particles using the same structure as "Simulated," but attaches a simple SpriteRenderer to each one. Does not require gizmos.

<img src="https://user-images.githubusercontent.com/6518580/163490647-df434e8d-a83b-47fa-9167-e4c3c61e76a6.gif" height="300" />

## Results
Viewing the framerate through Unity's stats at runtime, I was getting the following results:
- __Simulated__ 
  - _Gizmos on:_ 280-300 fps
  - _Gizmos off:_ 470-500 fps
- __As Game Objects__
  - _Gizmos on:_ 240-260 fps
  - _Gizmos off:_ 350-360 fps
- __Rendered Objects__
  - _Gizmos on:_ 240-260 fps
  - _Gizmos off:_ 370-380 fps

Simulated with gizmos off results in no visible particles, but comparing between Game Objects and Rendered Objects, there is a slight difference. At higher magnitudes of particles, the impact will likely be much greater, but it was interesting to discover that a single particle-handler, coordinating the updates and collisions for all particles, was more efficient than each particle running their own updates.

---

This was meant to be the beginning of a 2D liquid simulation for a game idea, but the project ended here. There were some fun discoveries, nonetheless.
