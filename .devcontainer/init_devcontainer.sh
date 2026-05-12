#!/bin/bash

set -e

echo "Setting ownership for .NET user secrets..."
sudo chown -R vscode:vscode /home/vscode/.microsoft/usersecrets

echo "Adding development CA certificate..."
sudo cp /dev-certs/dotnet-dev-ca.pem /etc/ssl/certs/

echo "Updating certificate store..."
sudo update-ca-certificates