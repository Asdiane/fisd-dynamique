using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedSlideTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A prior content-seed migration existed at one point but was dropped when the
            // migration history got squashed into InitialCreate - the Slides table may be
            // completely empty on a freshly-migrated database. Reseed the site's 5 homepage
            // slides here, this time bilingual from the start.
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Slides])
                BEGIN
                    INSERT INTO [Slides] ([Id], [ImageUrl], [VideoUrl], [FocalPoint], [KickerFr], [KickerEn], [TitleFr], [TitleEn], [TextFr], [TextEn], [PlaceFr], [PlaceEn], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), 'assets/images/marraine-fisd-2026.jpg', NULL, 'center 18%',
                        N'Marraine du FISD 2026', N'2026 FISD Honorary Patroness',
                        N'Son Excellence Madame Kandia Kamissoko Camara', N'Her Excellency Madame Kandia Kamissoko Camara',
                        N'Présidente du Sénat de la Côte d''Ivoire. Figure majeure du leadership panafricain, elle soutient l''éducation, la coopération internationale et la diaspora. Son soutien accompagne les actions de sensibilisation et de plaidoyer du FISD en faveur des diasporas.',
                        N'President of the Senate of Côte d''Ivoire. A leading figure in Pan-African leadership, she champions education, international cooperation and the diaspora. Her support underpins FISD''s advocacy and awareness efforts on behalf of the diaspora.',
                        N'Côte d''Ivoire', N'Côte d''Ivoire', 1, 'Published', 1),
                    (NEWID(), 'assets/images/parrain-fisd-2026.jpeg', NULL, 'center 22%',
                        N'Parrain du FISD 2026', N'Honorary Patron of FISD 2026',
                        N'Son Excellence le Maréchal Mahamat Idriss Deby Itno', N'His Excellency Marshal Mahamat Idriss Deby Itno',
                        N'Président de la République du Tchad, Président d''honneur du FISD 2025 et Parrain du FISD 2026.',
                        N'President of the Republic of Chad, Honorary President of FISD 2025 and Patron of FISD 2026.',
                        N'Tchad', N'Chad', 1, 'Published', 2),
                    (NEWID(), 'assets/images/sl1.jpeg', NULL, NULL,
                        N'Prochaine édition du FISD', N'The next FISD edition',
                        N'Édition 2026 : trois jours d''envergure internationale', N'2026 edition: three days on an international scale',
                        N'Les 26, 27 et 28 novembre 2026, le Forum International Solidarité et Développement revient pour 3 jours d''immersion, de rencontres et de retombées stratégiques.',
                        N'On November 26, 27 and 28, 2026, the Forum International Solidarité et Développement returns for three days of immersion, encounters, and strategic outcomes.',
                        NULL, NULL, 1, 'Published', 3),
                    (NEWID(), 'assets/images/sl2.jpeg', NULL, NULL,
                        N'26, 27 et 28 novembre 2026', N'November 26, 27 and 28, 2026',
                        N'Connecter les acteurs d''ici et d''ailleurs', N'Connecting stakeholders from here and abroad',
                        N'Institutions, diasporas, entrepreneurs, partenaires et acteurs du développement international se retrouveront autour d''une vision commune.',
                        N'Institutions, diaspora communities, entrepreneurs, partners and international development actors will come together around a shared vision.',
                        NULL, NULL, 1, 'Published', 4),
                    (NEWID(), 'assets/images/sl3.jpeg', NULL, NULL,
                        N'FISD 2026', N'FISD 2026',
                        N'Trois jours de rencontres et d''échanges', N'Three days of encounters and exchange',
                        N'Panels, rencontres ciblées, réseautage et activités post-forum : retrouvez les acteurs de la solidarité et du développement. Le lieu sera annoncé prochainement.',
                        N'Panels, targeted meetings, networking and post-forum activities: join the actors of solidarity and development. The venue will be announced soon.',
                        NULL, NULL, 1, 'Published', 5);
                END
                """);

            // If the table already had these 5 slides (e.g. the previous migration's French
            // fallback just ran), overwrite their English fields with the real translations
            // above instead of leaving the French-language fallback in place.
            migrationBuilder.Sql("UPDATE [Slides] SET [KickerEn] = N'2026 FISD Honorary Patroness', [TitleEn] = N'Her Excellency Madame Kandia Kamissoko Camara', [TextEn] = N'President of the Senate of Côte d''Ivoire. A leading figure in Pan-African leadership, she champions education, international cooperation and the diaspora. Her support underpins FISD''s advocacy and awareness efforts on behalf of the diaspora.', [PlaceEn] = N'Côte d''Ivoire' WHERE [TitleFr] = N'Son Excellence Madame Kandia Kamissoko Camara';");
            migrationBuilder.Sql("UPDATE [Slides] SET [KickerEn] = N'Honorary Patron of FISD 2026', [TitleEn] = N'His Excellency Marshal Mahamat Idriss Deby Itno', [TextEn] = N'President of the Republic of Chad, Honorary President of FISD 2025 and Patron of FISD 2026.', [PlaceEn] = N'Chad' WHERE [TitleFr] = N'Son Excellence le Maréchal Mahamat Idriss Deby Itno';");
            migrationBuilder.Sql("UPDATE [Slides] SET [KickerEn] = N'The next FISD edition', [TitleEn] = N'2026 edition: three days on an international scale', [TextEn] = N'On November 26, 27 and 28, 2026, the Forum International Solidarité et Développement returns for three days of immersion, encounters, and strategic outcomes.' WHERE [TitleFr] = N'Édition 2026 : trois jours d''envergure internationale';");
            migrationBuilder.Sql("UPDATE [Slides] SET [KickerEn] = N'November 26, 27 and 28, 2026', [TitleEn] = N'Connecting stakeholders from here and abroad', [TextEn] = N'Institutions, diaspora communities, entrepreneurs, partners and international development actors will come together around a shared vision.' WHERE [TitleFr] = N'Connecter les acteurs d''ici et d''ailleurs';");
            migrationBuilder.Sql("UPDATE [Slides] SET [KickerEn] = N'FISD 2026', [TitleEn] = N'Three days of encounters and exchange', [TextEn] = N'Panels, targeted meetings, networking and post-forum activities: join the actors of solidarity and development. The venue will be announced soon.' WHERE [TitleFr] = N'Trois jours de rencontres et d''échanges';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
