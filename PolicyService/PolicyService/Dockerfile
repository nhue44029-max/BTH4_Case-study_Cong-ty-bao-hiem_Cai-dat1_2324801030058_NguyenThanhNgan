FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PricingService/PricingService.Api/ PricingService/PricingService.Api/
COPY PolicyService/PolicyService.Api/ PolicyService/PolicyService.Api/
COPY PolicyService/PolicyService/ PolicyService/PolicyService/
RUN dotnet publish PolicyService/PolicyService/PolicyService.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=docker
COPY --from=build /out .
ENTRYPOINT ["dotnet", "PolicyService.dll"]
