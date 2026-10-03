#!/bin/bash

set -e

echo "Setting ownership for .NET user secrets..."
sudo chown -R vscode:vscode /home/vscode/.microsoft/usersecrets

echo "Setting ownership for dataprotection keys..."
sudo chown -R vscode:vscode /home/vscode/.aspnet/DataProtection-Keys

echo "Adding development CA certificate..."
sudo cp /.aspnet/dev-certs/dotnet-dev-ca.crt /usr/local/share/ca-certificates/

echo "Updating certificate store..."
sudo update-ca-certificates

echo "Installing required dotnet sdk version"
curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --jsonfile global.json
rm dotnet-install.sh

echo "Importing development certificate..."
dotnet dev-certs https --import /.aspnet/dev-certs/dotnet-dev-cert.pfx --clean -p secret

echo "Extracting crt and key"
openssl pkcs12 -in /.aspnet/dev-certs/dotnet-dev-cert.pfx -clcerts -nokeys -out /.aspnet/dev-certs/cert.crt -passin pass:'secret'
openssl pkcs12 -in /.aspnet/dev-certs/dotnet-dev-cert.pfx -nocerts -out /.aspnet/dev-certs/key.key -passin pass:'secret' -nodes
