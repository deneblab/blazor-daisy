# Contributing to Deneblab.BlazorDaisy

Thank you for your interest in contributing! This document provides guidelines for contributing to the project.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for TailwindCSS/DaisyUI)

### Setup

1. Clone the repository
   ```bash
   git clone https://github.com/DenebLab/balzor-daisy.git
   cd balzor-daisy
   ```

2. Restore dependencies
   ```bash
   dotnet restore src/Deneblab.BlazorDaisy.sln
   ```

3. Install npm packages (for CSS processing)
   ```bash
   cd src/Deneblab.BlazorDaisy
   npm install
   ```

4. Run the application
   ```bash
   dotnet run --project src/Deneblab.BlazorDaisy
   ```

## Development Workflow

1. Create a new branch from `main`
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Make your changes

3. Ensure the project builds
   ```bash
   dotnet build src/Deneblab.BlazorDaisy.sln
   ```

4. Commit your changes with a clear message
   ```bash
   git commit -m "feat: add your feature description"
   ```

5. Push and create a Pull Request

## Commit Message Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` - New features
- `fix:` - Bug fixes
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting, etc.)
- `refactor:` - Code refactoring
- `test:` - Adding or updating tests
- `chore:` - Maintenance tasks

## Code Style

- Follow existing code patterns in the project
- Use meaningful variable and method names
- Keep components focused and single-purpose

## Questions?

Feel free to open an issue if you have questions or need clarification.
