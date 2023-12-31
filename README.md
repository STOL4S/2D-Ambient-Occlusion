# 2D Ambient Occlusion (Soft Shadowing)
This repository contains a C# class, and an HLSL file, that are capable of generating
ambient occlusion shadows in a 2D scene. This algorithm works by
generating a 3D position buffer from the 2D scene and then
calculating soft shadows using this information. If self-shadowing
is enabled, then another pass is done and each sprite is scanned
individually and checked for occlusion on itself.

This was originally only a C# shader, but now is available as an HLSL shader
in the AOHLSL folder. This shader was intended for use in MonoGame, but could
be used in any DirectX project. The original C# function is still available
and could potentially be used in other projects involving software rendering only.

## Examples
### No Shading
![Buffer](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/8a84c6a5-1337-4c0f-9e5c-1478543f6e12)
### Ambient Occlusion
![GeneratedComposite](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/f371e30a-ac95-4666-a679-0915581742eb)
### Generated Occlusion Map
![AO](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/5a83ea8b-66b4-4ee5-9b5c-a41c489fc06b)

### No Shading
![Buffer](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/e9b45384-9187-4371-ad0e-39b2e8ac2e66)
### Ambient Occlusion (Self Shadow)
![AO_SS](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/f3c3f7aa-b776-4103-aa9f-664c593cb3d4)
### Rendered Scene
![RenderedScene](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/ca876fdb-199c-4b48-82aa-3336622afd5d)

## How Does it Work?
The function starts with passing a Bitmap BackBuffer and an array of sprites to be drawn to the SSAO function.
A position buffer is then calculated by ordering the sprites in drawing order. Sprites that are in the background
will have lower red values on the position buffer than sprites in the foreground. The blue value in the position
buffer corresponds with the local y position of the current pixel. This allows the algorithm to determine if the
sprite is too high off the ground to cast a shadow onto the ground. Based on the information in the position buffer,
depth-based SSAO can then be calculated for the 2D scene.

![DepthBuffer](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/79c5a2fd-cae9-4ace-b033-278f6bf125d7)

It is hard to see in the position buffer, but the trees all have a different red value from eachother with a blue gradient going upwards.
Similar to how a position buffer would be generated in a 3D world. This gives just enough information to calculate some 3D shaders
into this 2D scene.
