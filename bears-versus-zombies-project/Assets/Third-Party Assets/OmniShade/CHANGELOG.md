1.9.3
June 26, 2025
・Fix for Forward+ in Unity 6.1

1.9.2
June 3, 2025
・Fix bug with Saturation affecting fog color

1.9.1
April 18, 2025
・Update to Unity 2022

1.9.0
February 28, 2025
・Normal Map: Support adjusting strength for each normal map
・Commented out some seldom-used shader flags to improve build time

1.8.9
February 4, 2025
・Added support for UV2 selection in Normal Map 2 and Layers 1, 2, 3

1.8.8
January 15, 2025
・Outline: Fixed outline Z offset

1.8.7
January 13, 2025
・Outline: Add support for outline Z offset

1.8.6
November 10, 2024
・URP: Add support for light cookies

1.8.5
September 13, 2024
・Point lights fixed in Forward+ pipeline

1.8.4
August 17, 2024
・Point lights fixed in URP

1.8.3
July 28, 2024
・Cutout transparency: Support to specify cutoff.

1.8.2
July 25, 2024
・Shadow colors fixed for point lights.

1.8.1
April 25, 2024
・Fix shader compilation warning on older platforms. 

1.8.0
March 22, 2024
・UV Tile Discard: Bug fix
・Fix to support latest version of Polybrush

1.7.9
March 15, 2024
・URP: Fixed script error on Unity 2023

1.7.8
January 19, 2024
・URP: Fixed Forward+ error on Unity verions before 2022.3.17

1.7.7
January 19, 2024
・Outline: Added camera-independent width option

1.7.6
January 18, 2024
・URP: Added support for Forward+ rendering path

1.7.5
January 16, 2024
・URP: Added support for additional lights shadows
・Added support for UV Tile Discard

1.7.4
December 31, 2023
・Anime: Fixed bright spots artifacts with HDR Bloom

1.7.3
October 16, 2023
・Plant sway: Add support for local space option

1.7.2
October 16, 2023
・Plant sway: Fix so rotations don't affect sway direction

1.7.1
October 10, 2023
・URP: Fix for specular and reflection when SRP Batcher disabled and Instancing on

1.7.0
October 6, 2023
・URP: Fix light layers in Unity 2022

1.6.9
August 16, 2023
・Triplanar normal map: Added normal map 2 option, fixed bug with underside lighting

1.6.8
June 8, 2023
・Outline: Added option to hide interior outlines

1.6.7
June 3, 2023
URP: Added support for Screen Space Ambient Occlusion

1.6.6
June 1, 2023
・Animate textures script: Fix TransparencyMask

1.6.5
April 8, 2023
・Fixed default preset for Opaque blend settings

1.6.4
February 20, 2023
・Shadows: Fixed bug with Mixed Lighting on iOS/Android devices
・Flat shading: Fixed bug on Vulkan
・Lightmap: Fix bug with Unity 2020

1.6.3
February 15, 2023
・Shadows: Fixed bug with Mixed Lighting shadow fading

1.6.2
February 11, 2023
・Shadows: Fixed bug with Mixed Lighting where shadows appeared on back faces
・Shadows: Fixed distance fading in URP

1.6.1
February 9, 2023
・URP Baked Lighting: Fixed specular and light probe shadows

1.6.0
January 28, 2023
・Updated base version to Unity 2021
・Triplanar: Added Local Space option
・Diffuse Mixed Lighting: Renamed to Baked and Dynamic Lights, and fixed
・URP Mixed Lighting: Fixed shadows
・Code cleanup

1.5.8
January 16, 2023
・Outline: Fix flickering when used with HDR in URP

1.5.7
December 20, 2022
・Fixed Blend Preset for Opaque, which caused issues with HDR mode

1.5.6
December 9, 2022
・URP Shadows: Fix shadows with GPU Instancing
・Fix bug when used with OmniShade PBR

1.5.5
December 8, 2022
・Cutout Transparency: Fix flickering

1.5.4
November 16, 2022
・Fix some warnings

1.5.3
November 1, 2022
・Added Camera fade effect
・VR: Bug fix for view direction

1.5.2
October 30, 2022
・Environmental lights: Fix for Unity 2022.2

1.5.1
October 28, 2022
・Baked lightmaps: Fix for emissive lights

1.5.0
September 26, 2022
・Z-Offset: Fixed bug when used with Outline effect

