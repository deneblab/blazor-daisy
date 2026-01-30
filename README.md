# Deneblab.BlazorDaisy

A Blazor Server application template with DaisyUI and TailwindCSS styling.

## Features

- **Blazor Server** (.NET 10) with SSR and InteractiveServer components
- **DaisyUI + TailwindCSS** for modern, responsive styling
- **Authentication** - Cookie-based, OAuth (OIDC), or Guest mode
- **Navigation** - Collapsible sidebar with menu service
- **Theming** - Light/Dark mode with persistence
- **Snackbar Notifications** - Toast-style notification system
- **NLog Logging** - Structured logging configuration
- **Error Handling** - Global error handling middleware

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for TailwindCSS)

### Installation

```bash
# Clone the repository
git clone https://github.com/DenebLab/balzor-daisy.git
cd balzor-daisy

# Restore .NET dependencies
dotnet restore src/Deneblab.BlazorDaisy.sln

# Install npm packages
cd src/Deneblab.BlazorDaisy
npm install

# Run the application
dotnet run
```

The application will be available at `https://localhost:5001`

## Quick Start (Use as Template)

```powershell
# Windows
.\setup.ps1 -ProjectName "MyApp"

# Linux/Mac
./setup.sh MyApp
```

This renames the solution and project files. Namespaces remain unchanged.

## Project Structure

```
src/Deneblab.BlazorDaisy/
├── Areas/
│   ├── Template/           # Template infrastructure (don't modify)
│   │   ├── Components/     # Snackbar
│   │   ├── Infrastructure/ # Logging, Middleware
│   │   ├── Pages/          # Layouts, Login, Error pages
│   │   └── Services/       # Auth, Navigation, App services
│   ├── Dashboard/          # Example: Dashboard area
│   └── Example/            # Example: Feature area (delete when done)
├── Components/             # Your app components
│   ├── App.razor
│   └── Routes.razor
├── Styles/                 # TailwindCSS source
└── wwwroot/                # Static files
```

## Configuration

Configuration is managed via `appsettings.json`:

```json
{
  "App": {
    "Name": "Your App Name"
  },
  "Authentication": {
    "Mode": "Cookie",
    "AllowGuestMode": true
  },
  "Features": {
    "Dashboard": true,
    "Settings": true
  }
}
```

See `TEMPLATE.md` for detailed configuration options.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
