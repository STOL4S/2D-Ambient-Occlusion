# 2D Ambient Occlusion (Soft Shadowing)
This repository contains a class that is capable of taking a 2D image
and generate a shadow on the surface behind it, and shadows itself.
For the best results, use pixel art that is drawn from a side angle
or a top-down view. Other angles or non-pixel art can be used, but
some adjustment may have to be made to get it to work properly.

## Examples
### No Shadow&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp;&nbsp;&emsp;&emsp;Shadow&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&emsp;&emsp;&emsp;&emsp;Self-Shadow
![NoShadow](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/9468ec4b-73bf-4b2a-9f07-495e4c9d8093)
![Shadow](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/9b7c6665-763d-452b-99ad-6d4f42fdf6ff)
![SelfShadow](https://github.com/STOL4S/2D-Ambient-Occlusion/assets/138336394/d055e811-092a-43b4-b148-f8d767ac9599)

When using this shader, self-shadowing can be enabled/disabled when calling the function to generate the ambient occlusion.
Regular shadowing tends to give darker shadows under the object itself, while leaving most of the shading of the object itself
alone. Self-shadowing gives a lighter shadow on the ground, but also shadow details in areas where edges are detected in the image.

