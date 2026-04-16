# Cloud Task Manager (2026) by Filip Szatkowski 97155

Projekt aplikacji chmurowej w architekturze 3-warstwowej:
- frontend: React 19 + Vite
- backend: ASP.NET Core 9 Web API
- baza danych: Azure SQL / SQL Server

## Mapowanie na Azure

| Warstwa | Technologia lokalna | Usługa Azure |
| --- | --- | --- |
| Frontend | React 19 + Vite | Azure Static Web Apps |
| Backend | ASP.NET Core 9 Web API | Azure App Service |
| Data | SQL Server | Azure SQL Database |
| Sekrety | zmienne środowiskowe / User Secrets | Azure Key Vault |

## Status artefaktów

- [x] Artefakt 1 - architektura i struktura projektu
- [x] Artefakt 2 - środowisko lokalne i Docker
- [x] Artefakt 3 - frontend
- [x] Artefakt 4 - backend + REST API + baza
- [x] Artefakt 5 - migracje i DTO
- [x] Artefakt 6 - podstawowe wdrożenie do Azure
- [x] Artefakt 7 - zabezpieczenie aplikacji: Azure Key Vault + Managed Identity + usunięcie sekretów z kodu
- [x] Artefakt 8 - test jednostkowy, CI/CD GitHub Actions, przycisk Usuń, zaktualizowana dokumentacja

## Co zostało dodane w zadaniu 7

1. Integracja backendu z Azure Key Vault przez `DefaultAzureCredential`.
2. Odczyt connection stringa z sekretu `DbConnectionString`.
3. Konfiguracja CORS oparta o listę `Cors:AllowedOrigins`.
4. Usunięcie jawnego hasła produkcyjnego z konfiguracji repozytorium.
5. Instrukcja użycia Managed Identity w Azure App Service.



## Szybki start lokalny

### Frontend
```bash
cd frontend
cp .env.example .env
npm ci
npm run dev
```

### Backend
```bash
cd backend
dotnet restore
dotnet run
```

Domyślnie frontend oczekuje API pod adresem zapisanym w `VITE_API_URL`.

## Konfiguracja lokalna

### Frontend
Plik `frontend/.env` lub `frontend/.env.example`:
```env
VITE_API_URL=https://twoj-backend.azurewebsites.net/api
```

### Backend
Plik `backend/appsettings.Development.json` zawiera przykładowe lokalne ustawienie:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CloudAppDb;User Id=sa;Password=CHANGE_ME_LOCAL_ONLY;TrustServerCertificate=True"
  }
}
```

Na produkcji backend ma pobierać sekret `DbConnectionString` z Azure Key Vault.

## Testy

```bash
cd backend
dotnet test Tests/CloudBackend.Tests.csproj
```

## CI/CD - wymagane sekrety GitHub

### Backend workflow
- `AZURE_BACKEND_APP_NAME`
- `AZURE_BACKEND_PUBLISH_PROFILE`

### Frontend workflow
- `AZURE_STATIC_WEB_APPS_API_TOKEN`
- `VITE_API_URL`

## Azure - wymagane ustawienia

### App Service
- `KeyVaultName=<nazwa-twojego-key-vault>`
- `Cors__AllowedOrigins__0=https://<twoj-frontend>.azurestaticapps.net`

### Key Vault
- sekret `DbConnectionString`

## Dokumentacja

Szczegółowa instrukcja deploymentu i aktualizacji konfiguracji znajduje się w:
- `docs/Azure_Deployment_Guide_PL.md`
- `Azure_deployment_audyt_cloud_task_manager_v2.docx`
