dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\Tenants.Web.pfx -p P@ssw0rd
copy to %USERPROFILE%\AppData\Roaming\ASP.NET\Https
dotnet dev-certs https --trust
