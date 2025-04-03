# RouletteApp
This project was developed by Platon Matsinski as a part of the PlayTech internship selection program.

This is a desktop roulette simulator built with Avalonia UI and .NET 9.0.

## Features
 - Simulates roulette generates random results (numbers 0-36 with appropriate colors)
 - Displays last 10 results with color-coded UI
 - Calculates and displays multipliers for consecutive same-color results
 - Receives statistics via TCP connection (active players and biggest multiplier)
 - Shows notifications with auto-dismiss functionality

## Project Structure
 - Models: Core data structures and business logic
 - ViewModels: UI logic and command handling
 - Views: Avalonia UI components
 - Services: Background services like TCP listener
 - Commands: Command pattern implementation
 - Converters: Avalonia value converters
 - Tests: Unit tests using NUnit and Moq

## Requirements
 - .NET 9.0 SDK or newer

## Building and Running
### 1. Clone the repository or download and unpack zip
### 2. Navigate to project directory
    cd RouletteApp/RouletteApp
### 3. Build the project
    dotnet build
### 4. Run the application
    dotnet run

## Sending Statistics
The application listens for statistics on port 4948. You can send JSON data in this format:

    {
        "ActivePlayers": 10,
        "BiggestMultiplier": 25
    }

or

    {
        "ActivePlayers": 10
    }  

or

    {
        "BiggestMultiplier": 25
    }

