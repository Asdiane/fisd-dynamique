using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHelpArticlePlatformAdminOnlyFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPlatformAdminOnly",
                table: "HelpArticles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // The PlatformAdmin role is deliberately never mentioned to other admins - hide the
            // oversight article from the general reading list, and reword the two articles that
            // used to name the role so Editor/SuperAdmin readers never learn it exists.
            migrationBuilder.Sql("UPDATE [HelpArticles] SET [IsPlatformAdminOnly] = 1 WHERE [Category] = N'Supervision';");

            migrationBuilder.Sql(@"UPDATE [HelpArticles] SET
                [ContentFr] = N'Cette interface vous permet de gérer tout le contenu du site FISD : diaporama d''accueil, piliers, participants, intervenants, partenaires, articles, témoignages, souvenirs et le programme de chaque édition.

Deux rôles existent :
- Éditeur : gère le contenu du site.
- Super administrateur : gère aussi les comptes (inviter, changer un rôle, désactiver).

Votre rôle détermine ce que vous voyez dans le menu de gauche.',
                [ContentEn] = N'This interface lets you manage all of the FISD site''s content: homepage slideshow, pillars, participants, speakers, partners, articles, testimonials, souvenirs, and each edition''s program.

There are two roles:
- Editor: manages the site''s content.
- Super admin: also manages accounts (invite, change a role, deactivate).

Your role determines what you see in the left-hand menu.'
                WHERE [TitleFr] = N'Bienvenue dans l''administration du FISD';");

            migrationBuilder.Sql(@"UPDATE [HelpArticles] SET
                [ContentFr] = N'Depuis la page Utilisateurs (réservée aux Super administrateurs), cliquez sur « Inviter un compte », entrez son courriel et choisissez son rôle. La personne reçoit un courriel avec un lien valide 48 heures pour choisir son mot de passe et activer son compte - vous n''avez jamais à définir ou communiquer un mot de passe vous-même.

Si vous renvoyez une invitation à la même adresse, l''ancien lien est automatiquement invalidé - seul le plus récent fonctionne.',
                [ContentEn] = N'From the Users page (restricted to Super admins), click ""Send invitation"", enter their email and choose their role. They receive an email with a 48-hour link to choose their password and activate their account - you never need to set or share a password yourself.

If you resend an invitation to the same address, the previous link is automatically invalidated - only the most recent one works.'
                WHERE [TitleFr] = N'Inviter un nouvel administrateur';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPlatformAdminOnly",
                table: "HelpArticles");
        }
    }
}
