#!/bin/bash

set -e

MIGRATION_NAME=$1

if [ -z "$MIGRATION_NAME" ]; then
  echo "Usage: $0 <migration name>"
  exit 1
fi

dotnet ef migrations add "$MIGRATION_NAME" --context UsersDbContext --startup-project ../../../Identity.WebHost/ --output-dir Database/Migrations/