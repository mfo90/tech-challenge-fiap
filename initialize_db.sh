#!/bin/bash
set -e

# Initialize database
dotnet run --project api/AuthService.API/AuthService.API.csproj &

# Wait for the app to initialize the database
sleep 30

# Kill the dotnet process
pkill dotnet
