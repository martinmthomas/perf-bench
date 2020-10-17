#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

ARG AnalysisCosmosDbConnectionString=''
ARG Core31LinuxUrl=''
ARG Core31FuncAppUrl=''

ENV AnalysisCosmosDb__ConnectionString=$AnalysisCosmosDbConnectionString
ENV Platforms__0__Url=$Core31LinuxUrl
ENV Platforms__1__Url=$Core31FuncAppUrl

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Analyzer/Analyzer.csproj", "Analyzer/"]
RUN dotnet restore "Analyzer/Analyzer.csproj"
COPY . .
WORKDIR "/src/Analyzer"
RUN dotnet build "Analyzer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Analyzer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Analyzer.dll"]