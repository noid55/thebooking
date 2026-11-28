# Subapp 2 – React-frontend + Web API

Subapp 2 består av to deler:

| Mappe | Hva | Port |
|---|---|---|
| `api/` | ASP.NET Core Web API (.NET 10, EF Core + SQLite) | `http://localhost:5234` |
| `thebooking-react/` | React 19 + Vite + TypeScript | `http://localhost:4000` |

## Kjøring

### Første gang: installer pakker
```
cd subapp2/thebooking-react
npm install
```

### Start både API og frontend
Fra `subapp2/thebooking-react`:
```
npm run dev
```
Åpne `http://localhost:4000` i nettleseren.

`npm run dev` bruker `concurrently` til å starte begge delene samtidig:
1. `npm run backend` → `dotnet run --project ../api/api.csproj` (API på port 5234)
2. `wait-on tcp:5234 && npm run frontend` → venter til API-et svarer, og starter så Vite på port 4000

Ctrl+C stopper begge.

### Alternativ: start hver del for seg (i to terminaler)
```
cd subapp2/api
dotnet run
```
```
cd subapp2/thebooking-react
npm run frontend
```
Da er loggene fra API og frontend lettere å skille når man feilsøker.

## Hvordan delene henger sammen
- **React → API:** `.env.development` setter `VITE_API_URL=http://localhost:5234`. `src/services/roomService.ts` bruker denne til å bygge URL-ene (f.eks. `/api/roomapi/roomlist`).
- **CORS:** Frontend (port 4000) og API (port 5234) er ulike *origins*. `Program.cs` i `api/` har en CORS-policy som tillater `localhost`, ellers ville nettleseren blokkert kallene.
- **API → database:** Ved oppstart kjører `Program.cs` `db.Database.Migrate()` mot `api/Booking.db`. Deretter legger `DAL/DBInit.cs` inn testdata hvis tabellene er tomme: 5 bygg (P32–P52), 4 typer utstyr og 7 grupperom med utstyr. Man trenger altså ikke kjøre `dotnet ef database update` manuelt.
- **Datalaget (DAL):** Controllerne bruker repositories (`DAL/I*Repository.cs`) i stedet for `BookingDbContext` direkte. De registreres i `Program.cs`. Domenereglene for booking ligger i `Services/BookingService.cs`.

### Etter at du har hentet nye endringer: slett gammel database
Migrasjonene ble samlet til én ny `InitialCreate` da datalaget ble lagt inn. Har du en `Booking.db` fra før, må den slettes før API-et startes, ellers krasjer det med `table "..." already exists`:
```
cd subapp2/api
rm Booking.db*
```
(I PowerShell: `Remove-Item Booking.db*`.) En ny database med testdata lages ved neste oppstart.

## Teste API-et uten frontend
Når API-et kjører: `http://localhost:5234/scalar/v1` (Scalar). Her kan man prøve endepunktene direkte – nyttig for å finne ut om en feil ligger i frontend eller backend.

## Vanlige feil
- **`'concurrently' is not recognized`** → `npm install` er ikke kjørt.
- **`address already in use` på 5234** → API-et kjører allerede (f.eks. fra Rider). Stopp det først.
- **Rommene lastes ikke** → Åpne DevTools (F12) → *Network*. Sjekk hvilken URL kallet gikk til og hvilken statuskode det fikk.
- **`table "..." already exists`** → gammel `Booking.db`. Slett `api/Booking.db*`, så lages en ny ved neste oppstart.

Se `DemoDokumentasjon/React/Demo-React-2-APIBackendAndDebugging.pdf` for mer om oppsettet med API-backend og debugging.
