using Fisd.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Tests
{
    public static class TestDbContextFactory
    {
        // A fresh, isolated in-memory database per call - every test starts empty and controls
        // its own fixture data.
        public static FisdDbContext Create()
        {
            var options = new DbContextOptionsBuilder<FisdDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FisdDbContext(options);
        }
    }
}
