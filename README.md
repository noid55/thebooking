# This is not a booking readme file

Currently this is the best project in the world, please give us A+++ thank you 🥰


## Kjøring av webappen
Naviger til folder ```cd C:\Skole\Webapp\thebooking\src\thebooking```

Kjør kommando ```dotnet watch```
Eller ```dotnet run```

Naviger til ```http://localhost:5190/Room/table``` i nettleser.

Nå har vi også en Homepage, så man kan navigere seg rundt på webapplikasjonen ved hjelp av GUI etter ```dotnet watch```.

## Home page og shared view 
Under [thebooking/src/thebooking/Views/shared/_Layout.cshtml](./src/thebooking/Views/Shared/_Layout.cshtml) ligger _layout.cshtml filen. 
Her lagres et felles shared layout, aner ikke hvordan det funker. 
Jeg tror at den setter inn det samme layoutet på alle sidene, og i _Layout.cshtml har den kode som bare viser en vilkårlig view basert på hvilken side man velger.
Men det gir oss hvertfall en navbar som dukker opp på alle de forksjellige sidene av applikasjonen.
Vi har også laget en "Home" page som er default når applikasjonen starter (altså den vises på root directory ```http://localhost:5190``). 

Denne ligger lagret under [thebooking/src/thebooking/Views/Home/Index.cshtml](./src/thebooking/Views/Home/Index.cshtml). Her definers hjemmesiden, den er per nå veldig enkel og tjener ingen funksjon. Men kult uansett? 😁💪

Vi har også implementert en slags felles import fil? Aner ikke hvorfor eller hva fordelene er. 
Såvidt jeg forstår det importerer den felles på alle filene vi lagrer?  
``` 
@using thebooking
@using thebooking.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```
Det var en del ting under denne biten som var litt forvirrende og ikke like enkelt å forklare. 
Se [Demo-ShopMVC-2-HomeView-Navigation.pdf](DemoDokumentasjon/Demo-ShopMVC-2-HomeView-Navigation.pdf) for mer informasjon om enten startsiden, shared views eller imports.

## thebooking.slnx
Kjørt samtaler med AI ift å bygge prosjektstruktur, og den sier at det er kjekt å ha en solution fil (.slnx). 
Den gir prosjektet en slaks oversikt over hvilke løsninger som er implementert, usikker på hvor nødvendig det er for våres prosjekt men vi ser an. 
Legg til forskjellige solution filer med f.eks:

```dotnet sln thebooking.slnx add src/thebooking/thebooking.csproj``` 

Se om løsningen er lagt til filen riktig med: 
```dotnet sln thebooking.slnx list```


# Frontend notater

Features: 
* Oversikt over rom som er ledige
  * Mulighet for å se romkapasitet
  * Hva slaks type rom det er
* Reserve et rom
* Filtreringsmulighet/Søk (velge bygning, rom, tidspunkt, plasser)
* Se hvilke rombestillinger man har gjort
* Kalender (mulighet for å velge dager/uker/månder)

Grensesnitt å ta inspirasjon fra (Read: Straight up kopier dette)
https://booking.oslo.kommune.no/?loanType=singleLoan&purposes=&date=2026-09-16&datePicker1=2026-09-16&duration=PT1H&searchType=list
![img_1.png](img_1.png)