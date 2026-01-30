#!/bin/bash
#
# Setup script to rename this Blazor template project to your own project name.
# Renames the project folder and .csproj file.
# Namespaces in code files are NOT changed - they remain as Deneblab.BlazorDaisy.
#
# Usage:
#   ./setup-project.sh                    # Interactive mode
#   ./setup-project.sh MyApp              # With project name
#   ./setup-project.sh --no-cleanup MyApp # Skip cleanup of template files

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Colors
CYAN='\033[0;36m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

step() { echo -e "${CYAN}>> $1${NC}"; }
success() { echo -e "${GREEN}   $1${NC}"; }
warn() { echo -e "${YELLOW}   $1${NC}"; }

# Current names
OLD_NAME="Deneblab.BlazorDaisy"

# Parse arguments
NO_CLEANUP=false
PROJECT_NAME=""

while [[ $# -gt 0 ]]; do
    case $1 in
        --no-cleanup)
            NO_CLEANUP=true
            shift
            ;;
        *)
            if [ -z "$PROJECT_NAME" ]; then
                PROJECT_NAME="$1"
            fi
            shift
            ;;
    esac
done

# Read from template.json if exists and no parameters provided
TEMPLATE_JSON="$SCRIPT_DIR/src/$OLD_NAME/template.json"
if [ -f "$TEMPLATE_JSON" ] && [ -z "$PROJECT_NAME" ]; then
    if command -v jq &> /dev/null; then
        TEMPLATE_NAME=$(jq -r '.project.name // empty' "$TEMPLATE_JSON")
        if [ -n "$TEMPLATE_NAME" ] && [ "$TEMPLATE_NAME" != "MyNewProject" ]; then
            PROJECT_NAME="$TEMPLATE_NAME"
            step "Using settings from template.json"
        fi
    fi
fi

# Prompt if still not provided
if [ -z "$PROJECT_NAME" ]; then
    echo ""
    echo -e "${MAGENTA}=== Blazor DaisyUI Template Setup ===${NC}"
    echo ""
    echo -e "${YELLOW}This script will rename:${NC}"
    echo "  - Project folder"
    echo "  - .csproj file"
    echo ""
    echo -e "${YELLOW}Namespaces in code files will NOT be changed.${NC}"
    echo ""
    read -p "Enter new project name (e.g., MyApp): " PROJECT_NAME
    if [ -z "$PROJECT_NAME" ]; then
        echo -e "${RED}Project name is required.${NC}"
        exit 1
    fi
fi

# Validate project name
if [[ ! "$PROJECT_NAME" =~ ^[a-zA-Z0-9.]+$ ]]; then
    echo -e "${RED}Project name can only contain letters, numbers, and dots.${NC}"
    exit 1
fi

echo ""
echo -e "${YELLOW}Configuration:${NC}"
echo "  Old Name:  $OLD_NAME"
echo "  New Name:  $PROJECT_NAME"
echo ""

read -p "Proceed with rename? (y/N): " confirm
if [[ "$confirm" != "y" && "$confirm" != "Y" ]]; then
    echo -e "${YELLOW}Aborted.${NC}"
    exit 0
fi

echo ""

# Step 1: Update solution file references
step "Updating solution file..."
SLN_PATH="$SCRIPT_DIR/src/$OLD_NAME.sln"
if [ -f "$SLN_PATH" ]; then
    if [[ "$OSTYPE" == "darwin"* ]]; then
        sed -i '' "s/$OLD_NAME/$PROJECT_NAME/g" "$SLN_PATH"
    else
        sed -i "s/$OLD_NAME/$PROJECT_NAME/g" "$SLN_PATH"
    fi
    success "Updated solution references"
fi

# Step 2: Rename .csproj file
step "Renaming project file..."
CSPROJ_PATH="$SCRIPT_DIR/src/$OLD_NAME/$OLD_NAME.csproj"
if [ -f "$CSPROJ_PATH" ]; then
    mv "$CSPROJ_PATH" "$SCRIPT_DIR/src/$OLD_NAME/$PROJECT_NAME.csproj"
    success "Renamed to $PROJECT_NAME.csproj"
fi

# Step 3: Rename project folder
step "Renaming project folder..."
OLD_FOLDER="$SCRIPT_DIR/src/$OLD_NAME"
NEW_FOLDER="$SCRIPT_DIR/src/$PROJECT_NAME"
if [ -d "$OLD_FOLDER" ]; then
    mv "$OLD_FOLDER" "$NEW_FOLDER"
    success "Renamed folder to $PROJECT_NAME"
fi

# Cleanup
if [ "$NO_CLEANUP" = false ]; then
    step "Cleaning up template files..."

    TEMPLATE_JSON_NEW="$SCRIPT_DIR/src/$PROJECT_NAME/template.json"
    if [ -f "$TEMPLATE_JSON_NEW" ]; then
        rm -f "$TEMPLATE_JSON_NEW"
        success "Removed template.json"
    fi

    TEMPLATE_MD="$SCRIPT_DIR/src/$PROJECT_NAME/TEMPLATE.md"
    if [ -f "$TEMPLATE_MD" ]; then
        rm -f "$TEMPLATE_MD"
        success "Removed TEMPLATE.md"
    fi

    warn "Note: Run 'rm setup-project.ps1 setup-project.sh' to remove setup scripts"
fi

echo ""
echo -e "${GREEN}=== Setup Complete ===${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "  1. cd src/$PROJECT_NAME"
echo "  2. dotnet restore"
echo "  3. dotnet run"
echo ""
