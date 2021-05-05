# ScarletAuth

- dotnet ef migrations add InitialSchema -o Migrations/ConfigurationDb -c ConfigurationDbContext
- dotnet ef migrations add InitialSchema -o Migrations/PersistedGrantDb -c PersistedGrantDbContext
- dotnet ef migrations add InitialSchema -o Migrations/Identity -c ApplicationDbContext

- dotnet ef database update -c ConfigurationDbContext
- dotnet ef database update -c PersistedGrantDbContext
- dotnet ef database update -c ApplicationDbContext
- 

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x86\makecert" -n "CN=ScarletAuth" -a sha256 -sv IdentityServer4Auth.pvk -r IdentityServer4Auth.cer

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x86\pvk2pfx" -pvk IdentityServer4Auth.pvk -spc IdentityServer4Auth.cer -pfx IdentityServer4Auth.pfx