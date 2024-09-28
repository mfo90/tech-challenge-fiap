#!/bin/bash
set -e

# Initialize database
dotnet run --project AuthService.API.csproj &

# Wait for the app to initialize the database
sleep 30

# Kill the dotnet process
pkill dotnet
