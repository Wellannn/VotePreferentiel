# VotePreferentiel

Application .NET de calcul d'un résultat de vote préférentiel.

## Utilisation

```powershell
dotnet build
dotnet run --project VotePreferentiel.Tests
dotnet run --project VotePreferentiel.Worker
```

Le Worker récupère les votes depuis GitHub et affiche le gagnant.
