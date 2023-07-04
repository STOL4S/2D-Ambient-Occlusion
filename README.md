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
Looking at the self-shadow image, the edge facing closest to the viewpoint is shaded slightly darker on the side of the edge that appears
to be facing away from the light source. Due to the algorithm being able to calculate the average deviation in pixel color between a single pixel
and it's surrounding neighbors, it can then calculate that the difference between any given pixel is greater than the average deviation and determine
that this pixel is likely a part of an edge. It is then determined if this edge is an edge facing outwards or inwards by using surrounding color data
to make guesses at the current position of the pixel in world space versus the neighboring pixels. The Z-axis or vertical axis in this case is always a
pre-generated gradient which has a near value of 255 (white) and a far value of black (0). The values of the calculated shadows are then multiplied by the
pre-generated gradient to give the effect that shadows are appearing on the ground rather than just behind or around the object. The generated shadow map
is than applied to the image by multiplying the original color values of the passed image by the ambient occlusion factor. Prior to doing this, the generated
shadow map can be blurred to provide a softer shadow, but if done on too low of a resolution it can appear washed out.
