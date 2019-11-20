FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY . /publish

ENTRYPOINT ["dotnet", "Xms.Web.dll"]