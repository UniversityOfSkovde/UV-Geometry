# UV Geometry
Generate cubes inside Unity with UV-maps that scale with the object for 
consistent texture detail across the scene.

![Image showing the rendered vectors in the Scene View](https://media.githubusercontent.com/media/UniversityOfSkovde/UV-Geometry/main/Documentation~/uv-cubes.gif)

## Installation
1. Make sure you have the following software installed:
* Git
* Git LFS
* Git Flow
2. Open Unity and open the Package Manager (located under `Window -> Package Manager`)
3. Press the `+` icon and select `Add Package From git URL...`
4. Enter the URL: `https://github.com/UniversityOfSkovde/UV-Geometry.git` and press `Add`
5. You can now start using the package!

## Usage
Create an empty game object in the scene and add the "UV Cube" component to it. It should generate a Mesh Filter and Mesh Renderer automatically.

**OBS:** The default material is made for the Universal Render Pipeline. The scripts are not specific to that pipeline however so it works in both Standard and HDRP as well, as long as you supply your own materials.

**Tip:** If you mark game objects with the `UVCube` component as *Static*, Unity can usually batch them together which is good for rendering performance.

## License
```
Copyright 2020-2021 Emil Forslund

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
```
