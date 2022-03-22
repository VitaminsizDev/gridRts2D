# GridRts2D
<h3>Made by Soykan Göksel Kamal in 2.5 days</h3>


<h1>Gameplay Video</h1>
https://user-images.githubusercontent.com/58806238/159464947-8f80acec-554c-45a0-b94e-c30672ac3f4c.mp4

<h1>Brief Information</h1>
-This is a 2D grid based rts game demo<br>
-Created in Unity 2020.3.30(LTS)<br>
-Algorithms Used: A* pathfinding, Grid based movement, Grid build system<br>
-Other features: Infinite scrollview, animated Ui, post processing, build sound


<h1>Design Choices</h1>

-Observer pattern: <br>
* Visual and functional code bodies are seperated. Functional code bodies work with or without the visual scripts and they are not aware if they exist or not. Visual and functional bodies comminucate only through events.<br>
* Code is easily extendible with the usage of events.<br><br>

-Singleton: <br>
* Managers implement singleton design pattern. Their static instances are reachable when needed.<br><br>

-Factory: <br>
* Used in grid creation. Grid system is generic, grid nodes are generic and they create their own instances.

<h1>Gameplay Gif</h1>
<img src="https://github.com/VitaminsizDev/gridRts2D/blob/main/Gifs/introGif.gif" alt="c#" width="1280" height="720"/> <br/>