1.4.9
September 7, 2022
・Reflection: Added support for mask with Specular Map
・Triplanar: Precision-related bug fix for lower end devices
・Bug fix for UV2 in Normal Map and Occlusion Map
・Bug fix for specular in baked lighting

1.4.8
August 30, 2022
・MatCap: Bug fix for scaled objects
・Reflection: Bug fix for scaled objects

1.4.7
August 24, 2022
・Transparency Cutout: Fixed to work with Transparency Mask
・Triplanar: Support for transparent texture

1.4.6
August 5, 2022
・URP Light Layers bug fix
・Flat shading bug fix on Unity 2021

1.4.5
July 20, 2022
・Added support for Light Layers in URP

1.4.4
July 5, 2022
・Mixed Lighting bug fixes

1.4.3
July 1, 2022
・Normal map: Added secondary normal map

1.4.2
June 14, 2022
・Anime style: Fixed light color not working

1.4.1
May 30, 2022
・Fixed occasional error when baking lightmaps

1.4.0
May 23, 2022
・Fixed bug with alpha cutout transparency when baking lightmaps
・Fixed bug with light culling mask

1.3.9
May 16, 2022
・Fixed bug with Outline thickness.
・Fixed bug with lights affecting objects not included in their culling mask.
・Added Depth pass in URP for compatibility with other effects.

1.3.8
May 9, 2022
・Fixed bug where GPU Instancing was not shown in Rendering options of non-pro verison.

1.3.7
April 21, 2022
・Diffuse: Added Mixed Lighting option to enable improved support for baked and realtime lighting

1.3.6
April 18, 2022
・Added support for Layers in Triplanar shader

1.3.5
April 14, 2022
・Fixed light culling mask in URP
・Fixed bug with lightmap baking if using Saturation

1.3.4
April 4, 2022
・Material converter: Improved to preserve more values

1.3.3
March 28, 2022
・Point lights: Fixed bug which stripped point lights from builds
・Reflection: Fixed bug when used only by itself and normal map
・Removed the menu to convert materials to OmniShade, and made this automatic on switching shaders.

1.3.2
March 21, 2022
・Added menu option to convert an existing material to use OmniShade shader.

1.3.1
March 17, 2022
・Triplanar: Fixed bug with alpha
・Added some tooltips

1.3.0
March 14, 2022
・Revamped UI with collapsable groups
・Added Desaturation effect
・Specular: Added override color
・Reflection: Revised to work independently of Rim, and to work with lighting
・Light Map: Renamed to Occlusion Map
・Triplanar: Support for using UV texture on sides

1.2.1
March 8, 2022
・Support for Single Pass Stereo rendering in VR

1.2.0
March 7, 2022
・Renamed to OmniShade Pro

1.1.6
March 7, 2022
・Fixed bug with outline effect on device builds

1.1.5
March 3, 2022
・Added Cutout Transparency support
・Added MatCap and Lightmap color

1.1.4
February 28, 2022
・Flat shading: Support for MatCap
・Specular, Rim, Emissive: Fixed additive light blending bug
・Specular Hair: Fixed light visible on backside
・Shadows: Added support for colored shadows
・Anime: Added support for color tints
・Added Outline feature, and removed Vertex Extrude
・Baked lighting: Fixed bug with baked specular intensity

1.1.3
February 25, 2022
・Added Optimization Flags to assist shader stripping.
・Added Flat Shading.
・Added Specular Hair.
・Fixed UV2.
・Plant sway: Improved sway motion.

1.1.2
February 23, 2022
・MatCap: Fixed wrong calculation in perspective correction in URP
・Plant sway: Fixed for URP
・Fixed light baking in URP

1.1.1
February 22, 2022
・Optimized point lights to vertex shader, with Per Pixel option
・Layers: Fix to respect layer alpha

1.1.0
February 21, 2022
・Added support for point lights.
・Added Anime-look styling.
・Added 5 more demo scenes.
・Added OpenGL ES2 fallback, with a minimal subset of features.
・Fixed shadows not working on Unity 2021.
・Added convenience toggle to Receive Shadows in shader.
・Fixed broken Vertex Color Contrast.
・HeightColors: Added more blending modes and texture input.
・Specular: Removed Specular Color, using the main light color instead.

1.0.1
February 16, 2022
・MatCap: Fixed bug which caused it to slide with perspective camera rotation.  Added option to disable perspective correction, and option to set a static rotation.
・Detail Map: Added option to mask with vertex color alpha-channel.
・Added Ella demo scene.

1.0.0
February 15, 2022
・OmniShade released.