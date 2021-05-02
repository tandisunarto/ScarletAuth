# ScarletAuth

- dotnet ef migrations add InitialSchema -o Migrations/ConfigurationDb -c ConfigurationDbContext
- dotnet ef migrations add InitialSchema -o Migrations/PersistedGrantDb -c PersistedGrantDbContext
- dotnet ef migrations add InitialSchema -o Migrations/Identity -c ApplicationDbContext

- dotnet ef database update -c ConfigurationDbContext
- dotnet ef database update -c PersistedGrantDbContext
- dotnet ef database update -c ApplicationDbContext