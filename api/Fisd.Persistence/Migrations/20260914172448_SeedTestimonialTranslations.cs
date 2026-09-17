using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestimonialTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Testimonials])
                BEGIN
                    INSERT INTO [Testimonials] ([Id], [AuthorName], [AuthorRoleFr], [AuthorRoleEn], [ContentFr], [ContentEn], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), N'Marie Veillette', N'Caravane Philanthrope', N'Caravane Philanthrope',
                        N'Merci infiniment aux organisatrices et organisateurs de ce forum ! Quel bel événement rempli d''apprentissage et d''échanges enrichissants. Pour un organisme comme le nôtre Caravane Philanthrope qui vise des pratiques éthiques en coopération internationale, ce genre de moment est vraiment précieux. J''espère que ce ne sera pas le dernier et que d''autres occasions comme celle-ci continueront d''inspirer et de rassembler. Chapeau pour votre merveilleuse organisation.',
                        N'Thank you so much to the organizers of this forum! What a wonderful event, full of learning and enriching exchanges. For an organization like ours, Caravane Philanthrope, which aims for ethical practices in international cooperation, moments like this are truly precious. I hope this won''t be the last, and that other opportunities like this one will continue to inspire and bring people together. Hats off to your wonderful organization.',
                        1, 'Published', 1),
                    (NEWID(), N'Benie Kouyate', N'Participante', N'Participant',
                        N'Je suis particulièrement reconnaissante d''avoir eu l''opportunité de partager mes idées et d''échanger avec des participants aussi passionnés et engagés. Je vous remercie également pour l''accueil chaleureux et l''organisation remarquable de cet événement. Je suis convaincue que le FISD continuera à jouer un rôle crucial dans la promotion de la solidarité et du développement à l''échelle internationale.',
                        N'I am especially grateful to have had the opportunity to share my ideas and exchange with participants who are so passionate and engaged. I also thank you for the warm welcome and the remarkable organization of this event. I am convinced that FISD will continue to play a crucial role in promoting solidarity and development on an international scale.',
                        1, 'Published', 2),
                    (NEWID(), N'Valérie Phaneuf', N'Participante', N'Participant',
                        N'Merci pour cette belle opportunité de mettre de l''avant des entrepreneurs sociaux et de discuter de grands enjeux de coopération internationale ! Quel bel événement ! Merci à tous les organisateurs et participants ! Merci Alpha Touré pour l''invitation !',
                        N'Thank you for this wonderful opportunity to showcase social entrepreneurs and discuss major international cooperation issues! What a great event! Thank you to all the organizers and participants! Thank you Alpha Touré for the invitation!',
                        1, 'Published', 3);
                END
                """);

            migrationBuilder.Sql("UPDATE [Testimonials] SET [AuthorRoleEn] = N'Caravane Philanthrope', [ContentEn] = N'Thank you so much to the organizers of this forum! What a wonderful event, full of learning and enriching exchanges. For an organization like ours, Caravane Philanthrope, which aims for ethical practices in international cooperation, moments like this are truly precious. I hope this won''t be the last, and that other opportunities like this one will continue to inspire and bring people together. Hats off to your wonderful organization.' WHERE [AuthorName] = N'Marie Veillette';");
            migrationBuilder.Sql("UPDATE [Testimonials] SET [AuthorRoleEn] = N'Participant', [ContentEn] = N'I am especially grateful to have had the opportunity to share my ideas and exchange with participants who are so passionate and engaged. I also thank you for the warm welcome and the remarkable organization of this event. I am convinced that FISD will continue to play a crucial role in promoting solidarity and development on an international scale.' WHERE [AuthorName] = N'Benie Kouyate';");
            migrationBuilder.Sql("UPDATE [Testimonials] SET [AuthorRoleEn] = N'Participant', [ContentEn] = N'Thank you for this wonderful opportunity to showcase social entrepreneurs and discuss major international cooperation issues! What a great event! Thank you to all the organizers and participants! Thank you Alpha Touré for the invitation!' WHERE [AuthorName] = N'Valérie Phaneuf';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
