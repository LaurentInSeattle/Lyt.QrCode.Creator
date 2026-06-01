# Lyt.QrCode.Creator

Avalonia Application to create QR Codes based on the Lyt.QrCode library: https://github.com/LaurentInSeattle/Lyt.QrCode 

# Almost there...

<p align="left"><img src="Screenshots/Screenshot 2026-05-31 105825.png" height="600"/>

Camera (Webcam) support is only partially implemented, for Windows only. The "Camera" tab is still a work in progress. 

<p align="left"><img src="Screenshots/Screenshot 2026-05-31 163123.png" height="600"/>

# Localization

- Coming soon... 

# Build it... Windows ONLY for now.

- Clone this repo'
- => Clone the "Lyt.Framework" repo' side by side. (https://github.com/LaurentInSeattle/Lyt.Framework)
- => Clone the "Lyt.Avalonia" repo' side by side. (https://github.com/LaurentInSeattle/Lyt.Avalonia)
- => Clone the "Lyt.Video" repo' side by side. (https://github.com/LaurentInSeattle/Lyt.Video)
- Open the solution in Visual Studio, restore nugets, then clean and build.

Developed and tested with .Net 10, Visual Studio 2026 18.4 and Avalonia 12.0.4.

Does not build yet with Jet Brains Rider. Windows ONLY for now.

# Dependencies

- Avalonia (Skia) 12
- Lyt.QrCode library: https://github.com/LaurentInSeattle/Lyt.QrCode (Nuget package: Lyt.QrCode)
- Microsoft Dependency Injection and Hosting Framework
- Microsoft Community Toolkit MVVM Framework
- Microsoft CSharp Scripting Framework