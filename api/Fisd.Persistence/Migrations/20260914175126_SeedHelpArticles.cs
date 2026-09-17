using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedHelpArticles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var contentFr1 = "Cette interface vous permet de gérer tout le contenu du site FISD : diaporama d'accueil, piliers, participants, intervenants, partenaires, articles, témoignages, souvenirs et le programme de chaque édition.\n\nTrois rôles existent :\n- Éditeur : gère le contenu du site.\n- Super administrateur : gère aussi les comptes (inviter, changer un rôle, désactiver).\n- Administrateur plateforme : supervise l'ensemble (journal d'erreurs, journal d'audit, santé du système) et peut agir même sur les comptes Super administrateur.\n\nVotre rôle détermine ce que vous voyez dans le menu de gauche.";
            var contentEn1 = "This interface lets you manage all of the FISD site's content: homepage slideshow, pillars, participants, speakers, partners, articles, testimonials, souvenirs, and each edition's program.\n\nThere are three roles:\n- Editor: manages the site's content.\n- Super admin: also manages accounts (invite, change a role, deactivate).\n- Platform admin: oversees everything (error log, audit log, system health) and can act even on Super admin accounts.\n\nYour role determines what you see in the left-hand menu.";

            var contentFr2 = "Chaque élément de contenu (diaporama, pilier, article, etc.) a un statut : Brouillon ou Publié.\n\nUn élément en Brouillon n'apparaît jamais sur le site public, même si vous l'avez enregistré. Cela vous permet de préparer du contenu à l'avance, de le relire, sans risque qu'il apparaisse trop tôt.\n\nUn élément Publié apparaît immédiatement sur le site dès que vous l'enregistrez, à condition que son interrupteur « Visible » soit aussi activé — le statut et la visibilité sont deux réglages distincts : le statut contrôle si c'est prêt, la visibilité si vous voulez le montrer maintenant.";
            var contentEn2 = "Every piece of content (slide, pillar, article, etc.) has a status: Draft or Published.\n\nAn item in Draft never appears on the public site, even after you save it. This lets you prepare content ahead of time and proofread it without any risk of it showing up too early.\n\nA Published item appears on the site as soon as you save it, provided its \"Visible\" toggle is also on - status and visibility are two separate settings: status controls whether it's ready, visibility controls whether you want to show it right now.";

            var contentFr3 = "Le site est bilingue. Chaque formulaire de contenu a deux onglets, Français et English : les deux doivent être remplis avant d'enregistrer.\n\nLe visiteur voit automatiquement la version dans sa langue courante — vous n'avez rien d'autre à faire une fois les deux onglets remplis. Si vous oubliez une langue, l'enregistrement est bloqué avec un message d'erreur.";
            var contentEn3 = "The site is bilingual. Every content form has two tabs, Français and English: both must be filled in before saving.\n\nVisitors automatically see the version in their current language - there's nothing else to do once both tabs are filled in. If you forget a language, saving is blocked with an error message.";

            var contentFr4 = "Dans les formulaires qui ont une image (diaporama, piliers, intervenants, etc.), cliquez sur le champ image et choisissez un fichier JPG, PNG ou WebP de 5 Mo maximum. L'image s'affiche en aperçu avant l'enregistrement — elle n'est réellement envoyée que lorsque vous enregistrez le formulaire.";
            var contentEn4 = "In forms that have an image (slideshow, pillars, speakers, etc.), click the image field and pick a JPG, PNG or WebP file, 5 MB maximum. The image shows a preview before saving - it's only actually uploaded once you save the form.";

            var contentFr5 = "La page Programme liste toutes les éditions du FISD. Une seule peut être « courante » à la fois : c'est celle dont le contenu (compte à rebours, intervenants) s'affiche sur la page d'accueil.\n\nPour changer d'édition courante, ouvrez la liste des éditions et cliquez sur « Définir comme courante » sur la nouvelle édition — l'ancienne perd automatiquement ce statut.";
            var contentEn5 = "The Program page lists every FISD edition. Only one can be \"current\" at a time: it's the one whose content (countdown, speakers) shows on the homepage.\n\nTo change the current edition, open the editions list and click \"Set as current\" on the new edition - the previous one automatically loses that status.";

            var contentFr6 = "La double authentification (2FA) est obligatoire pour tous les comptes administrateurs. À votre première connexion, un code QR vous permet de la configurer avec une application comme Google Authenticator ou Authy.\n\nVous pouvez aussi ajouter une clé d'accès (passkey) — empreinte, visage ou code de votre appareil — depuis la page Sécurité, pour vous connecter sans mot de passe.\n\nSi vous perdez l'accès à votre authentificateur : un administrateur plateforme peut réinitialiser votre 2FA. Si vous êtes vous-même administrateur plateforme, vous pouvez le faire vous-même depuis la page Sécurité.";
            var contentEn6 = "Two-factor authentication (2FA) is mandatory for every admin account. On your first sign-in, a QR code lets you set it up with an app like Google Authenticator or Authy.\n\nYou can also add a passkey - your device's fingerprint, face or PIN - from the Security page, to sign in without a password.\n\nIf you lose access to your authenticator: a platform admin can reset your 2FA. If you're a platform admin yourself, you can do it yourself from the Security page.";

            var contentFr7 = "Depuis la page Utilisateurs (réservée aux Super administrateurs et Administrateurs plateforme), cliquez sur « Inviter un compte », entrez son courriel et choisissez son rôle. La personne reçoit un courriel avec un lien valide 48 heures pour choisir son mot de passe et activer son compte — vous n'avez jamais à définir ou communiquer un mot de passe vous-même.\n\nSeul un administrateur plateforme peut attribuer le rôle Administrateur plateforme.";
            var contentEn7 = "From the Users page (restricted to Super admins and Platform admins), click \"Send invitation\", enter their email and choose their role. They receive an email with a 48-hour link to choose their password and activate their account - you never need to set or share a password yourself.\n\nOnly a platform admin can assign the Platform admin role.";

            var contentFr8 = "Réservées aux administrateurs plateforme, trois pages complètent la supervision du site :\n- Journal des erreurs : chaque erreur serveur, capturée automatiquement.\n- Journal d'audit : chaque modification de contenu ou de compte, avec qui, quand et les valeurs avant/après.\n- Santé du système : un coup d'œil sur l'état de la base de données, les erreurs récentes et les comptes actifs.";
            var contentEn8 = "Restricted to platform admins, three pages round out site oversight:\n- Error log: every server error, captured automatically.\n- Audit log: every content or account change, with who, when, and the before/after values.\n- System health: a glance at the database's state, recent errors, and active accounts.";

            migrationBuilder.Sql($"""
                IF NOT EXISTS (SELECT 1 FROM [HelpArticles])
                BEGIN
                    INSERT INTO [HelpArticles] ([Id], [Category], [TitleFr], [TitleEn], [ContentFr], [ContentEn], [IsVisible], [DisplayOrder])
                    VALUES
                    (NEWID(), N'Premiers pas', N'Bienvenue dans l''administration du FISD', N'Welcome to the FISD admin', N'{Esc(contentFr1)}', N'{Esc(contentEn1)}', 1, 1),
                    (NEWID(), N'Contenu', N'Brouillon et publié', N'Draft and published', N'{Esc(contentFr2)}', N'{Esc(contentEn2)}', 1, 2),
                    (NEWID(), N'Contenu', N'Contenu bilingue (français / anglais)', N'Bilingual content (French / English)', N'{Esc(contentFr3)}', N'{Esc(contentEn3)}', 1, 3),
                    (NEWID(), N'Contenu', N'Ajouter une image', N'Adding an image', N'{Esc(contentFr4)}', N'{Esc(contentEn4)}', 1, 4),
                    (NEWID(), N'Programme', N'L''édition courante', N'The current edition', N'{Esc(contentFr5)}', N'{Esc(contentEn5)}', 1, 5),
                    (NEWID(), N'Sécurité', N'Double authentification et clés d''accès', N'Two-factor authentication and passkeys', N'{Esc(contentFr6)}', N'{Esc(contentEn6)}', 1, 6),
                    (NEWID(), N'Comptes', N'Inviter un nouvel administrateur', N'Inviting a new admin', N'{Esc(contentFr7)}', N'{Esc(contentEn7)}', 1, 7),
                    (NEWID(), N'Supervision', N'Journal d''erreurs, journal d''audit et santé du système', N'Error log, audit log and system health', N'{Esc(contentFr8)}', N'{Esc(contentEn8)}', 1, 8);
                END
                """);
        }

        // Doubles single quotes for safe embedding inside a T-SQL N'...' literal.
        private static string Esc(string value) => value.Replace("'", "''");

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
