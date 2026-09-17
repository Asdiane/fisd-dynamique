namespace Fisd.Persistence.Enums
{
    // Editor manages public-facing content (articles, contacts, testimonials, souvenirs).
    // SuperAdmin does everything Editor does, plus manages other admin accounts.
    // PlatformAdmin can also act on SuperAdmin accounts (role, status, 2FA, deletion).
    public enum AdminRoleEnum
    {
        Editor,
        SuperAdmin,
        PlatformAdmin
    }
}
