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

echo "Installing Neovim v0.12.5"
curl -LO https://github.com/neovim/neovim/releases/download/v0.12.5/nvim-linux-x86_64.tar.gz
sudo rm -rf /opt/nvim-linux-x86_64
sudo tar -C /opt -xzf nvim-linux-x86_64.tar.gz
sudo rm -rf nvim-linux-x86_64.tar.gz
sudo rm -rf nvim-linux-x86_64
echo "export PATH="$PATH:/opt/nvim-linux-x86_64/bin"" >> /home/vscode/.bashrc

echo "Installing Neovim dependencies"
sudo apt update
sudo apt install -y software-properties-common
sudo add-apt-repository -y universe
sudo apt install -y ripgrep fd-find xclip

sudo bash -c 'cat > /etc/default/locale' <<EOF
LANG=en_GB.UTF-8
EOF

