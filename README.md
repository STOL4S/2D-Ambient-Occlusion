# 2D Ambient Occlusion (Soft Shadowing)
This repository contains a C# class that is capable of taking a 2D image
and generate a shadow on the surface behind it, and shadows itself.
For the best results, use pixel art that is drawn from a side angle
or a top-down view. Other angles or non-pixel art can be used, but
some adjustment may have to be made to get it to work properly.

## Examples
### No Shadow
![NoShadow](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/9468ec4b-73bf-4b2a-9f07-495e4c9d8093)
### Shadow
![Shadow](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/9b7c6665-763d-452b-99ad-6d4f42fdf6ff)
### Self-Shadow
![SelfShadow](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/d055e811-092a-43b4-b148-f8d767ac9599)
### No Shadow&emsp;&nbsp;Self-Shadow
![dandellionorig](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/7ac41412-bf5d-412e-9b01-c737e4a277bf)
![Dandellion_SelfShadow128](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/cde10a34-afce-4982-b432-5ee2bbb3ab83)

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
