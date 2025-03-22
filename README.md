# Mission 3A - Application de Gestion GSB

Cette application Windows Forms permet de gérer les rapports de visite, médecins et médicaments pour GSB.

## Prérequis

- Visual Studio 2019 ou supérieur
- .NET Framework 4.7.2 ou supérieur
- SQL Server / SQL Server Express

## Installation

1. Clonez le dépôt sur votre machine locale
   ```
   git clone https://github.com/ZombieEvil/Mission-3-Version-A.git
   ```

2. Ouvrez la solution `mission3A.sln` dans Visual Studio

3. Restaurez les packages NuGet nécessaires
   - Clic droit sur la solution dans l'explorateur de solutions
   - Sélectionnez "Restaurer les packages NuGet"

4. Assurez-vous que la connexion à la base de données est correctement configurée dans `App.config`

## Lancement de l'application

1. Dans Visual Studio, appuyez sur F5 ou cliquez sur le bouton "Démarrer" pour lancer l'application en mode debug
2. Alternativement, vous pouvez générer l'application (Build > Build Solution) puis exécuter le fichier `.exe` généré dans le dossier `bin/Debug` ou `bin/Release`

## Fonctionnalités

- Gestion des rapports de visite
- Gestion des médecins
- Gestion des médicaments
- Visualisation des rapports
- Export des rapports au format XML 