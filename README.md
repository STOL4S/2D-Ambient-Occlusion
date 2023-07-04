# 2D Ambient Occlusion (Soft Shadowing)
This repository contains a C# class that is capable of taking a 2D image
and generate a shadow on the surface behind it, and shadows itself.
For the best results, use pixel art that is drawn from a side angle
or a top-down view. Other angles or non-pixel art can be used, but
some adjustment may have to be made to get it to work properly.

## Examples
### No Shading
![Buffer](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/8a84c6a5-1337-4c0f-9e5c-1478543f6e12)
### Ambient Occlusion
![GeneratedComposite](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/3a3f8ce4-dd24-414b-944c-b6fdca547851)
### Generated Occlusion Map
![AO](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/e6425ea4-3d30-4c25-bf85-ffdc8d11481f)

When using this shader, self-shadowing can be enabled/disabled when calling the function to generate the ambient occlusion.
Regular shadowing tends to give darker shadows under the object itself, while leaving most of the shading of the object itself
alone. Self-shadowing gives a lighter shadow on the ground, but also shadow details in areas where edges are detected in the image.

## How Does it Work?
Currently, a back buffer bitmap and an array of all the sprites to be drawn are passed to the ambient occlusion function.
A position buffer is then calculated by ordering the sprites in drawing order and setting sprites furthest back to darker
colors, while sprites in the front are lighter colors. Here is an example:

![Pos](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/4821d66d-a552-41cf-9440-45eaa45e2fed)

From there, the rest of the shader is very similar to the 3D version of this shader. Check surrounding pixels, get the distance
between you and the neighboring object, and draw shadow based off occlusion amount. After checking for all shadows between sprites,
another pass is done very quickly to fill in any shadows between the sprites and the background scene.
