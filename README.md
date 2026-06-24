# GymApp

GymApp är en konsolapplikation i C# för att logga träningspass, övningar och set. Projektet är byggt som en grund för att stegvis utveckla en mer komplett träningsapp med historik, progression och statistik.
gymapp
## Funktioner

- Lägg till egna övningar
- Starta ett träningspass
- Logga set med reps och vikt
- Spara träningsdata lokalt i JSON
- Visa tidigare träningspass
- Visa övningar och hur ofta de har använts

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
