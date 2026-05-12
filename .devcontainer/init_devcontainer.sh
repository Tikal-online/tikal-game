#!/bin/bash

set -e

echo "Setting ownership for .NET user secrets..."
sudo chown -R vscode:vscode /home/vscode/.microsoft/usersecrets

echo "Adding development CA certificate..."
sudo cp /.aspnet/dev-certs/trust/dotnet-dev-ca.pem /etc/ssl/certs/

echo "Updating certificate store..."
sudo update-ca-certificates

echo "Importing development certificate..."
dotnet dev-certs https --import /.aspnet/dev-certs/trust/dotnet-dev-cert.pfx --clean -p secret