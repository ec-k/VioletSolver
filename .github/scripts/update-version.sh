#!/bin/bash

# Update package.json version and README URLs from git tag
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

# Ensure tag has 'v' prefix for URL
TAG_WITH_V="v${VERSION}"

PACKAGE_JSON="Assets/VioletSolver/package.json"

if [ ! -f "$PACKAGE_JSON" ]; then
  echo "::error::$PACKAGE_JSON not found"
  exit 1
fi

jq --arg v "$VERSION" '.version = $v' "$PACKAGE_JSON" > tmp.json && mv tmp.json "$PACKAGE_JSON"
echo "Updated package.json version to ${VERSION}"

# Update README URLs with version tag
REPO_URL="https://github.com/ec-k/VioletSolver.git?path=/Assets/VioletSolver"

for README in README.md README_jp.md; do
  if [ -f "$README" ]; then
    sed -i "s|${REPO_URL}\(#v[0-9.]*\)\?|${REPO_URL}#${TAG_WITH_V}|g" "$README"
    echo "Updated ${README} URLs with #${TAG_WITH_V}"
  fi
done
