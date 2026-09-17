using Fido2NetLib;
using Fisd.Application.Email;
using Fisd.Application.Security;
using Fisd.Application.Services;
using Fisd.Application.Site;
using Fisd.Persistence.Entities.Security;
using Fisd.Persistence.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using OtpNet;

namespace Fisd.Tests;

public class AuthServiceTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PendingQrSurvivesLoginRetryAndEnablesTwoFactor(bool spacedCode)
    {
        using var db = TestDbContextFactory.Create();
        var user = new AdminUserEntity { Id = Guid.NewGuid(), Email = "admin@example.test", PasswordHash = PasswordHasher.Hash("Test-password-123!"), Role = AdminRoleEnum.SuperAdmin, CreatedAt = DateTimeOffset.UtcNow };
        db.AdminUsers.Add(user);
        await db.SaveChangesAsync();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new AuthService(db,
            new JwtTokenGenerator(Options.Create(new JwtOptions { Key = new string('k', 64), Issuer = "tests", Audience = "tests" })),
            Mock.Of<IGlobalEmailService>(), Options.Create(new SiteOptions()), Mock.Of<IFido2>(), cache);
        var first = await service.LoginAsync(user.Email, "Test-password-123!");
        var retry = await service.LoginAsync(user.Email, "Test-password-123!");
        Assert.True(first.RequiresTwoFactorSetup);
        Assert.Equal(first.TwoFactorSecret, retry.TwoFactorSecret);
        Assert.Equal(first.TwoFactorQrUri, retry.TwoFactorQrUri);
        var rejected = await service.EnableTwoFactorAsync(user.Email, "Test-password-123!", "invalid");
        Assert.False(rejected.Success);
        Assert.False(user.TwoFactorEnabled);
        var code = new Totp(Base32Encoding.ToBytes(first.TwoFactorSecret!)).ComputeTotp();
        if (spacedCode) code = code.Insert(3, " ");
        var enabled = await service.EnableTwoFactorAsync(user.Email, "Test-password-123!", code);
        Assert.True(enabled.Success);
        Assert.Equal("SuperAdmin", enabled.Role);
        Assert.NotNull(enabled.Token);
        Assert.True(user.TwoFactorEnabled);
        var login = await service.LoginAsync(user.Email, "Test-password-123!");
        Assert.True(login.RequiresTwoFactor);
        Assert.False(login.RequiresTwoFactorSetup);
    }
}