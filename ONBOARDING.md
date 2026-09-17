# FISD, onboarding technique

FISD (Forum International Solidarité et Développement) est un site public bilingue (FR/EN) pour un forum international réunissant décideurs, entrepreneurs et diaspora. Le site présente les éditions du forum (programme, horaire, intervenants), du contenu évergreen (piliers, participants, partenaires, actions d'engagement), un centre d'actualités, une galerie de souvenirs, et un espace éditorial `/admin` complet pour gérer tout ce contenu sans redéploiement.

La billetterie de l'événement est externalisée sur Zeffy (lien codé en dur dans `site-content.ts`), le site n'a pas de module e-commerce.

## Structure du dépôt

```
api/    Backend .NET (Fisd.Api, Fisd.Application, Fisd.Persistence, Fisd.Tests)
web/    Frontend Angular (standalone components)
Web.yml                     pipeline Azure DevOps du frontend
api/azure-pipelines-api.yml pipeline Azure DevOps du backend
```

### Backend (`api/`)

Architecture en couches classique, une solution `Fisd.slnx` avec 4 projets :

- **Fisd.Api** : hôte ASP.NET Core. Controllers, middleware, `Program.cs`, `appsettings*.json`, dossier `media/` (stockage local des fichiers uploadés).
- **Fisd.Application** : la couche services. `Services/`, `Email/`, `Security/` (JWT, Fido2, options), `Models/receive` (DTOs de requête) et `Models/result` (DTOs de réponse) par fonctionnalité.
- **Fisd.Persistence** : la couche EF Core. `FisdDbContext.cs`, `Entities/` (sous-dossiers par domaine : `Content`, `Diagnostics`, `Media`, `Security`, `Settings`, chacun avec sa `Configuration/`), `Migrations/`, `Enums/`.
- **Fisd.Tests** : xUnit + Moq + EF InMemory. Couverture actuelle limitée à `EngagementActions`, `Media`, `Participants`, `Partners`, `Pillars`, `Speakers`. Rien encore sur Auth, Editions/ProgramDays/ScheduleItems, Articles, Contacts, Souvenirs, Testimonials, Tickets, AdminUsers, AuditLogs, ErrorLogs, EmailHistory, SiteSettings, SystemHealth ou Slides : bon point de départ si tu cherches où contribuer des tests.

Entités principales (`Fisd.Persistence/Entities/`) :

- `Content/` : `Edition`, `ProgramDay`, `ScheduleItem` (programme lié à une édition), `Slide`, `Pillar`, `Participant`, `Speaker`, `Partner`, `EngagementAction`, `Article`, `Contact`, `Souvenir` + `SouvenirPhoto`, `Testimonial`.
- `Diagnostics/` : `AuditTrail` (journal automatique des changements), `ErrorLog`, `EmailSentHistory`, `HelpArticle` (centre d'aide interne), `Ticket` + `TicketAttachment` (tickets de bug/amélioration soumis par les admins eux-mêmes, sans rapport avec la billetterie de l'événement).
- `Media/` : `MediaFile`, une ligne par fichier uploadé. Pas de génération de vignettes/variantes, stockage brut du fichier original.
- `Security/` : `AdminUser`, `AdminPasskey`, `AdminUserInvitation`, `PasswordResetToken`.
- `Settings/` : `SiteSettings` (ex. durée d'affichage des slides).

`FisdDbContext` intercepte chaque `SaveChanges` pour écrire une trace dans `AuditTrail`, en excluant `AuditTrail` lui-même, `ErrorLog` et les tables de jetons de sécurité, et en masquant `PasswordHash`/`TwoFactorSecret` avant sérialisation.

**Base de données** : SQL Server, clé de config `ConnectionStrings:FisdDB`. Les migrations s'appliquent automatiquement au démarrage (`db.Database.Migrate()` dans `Program.cs`), pas d'étape séparée en CI.

**Authentification** : JWT (config `FisdAuth:Jwt`, clé symétrique), avec en plus un vrai support Passkey/Fido2 (`Fido2:ServerDomain`/`ServerName`/`Origins`) et 2FA TOTP (`Otp.NET`). La création de compte admin se fait uniquement par invitation (`AdminUsersController.Invite` puis lien envoyé par courriel), aucune inscription publique. `Login` répond toujours en HTTP 200 avec un contenu succès/échec dans le corps, plutôt qu'un code d'erreur HTTP, pour que le frontend distingue toujours une vraie erreur réseau d'un identifiant refusé.

**Rôles** (`AdminRoleEnum`) : `Editor` (contenu public), `SuperAdmin` (Editor + gestion des autres comptes admin), `PlatformAdmin` (aussi les comptes SuperAdmin, plus les pages diagnostics : journal d'erreurs, journal d'audit, santé système, historique des courriels, gestion du centre d'aide, gestion des tickets).

### Frontend (`web/`)

Angular avec composants standalone. `src/app/core/` contient un service/garde/intercepteur par préoccupation (un `*.service.ts` par type de contenu), `layout/` le header/footer, `shared/` les composants réutilisables (`shared/admin/` pour les widgets propres à l'admin), `pages/` les pages publiques et `pages/admin/` l'espace éditorial.

**Routes publiques** (toutes en français, aucune variante anglaise dans l'URL) : `/`, `/agir`, `/programmation` et `/programmation/:edition`, `/galerie`, `/actualites` et `/actualites/:id`, `/contact`, `/reservation`, `/conditions`. Changer de langue change le contenu affiché, pas l'URL.

**Espace admin** (`/admin`, protégé par `authGuard`) : articles, contacts, temoignages, souvenirs, editions, diaporama, piliers, participants, intervenants, partenaires, engagement, media, securite, users (garde `superAdminGuard`), journal-erreurs / journal-audit / sante-systeme / email-history / help/manage / tickets/manage (garde `platformAdminGuard`).

**Contenu résilient** : `core/content/site-content.ts` exporte des constantes statiques (`SLIDES`, `PILLARS`, `PARTICIPANTS`, `PARTNERS`, `SPEAKERS`, etc.). Les pages initialisent leurs signaux avec ces valeurs par défaut, affichent immédiatement, puis dans `ngOnInit` appellent l'API correspondante et n'écrasent le signal que si la réponse est non vide. Toute erreur d'appel est avalée silencieusement : le contenu statique ou déjà chargé reste affiché, jamais de page blanche. Reproduis ce patron pour toute nouvelle page qui consomme du contenu dynamique.

**Bilingue**, deux mécanismes distincts :
1. Texte d'interface statique : `@ngx-translate/core`, fichiers extraits dans `public/assets/i18n/fr.json`/`en.json` via `npm run extract-i18n`. `LocaleService` garde la langue courante en signal, la persiste dans `localStorage` (`fisd_lang`).
2. Contenu dynamique (venant de l'API) : résolu côté serveur selon l'en-tête `Accept-Language`, attaché à chaque requête par `localeInterceptor`. Comme la résolution se fait une fois côté serveur, `LocaleService.setLocale()` déclenche un rechargement complet de la page au changement de langue, plutôt qu'une simple retraduction du texte d'interface.

**Tests** : `npm test` (`ng test`), runner Vitest via le builder unifié Angular (`@angular/build:unit-test`), pas de Karma/Jasmine dans ce projet.

## Lancer le projet en local

**API** (`api/Fisd.Api`) : `dotnet run`, sert sur `http://localhost:5196` (`https://localhost:7265` en HTTPS). Secrets locaux via `dotnet user-secrets` (le projet a déjà un `UserSecretsId`) : `ConnectionStrings:FisdDB`, `FisdAuth:Jwt:Key`, identifiants SMTP, config Fido2. Ne jamais mettre ces valeurs dans `appsettings.json`, qui les garde vides intentionnellement pour la prod/CI. En Development, SQL Server local attendu sur l'instance nommée `SQLEXPRESS` avec authentification Windows (voir `appsettings.Development.json`).

**Web** (`web/`) : `npm install` puis `npm start` (`ng serve`), sert sur `http://localhost:4200`. `environment.ts` pointe déjà vers l'API locale.

## Générer les clés et remplir la configuration

Règle générale : `appsettings.json` (le fichier commité) garde toutes les valeurs sensibles vides, c'est le gabarit partagé par tout le monde. `appsettings.Development.json` (aussi commité) ne contient que des valeurs de dev sans danger si elles fuitent (connexion SQL en auth Windows, ports, origines CORS). Toute vraie valeur sensible passe par `dotnet user-secrets` en local, jamais dans un fichier du dépôt.

Le projet `Fisd.Api` a déjà un `UserSecretsId` dans son `.csproj`, donc les commandes suivantes fonctionnent directement depuis `api/Fisd.Api` :

```powershell
cd api/Fisd.Api

# Clé de signature JWT (HMAC-SHA256, prendre au moins 32 octets aléatoires)
$bytes = [System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32)
$jwtKey = [Convert]::ToBase64String($bytes)
dotnet user-secrets set "FisdAuth:Jwt:Key" $jwtKey

# Identifiants SMTP (Gmail en dev, voir appsettings.Development.json pour le serveur/port)
dotnet user-secrets set "Email:Senders:Generic:Email" "toncompte@gmail.com"
dotnet user-secrets set "Email:Senders:Generic:Password" "<mot de passe d'application Gmail>"
dotnet user-secrets set "Email:Senders:Generic:FromAddress" "no-reply@fisd.ca"
```

Points à savoir :

- La clé JWT n'a pas besoin d'être en base64 précisément, elle est utilisée comme une chaîne UTF-8 brute (`JwtTokenGenerator.cs`). Ce qui compte, c'est la longueur : moins de 32 octets et la validation de la librairie de tokens refuse la clé au démarrage. Une chaîne base64 de 32 octets aléatoires est une façon simple d'obtenir une longueur correcte.
- Pour Gmail, `Password` doit être un "mot de passe d'application" généré depuis le compte Google (nécessite la validation en deux étapes activée sur ce compte), pas le mot de passe du compte lui-même.
- `Email:Senders:Generic:FromAddress` est l'adresse affichée aux destinataires, `Email` est l'identifiant SMTP réel. Si absent, `FromAddress` retombe sur `Email` (`GlobalEmailService.cs`), donc en dev une seule adresse suffit.
- `ConnectionStrings:FisdDB` n'a besoin d'aucun secret en local : `appsettings.Development.json` utilise déjà l'authentification Windows contre l'instance `SQLEXPRESS` locale.
- `Fido2:ServerDomain`/`ServerName`/`Origins` sont déjà remplis pour `localhost` dans `appsettings.Development.json`, rien à ajouter en local. En dehors de Development, `Program.cs` refuse de démarrer si ces valeurs sont vides, pour éviter que chaque appel passkey échoue silencieusement avec une erreur peu claire.
- Pour un nouvel environnement (staging, prod) : les vraies valeurs ne vont ni dans `appsettings.json` ni dans les secrets utilisateur d'un poste de dev, elles vont dans les variables du pipeline Azure DevOps (`api/azure-pipelines-api.yml`), qui génère `appsettings.Integration.json` au moment du déploiement.

## Déploiement

Deux pipelines Azure DevOps séparés, tous deux déclenchés sur push vers la branche `Integration`, tous deux en déploiement FTP (pas de conteneur, pas d'App Service) :

- **`Web.yml`** (racine) : build Angular en configuration `beta`, génère un `web.config` IIS (réécriture SPA, en-têtes de sécurité), puis passe en mode maintenance (upload d'un `app_offline.htm`) avant de déployer le vrai build par-dessus.
- **`api/azure-pipelines-api.yml`** : `dotnet test` tourne en CI (le build échoue si les tests échouent), publie avec `EnvironmentName=Integration`, génère `appsettings.Integration.json` à partir des variables du pipeline (chaîne de connexion, clé JWT, origines CORS, identifiants courriel, config Fido2), déploie en mode maintenance FTP puis retire systématiquement `app_offline.htm` à la fin, même en cas d'échec du déploiement.

## Points à connaître avant de contribuer

- Le mécanisme de spinner de chargement (`loaderId` sur les requêtes, `LoadingService`/`loadingInterceptor`) existe mais n'est branché que sur une minorité des services HTTP. La plupart des appels admin n'affichent aujourd'hui aucun indicateur de chargement via ce mécanisme.
- Aucune route publique n'a de variante anglaise dans l'URL, seul le contenu change de langue.
- La bibliothèque média ne génère ni vignette ni variante d'image, uniquement le fichier original.
- Le lien de billetterie Zeffy et certaines dates par défaut sont codés en dur dans `site-content.ts`/`home.ts` (année de l'édition incluse dans l'URL), à mettre à jour manuellement pour chaque nouvelle édition.
