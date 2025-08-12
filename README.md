Lastfm.API
A Blazor web app to display artist info from Last.fm API.

Project Overview
This project is a simple Artist Search application with a backend API and a frontend UI.
The backend fetches artist data from an external API and returns JSON responses.
The frontend allows users to search for artists and displays the results.

Technologies Used
Backend: ASP.NET Core Web API
Frontend: Blazor WebAssembly (Razor Pages)
External API: Last.fm API
Prerequisites
.NET 6 SDK or later installed
Visual Studio 2022 or VS Code
Internet connection to access the external Last.fm API
How to Run the Backend
Navigate to the backend project folder in your terminal or IDE.
Run the backend project using the command:
dotnet run
he backend server will start on http://localhost:5000 (or another port if configured).
You can test the API by navigating to http://localhost:5000/api/artist/{artistName} in your browser
Navigate to the frontend project folder in your terminal or IDE Run the frontend project using the command: dotnet run The frontend app will start and usually open at http://localhost:5123 (or another port).
