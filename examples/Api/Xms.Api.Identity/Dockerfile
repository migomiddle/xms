FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Api/Xms.Api.Identity/Xms.Api.Identity.csproj", "Api/Xms.Api.Identity/"]
COPY ["Api/Xms.Api.Core/Xms.Api.Core.csproj", "Api/Xms.Api.Core/"]
COPY ["Presentation/Xms.Web.FrameWork/Xms.Web.Framework.csproj", "Presentation/Xms.Web.FrameWork/"]
COPY ["Libraries/Logging/Xms.Logging.AppLog/Xms.Logging.AppLog.csproj", "Libraries/Logging/Xms.Logging.AppLog/"]
COPY ["Libraries/Logging/Xms.Logging.Abstractions/Xms.Logging.Abstractions.csproj", "Libraries/Logging/Xms.Logging.Abstractions/"]
COPY ["Libraries/Xms.Context/Xms.Context.csproj", "Libraries/Xms.Context/"]
COPY ["Libraries/Localization/Xms.Localization.Abstractions/Xms.Localization.Abstractions.csproj", "Libraries/Localization/Xms.Localization.Abstractions/"]
COPY ["Libraries/Xms.Core/Xms.Core.csproj", "Libraries/Xms.Core/"]
COPY ["Libraries/Xms.Infrastructure/Xms.Infrastructure.csproj", "Libraries/Xms.Infrastructure/"]
COPY ["Libraries/Xms.Identity/Xms.Identity.csproj", "Libraries/Xms.Identity/"]
COPY ["Libraries/Security/Xms.Security.Domain/Xms.Security.Domain.csproj", "Libraries/Security/Xms.Security.Domain/"]
COPY ["Libraries/Xms.Domain/Xms.Domain.csproj", "Libraries/Xms.Domain/"]
COPY ["Libraries/Xms.Session/Xms.Session.csproj", "Libraries/Xms.Session/"]
COPY ["Libraries/Organization/Xms.Organization.Domain/Xms.Organization.Domain.csproj", "Libraries/Organization/Xms.Organization.Domain/"]
COPY ["Libraries/Organization/Xms.Organization/Xms.Organization.csproj", "Libraries/Organization/Xms.Organization/"]
COPY ["Libraries/DataCore/Xms.Data/Xms.Data.csproj", "Libraries/DataCore/Xms.Data/"]
COPY ["Libraries/DataCore/Xms.Data.Abstractions/Xms.Data.Abstractions.csproj", "Libraries/DataCore/Xms.Data.Abstractions/"]
COPY ["Libraries/Configuration/Xms.Configuration/Xms.Configuration.csproj", "Libraries/Configuration/Xms.Configuration/"]
COPY ["Libraries/Configuration/Xms.Configuration.Domain/Xms.Configuration.Domain.csproj", "Libraries/Configuration/Xms.Configuration.Domain/"]
COPY ["Libraries/Xms.Caching/Xms.Caching.csproj", "Libraries/Xms.Caching/"]
COPY ["Libraries/Security/Xms.Security.Principal/Xms.Security.Principal.csproj", "Libraries/Security/Xms.Security.Principal/"]
COPY ["Libraries/Security/Xms.Security.MenuAuthorization/Xms.Security.MenuAuthorization.csproj", "Libraries/Security/Xms.Security.MenuAuthorization/"]
COPY ["Libraries/Solution/Xms.Solution.Abstractions/Xms.Solution.Abstractions.csproj", "Libraries/Solution/Xms.Solution.Abstractions/"]
COPY ["Libraries/Localization/Xms.Localization/Xms.Localization.csproj", "Libraries/Localization/Xms.Localization/"]
COPY ["Libraries/Solution/Xms.Solution/Xms.Solution.csproj", "Libraries/Solution/Xms.Solution/"]
COPY ["Libraries/File/Xms.File.Extensions/Xms.File.Extensions.csproj", "Libraries/File/Xms.File.Extensions/"]
COPY ["Libraries/Solution/Xms.Solution.Domain/Xms.Solution.Domain.csproj", "Libraries/Solution/Xms.Solution.Domain/"]
COPY ["Libraries/Localization/Xms.Localization.Domain/Xms.Localization.Domain.csproj", "Libraries/Localization/Xms.Localization.Domain/"]
RUN dotnet restore "Api/Xms.Api.Identity/Xms.Api.Identity.csproj"
COPY . .
WORKDIR "/src/Api/Xms.Api.Identity"
RUN dotnet build "Xms.Api.Identity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Xms.Api.Identity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Xms.Api.Identity.dll"]
