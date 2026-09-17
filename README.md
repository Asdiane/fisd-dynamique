# fisd-dynamique

Site du Forum International Solidarité et Développement (FISD).

## Structure

- `api/` : API ASP.NET Core, services, persistance Entity Framework Core et tests.
- `web/` : site Angular bilingue et administration.
- `ONBOARDING.md` : installation, configuration et démarrage local.
- `CORRECTIONS-SITE.md` : modifications réalisées et contenus restant à fournir.

Les secrets doivent être configurés hors du dépôt, via les secrets utilisateur .NET en développement et les variables de déploiement sur le serveur. Les données de la base locale et les médias téléversés ne sont pas inclus.