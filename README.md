# GymApp

GymApp är en konsolapplikation i C# för att logga träningspass, övningar och set. Projektet är byggt som en grund för att stegvis utveckla en mer komplett träningsapp med historik, progression och statistik.
gymapp
## Funktioner

- Lägg till egna övningar
- Starta ett träningspass
- Logga set med reps och vikt
- Se tidigare resultat, bästa set och total volym för en övning innan du kör den
- Spara träningsdata lokalt i JSON
- Visa övningar och hur ofta de har använts

### Historik

Historiken visar en rad per träningspass, nyaste först, med datum och en
sammanfattning: antal övningar, antal set och total volym.

```
==== WORKOUT HISTORY ====   (page 1 of 3)

  1. 2026-08-28 14:20   3 exercises, 12 sets, 4250 kg
  2. 2026-08-26 17:05   2 exercises, 8 sets, 2900 kg

n) Next page   0) Back
Choose a number for details:
```

Listan visar tio pass i taget och bläddras med `n` och `p`. Skriv numret på ett
pass för att se detaljerna — alla set per övning, plus volymen för varje övning.
Numreringen löper över hela listan, så samma pass har samma nummer oavsett
vilken sida du står på.

## Teknik

- C#
- .NET 8
- Konsolapplikation
- JSON som lokal datalagring

## Projektstruktur

Models innehåller dataklasser för övningar, träningspass och set.

Service innehåller logik för lagring, träningspass och framtida progression.

GymApplication.cs innehåller menyflödet och användarinteraktionen.

Program.cs startar applikationen.

## Så kör du projektet

1. Klona eller ladda ner projektet.
2. Öppna `GymApp.sln` i Visual Studio.
3. Kontrollera att .NET 8 är installerat.
4. Starta projektet med F5.

Alternativt kan projektet köras via terminalen:

```bash
dotnet run
