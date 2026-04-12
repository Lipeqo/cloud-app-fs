# Instrukcja uruchomienia i deploymentu - wersja po zadaniu 7 i 8

## 1. Co zostało zmienione

### Zadanie 7 - bezpieczeństwo
- backend odczytuje `DbConnectionString` z Azure Key Vault,
- konfiguracja produkcyjna nie trzyma hasła bazy w repozytorium,
- App Service ma działać z Managed Identity,
- CORS jest oparty o listę dozwolonych adresów.

### Zadanie 8 - testowanie i automatyzacja
- dodano projekt testowy xUnit,
- dodano test `NewTask_ShouldNotBeCompleted`,
- dodano dwa workflow GitHub Actions,
- dopracowano endpoint aktualizacji i usuwania zadania,
- frontend ma czytelniejszy interfejs oraz przycisk `Usuń`.

## 2. Co trzeba podmienić przy innej bazie, haśle i parametrach

### Frontend
Plik: `frontend/.env`
- `VITE_API_URL` - adres backendu, np. `https://twoj-backend.azurewebsites.net/api`

### Backend lokalnie
Plik: `backend/appsettings.Development.json`
- `ConnectionStrings:DefaultConnection` - lokalny connection string
- `Cors:AllowedOrigins` - lista frontendów lokalnych lub testowych

### Backend w Azure App Service
Application Settings:
- `KeyVaultName` - nazwa Key Vault
- `Cors__AllowedOrigins__0` - adres produkcyjnego frontendu
- `Cors__AllowedOrigins__1` - opcjonalnie adres środowiska testowego

### Azure Key Vault
Sekret:
- `DbConnectionString` - pełny connection string do Azure SQL lub innej docelowej bazy zgodnej z SQL Server

Przykład:
```text
Server=tcp:twoj-serwer.database.windows.net,1433;Initial Catalog=twoja-baza;Persist Security Info=False;User ID=twoj-login;Password=twoje-haslo;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## 3. Jak uruchomić lokalnie

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

### Testy
```bash
cd backend
dotnet test Tests/CloudBackend.Tests.csproj
```

## 4. Jak wdrożyć na Azure

### Frontend - Azure Static Web Apps
1. Utwórz zasób Static Web Apps.
2. W GitHub Secrets dodaj `AZURE_STATIC_WEB_APPS_API_TOKEN`.
3. Dodaj `VITE_API_URL` z adresem backendu.
4. Wypchnij kod do `main`.
5. Workflow `frontend-staticwebapp.yml` zbuduje i wdroży frontend.

### Backend - Azure App Service
1. Utwórz App Service dla .NET 9.
2. Włącz System Assigned Managed Identity.
3. W Azure Key Vault nadaj aplikacji rolę lub politykę pozwalającą czytać sekrety.
4. Dodaj w App Service ustawienie `KeyVaultName`.
5. Dodaj ustawienia CORS przez `Cors__AllowedOrigins__0` itd.
6. W GitHub Secrets dodaj:
   - `AZURE_BACKEND_APP_NAME`
   - `AZURE_BACKEND_PUBLISH_PROFILE`
7. Wypchnij kod do `main`.
8. Workflow `backend-appservice.yml` wykona build, test i deploy.

### Baza danych - Azure SQL
1. Utwórz SQL Server i bazę danych.
2. Dodaj regułę firewalla dla Azure Services lub konkretnego IP.
3. Utwórz sekret `DbConnectionString` w Key Vault.
4. Po starcie backend wykona `Database.Migrate()` i utworzy tabele z migracji.

## 5. Jak przygotować screeny do artefaktu 7

Zgodnie z checklistą artefaktu 7 trzeba pokazać: utworzenie sekretu, kod korzystający z Key Vault, Managed Identity, działającą aplikację po zmianach i aktualizację GitHuba.

## 6. Jak przygotować screeny do artefaktu 8

Zgodnie z checklistą artefaktu 8 trzeba pokazać: kod testu, wynik `dotnet test`, konfigurację automatyzacji, push test po zmianie, przycisk `Usuń`, oraz finalną dokumentację na GitHub.

## 7. Ograniczenia

W tym pakiecie dodałem kod, konfigurację i dokumentację. Ręczne czynności po stronie Twojego konta Azure i GitHub musisz wykonać samodzielnie, bo wymagają logowania do Twoich usług i przygotowania screenów do zaliczenia.
