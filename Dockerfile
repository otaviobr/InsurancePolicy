# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
COPY . .
RUN dotnet restore "InsurancePolicy.Api/InsurancePolicy.Api.csproj"
RUN dotnet publish "InsurancePolicy.Api/InsurancePolicy.Api.csproj" -c Release -o /app

# Stage 2
FROM  mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080

ENTRYPOINT ["dotnet", "InsurancePolicy.Api.dll"]