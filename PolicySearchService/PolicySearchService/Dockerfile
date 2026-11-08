FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PolicyService/PolicyService.Api/ PolicyService/PolicyService.Api/
COPY PolicySearchService/PolicySearchService.Api/ PolicySearchService/PolicySearchService.Api/
COPY PolicySearchService/PolicySearchService/ PolicySearchService/PolicySearchService/
RUN dotnet publish PolicySearchService/PolicySearchService/PolicySearchService.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5065
ENV ASPNETCORE_ENVIRONMENT=docker
COPY --from=build /out .
ENTRYPOINT ["dotnet", "PolicySearchService.dll"]
