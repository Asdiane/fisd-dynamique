using System.ComponentModel.DataAnnotations;
namespace Fisd.Application.Models.Editorial;
public class EditorialContent : IValidatableObject
{
    public Dictionary<string, string> Fr { get; set; } = new();
    public Dictionary<string, string> En { get; set; } = new();
    public List<HomeSection> Sections { get; set; } = new();
    public List<EditorialStat> Stats { get; set; } = new();
    public string ThemeImage { get; set; } = "assets/images/concertations.jpeg";
    public string ReportUrl { get; set; } = "";
    public string NewsletterUrl { get; set; } = "";
    public string TikTokUrl { get; set; } = "";
    public string YouTubeUrl { get; set; } = "";
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Fr == null || En == null || Sections == null || Stats == null ||
            Sections.Any(x => x == null || x.Buttons == null || x.Id == null || x.Kind == null) ||
            Stats.Any(x => x == null || x.LabelFr == null || x.LabelEn == null) ||
            Fr.Concat(En).Any(x => x.Value == null))
        { yield return new ValidationResult("Contenu incomplet."); yield break; }
        if (Fr.Count > 500 || En.Count > 500 || Fr.Concat(En).Any(x => x.Key.Length > 100 || x.Value.Length > 12000))
            yield return new ValidationResult("Les textes dépassent les limites autorisées.");
        if (Sections.Count > 30 || Stats.Count > 20 || Sections.Select(x => x.Id).Distinct().Count() != Sections.Count)
            yield return new ValidationResult("Sections ou statistiques invalides.");
        if (Stats.Any(x => x.Value < 0 || x.Value > 1000000000 || x.LabelFr.Length > 150 || x.LabelEn.Length > 150))
            yield return new ValidationResult("Les chiffres doivent être positifs et leurs libellés courts.");
        foreach (var url in new[] { ReportUrl, NewsletterUrl, TikTokUrl, YouTubeUrl, ThemeImage })
            if (url == null || url.Length > 2048 || !IsSafeUrl(url)) yield return new ValidationResult("Utilisez une adresse HTTPS ou un chemin local.");
        var builtIn = new[] { "theme", "program", "speakers", "stats", "pillars", "testimonials", "partners", "custom" };
        foreach (var section in Sections)
        {
            if (!builtIn.Contains(section.Kind) || section.Id.Length > 100 || section.Buttons.Count > 10)
                yield return new ValidationResult("Section invalide.");
            if (section.Buttons.Any(b => b == null || b.Path == null || !b.Path.StartsWith('/') || b.Path.StartsWith("//") || b.Path.Contains('\\')))
                yield return new ValidationResult("Les boutons doivent pointer vers une page du site.");
        }
    }
    private static bool IsSafeUrl(string value) => string.IsNullOrEmpty(value) ||
        (value.StartsWith('/') && !value.StartsWith("//") && !value.Contains('\\')) ||
        value.StartsWith("assets/") || (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == "https");
}
public class HomeSection
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "custom";
    public bool Visible { get; set; } = true;
    public string TitleFr { get; set; } = "";
    public string TitleEn { get; set; } = "";
    public string TextFr { get; set; } = "";
    public string TextEn { get; set; } = "";
    public List<EditorialButton> Buttons { get; set; } = new();
}
public class EditorialButton
{
    public string LabelFr { get; set; } = "";
    public string LabelEn { get; set; } = "";
    public string Path { get; set; } = "/";
}
public class EditorialStat
{
    public double Value { get; set; }
    public string Suffix { get; set; } = "";
    public string LabelFr { get; set; } = "";
    public string LabelEn { get; set; } = "";
}
