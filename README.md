# P2PBank

P2PBank is a peer-to-peer banking server application built in C#. It provides a web-based interface for showing the server status. The application is modular, using dependency injection.

---

## Features

- Modular architecture with dependency injection
- TCP server for handling client connections
- Web monitoring server for real-time status and management
- Configurable database connection using Microsoft SQL
- Support for default and custom configuration files
- Command-based application layer for account and transaction management
- Logging system for operational monitoring and error reporting

---

## Requirements

- .NET 8.0 or later (required for building from source)
- Microsoft SQL Server
- Access to read/write configuration and log files

---

## Installation

### Using Prebuilt Releases

Prebuilt binaries are available for Windows and Linux:

- **Windows:** `p2p_bank.exe`
- **Linux:** `p2p_bank`  

Download the appropriate version from the [GitHub Releases](https://github.com/tadeas223/branka/releases) page.

### Building from Sourcehttps://github.com/tadeas223/branka

1. Clone the repository:

   ```bash
   git clone https://github.com/tadeas223/branka
   cd branka/src
   ```

2. Build the project using .NET CLI:

   ```bash
   dotnet build
   ```

3. Ensure the configuration directory contains the following files (or use defaults):

   - `ServerConfig.config`
   - `DatabaseConfig.config`

---

## Configuration

P2PBank supports both default and custom configurations:

- **ServerConfig.config** – Defines TCP server settings.
- **DatabaseConfig.config** – Defines database connection and credentials.

If no custom configurations are provided, the application will use built-in default configurations.

---

## Usage

### Starting the Server

Run the application with the following syntax:

```bash
./p2p_bank [config directory path] [log file path]

# example
./p2p_bank ./config ./logs/server.log
```

- `config directory path` – Path to the directory containing `ServerConfig.config` and `DatabaseConfig.config`.
- `log file path` – Path to the log file where operational messages will be written.

**Default Configuration Mode:**

```bash
./p2p_bank default ServerConfig.config
./p2p_bank default DatabaseConfig.config
```

This prints the default configuration for the specified file.

---

### Command Layer

The application uses a command dispatcher to process banking commands asynchronously. The following commands are registered by default:

- `AbCommand`, `AcCommand`, `AdCommand`, `ArCommand`, `AwCommand`
- `BaCommand`, `BcCommand`, `BnCommand`, `ErCommand`

Commands are executed by connected TCP clients through the server interface.

---

### Monitoring

The web monitoring server runs by default on port `8080` and provides server status, client load, current balance.

You can monitor server activity in a browser:

```
http://localhost:8080
```

---

### Logging

All operational messages, warnings, and errors are logged to the file specified when starting the application.
