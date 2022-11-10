#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/core/Jx.Cms.Web/Jx.Cms.Web.csproj", "src/core/Jx.Cms.Web/"]
COPY ["src/core/Jx.Cms.Common/Jx.Cms.Common.csproj", "src/core/Jx.Cms.Common/"]
COPY ["src/core/Jx.Cms.Themes/Jx.Cms.Themes.csproj", "src/core/Jx.Cms.Themes/"]
COPY ["src/core/Jx.Cms.Plugin/Jx.Cms.Plugin.csproj", "src/core/Jx.Cms.Plugin/"]
COPY ["src/core/Jx.Cms.DbContext/Jx.Cms.DbContext.csproj", "src/core/Jx.Cms.DbContext/"]
RUN dotnet restore "src/core/Jx.Cms.Web/Jx.Cms.Web.csproj"
COPY . .
WORKDIR "/src/src/core/Jx.Cms.Web"
RUN dotnet build "Jx.Cms.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Jx.Cms.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=zh_CN.UTF-8 \
    LANG=zh_CN.UTF-8
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jx.Cms.Web.dll"]