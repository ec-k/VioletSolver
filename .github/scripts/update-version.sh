#!/bin/bash

# Update package.json version from git tag
# Usage: update-version.sh <tag>
# Tag must be in format "v0.0.0" or "0.0.0"

TAG=$1

if [ -z "$TAG" ]; then
  echo "::warning::No tag provided, skipping version update"
  exit 0
fi

# Validate tag format (v0.0.0 or 0.0.0)
if [[ ! "$TAG" =~ ^v?[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
  echo "::warning::Tag '$TAG' is not in valid format (v0.0.0 or 0.0.0), skipping version update"
  exit 0
fi

# Remove 'v' prefix if present
VERSION=${TAG#v}

PACKAGE_JSON="Assets/VioletSolver/package.json"

if [ ! -f "$PACKAGE_JSON" ]; then
  echo "::error::$PACKAGE_JSON not found"
  exit 1
fi

jq --arg v "$VERSION" '.version = $v' "$PACKAGE_JSON" > tmp.json && mv tmp.json "$PACKAGE_JSON"
echo "Updated package.json version to ${VERSION}"
