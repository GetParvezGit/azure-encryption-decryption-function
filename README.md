# Azure.EncryptionDecryption.Function

## Overview

`Azure.EncryptionDecryption.Function` is an Azure Functions project (isolated .NET worker) that provides PGP-based encryption and decryption capabilities. This project demonstrates a secure PGP-based encryption and decryption solution built with Azure Functions and C#. It accepts input through HTTP request bodies, encrypts the data using a PGP public key, and decrypts encrypted payloads using the corresponding private key and passphrase.

## Features

- PGP encryption and decryption using environment-provided keys
- Runs as an Azure Function using the dotnet-isolated worker (.NET 9)
- Configurable via environment variables or `local.settings.json` for local development

## Prerequisites

- Visual Studio 2022 or later (with Azure development workload) or CLI:
- .NET 9 SDK
- Azure Functions Core Tools (v4+) for local development if using the Functions CLI
- Azurite or an Azure Storage account for `AzureWebJobsStorage`

## Configuration

The function expects configuration via environment variables. For local development, these go in `local.settings.json` under `Values` (do not commit secrets).

Required configuration keys:

- `AzureWebJobsStorage` - Storage connection string (for local use: `UseDevelopmentStorage=true` with Azurite)
- `FUNCTIONS_WORKER_RUNTIME` - should be `dotnet-isolated`
- `PGP_PUBLIC_KEY` - Base64 or armored PGP public key (use secure storage in production)
- `PGP_PRIVATE_KEY` - Base64 or armored PGP private key (use secure storage in production)
- `PGP_PASSPHRASE` - Passphrase for the private key

Example `local.settings.json` snippet (use placeholders, never check secrets in source control):

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "PGP_PUBLIC_KEY": "<BASE64_OR_ARMORED_PUBLIC_KEY_HERE>",
    "PGP_PRIVATE_KEY": "<BASE64_OR_ARMORED_PRIVATE_KEY_HERE>",
    "PGP_PASSPHRASE": "<PASSPHRASE_HERE>"
  }
}
```

For production, store keys in Azure Key Vault or another secret store and reference them from app settings.

## Local Development

1. Clone the repository.
2. Open the solution in Visual Studio 2022 or use the command line.
3. Install and run Azurite (or point `AzureWebJobsStorage` to a real storage account).
4. Populate `local.settings.json` with the configuration values using placeholders or test keys.

Run using Visual Studio (F5) or via CLI:

- From the function project folder:
  - `dotnet build`
  - `func start --verbose`

When debugging from Visual Studio, ensure the selected profile runs the Functions host.

## Build and Run

Build the solution:

- `dotnet build` (solution root)

Run the function locally:

- `func start` (from the function project directory)

Deploy to Azure (high-level steps):

1. Create an Azure Function App (Linux/Windows) using a supported runtime that targets .NET 9 isolated worker.
2. Configure application settings in the portal: `AzureWebJobsStorage`, `PGP_PUBLIC_KEY`, `PGP_PRIVATE_KEY`, `PGP_PASSPHRASE`, and `FUNCTIONS_WORKER_RUNTIME`.
3. Publish from Visual Studio or use `func azure functionapp publish <APP_NAME>`.

## Testing

If the repository contains unit tests, run them with:

- `dotnet test` (solution or test project folder)

Add unit and integration tests for PGP operations; mock key retrieval when testing key management logic.

## CI / CD

Use GitHub Actions, Azure DevOps, or other CI to build and test on push and create workflows for deploy. Typical steps:

- Checkout
- Setup .NET 9 SDK
- Restore / Build / Test
- Publish artifacts
- Deploy to Azure Function App (deploy token or publish profile stored securely)

Do not expose secret values in CI logs. Use repository secrets or Azure managed identities.

## Secrets & Security

- Never commit keys, passphrases, or `local.settings.json` with real secrets.
- Use Azure Key Vault, GitHub Secrets, or other secure secret stores for production keys.
- Enforce least privilege and rotate keys regularly.
- Validate and sanitize all inputs to the function to avoid injection or DoS vectors.

## Coding Standards

- This repository enforces project-level coding standards.
- Pull requests must adhere to the configured formatting and linting rules.

## Contributing

Follow contribution guidelines. In brief:

- Fork the repo and create feature branches
- Ensure builds and tests pass locally
- Open pull requests with clear descriptions and linked issues

## License

This project is provided under the MIT License. See `LICENSE` file for details. 

## Contact

For questions, open an issue or contact the repository maintainers.


This version maintains the original structure while ensuring clarity and coherence throughout the document. Each section is clearly defined, and the content flows logically from one topic to the next.