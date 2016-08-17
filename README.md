# Vertices Engine

The Vertices Engine is an in-house developed engine based in built from the ground up in C#. It comes compatible with both XNA and MonoGame libraries (although 3D is still in process of being ported over) for use as the back end allowing it to be multiplatform across PC, MacOS, Linux, iOS, and Android.

# Status

| Platform | Build Status                   |
|----------|--------------------------------|
| XNA      | ![alt text][buildstatusXNA]    |
| DirectX  | ![alt text][buildstatusDirctX] |
| Windows (OpenGL)   | ![alt text][buildstatusGL]     |
| OSX (OpenGL)   | ![alt text][buildstatusGL]     |
| Linux (OpenGL)   | ![alt text][buildstatusGL]     |
| Android  | ![alt text][buildstatusDroid]  |
| iOS  | ![alt text][buildstatusIOS]  |

[buildstatusXNA]: https://img.shields.io/badge/build-passing-brightgreen.svg
[buildstatusDirctX]: https://img.shields.io/badge/build-tbd-blue.svg
[buildstatusGL]: https://img.shields.io/badge/build-passing-brightgreen.svg
[buildstatusDroid]: https://img.shields.io/badge/build-passing-brightgreen.svg
[buildstatusIOS]: https://img.shields.io/badge/build-tbd-blue.svg

# Features
## Real-time Surface and Water Reflections
![Reflections](https://virtexedgedesign.files.wordpress.com/2015/10/reflections.png)

## Crepuscular Rays(God Rays)
![Crepuscular Rays](https://virtexedgedesign.files.wordpress.com/2015/10/godrays.png)

## Depth of Field
![Depth of Field](https://farm2.staticflickr.com/1476/25396320090_422ec688b0_z.jpg)

## Cascade Shadow Mapping
![Cascade Shadow Mapping](https://virtexedgedesign.files.wordpress.com/2015/10/shadowmaps.png)

## Extendable in-game Sandbox for Rapid Level design
![Sandbox](https://virtexedgedesign.files.wordpress.com/2015/10/sandbox.png)

## Integrated Debug System, Viewer and Console
![Debug](https://virtexedgedesign.files.wordpress.com/2015/10/debug.png)

## Customizable and Sinkable GUI system
![GUI](https://virtexedgedesign.files.wordpress.com/2015/10/gui.png)

# Other features
* Modular design allowing for smaller and more efficient 2D and 3D applications.
* Touch screen, gamepad and traditional keyboard-mouse support.

# 3rd Party Library Integration
* 3D Physics through platform independent fork of BEPU Physics.
* 2D Physics provided by Farseer Physics library.
* Networking support through Lidgren Networking Library.
