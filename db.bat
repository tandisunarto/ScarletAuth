dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext

dotnet run /seedIDP
dotnet run /seedUsers