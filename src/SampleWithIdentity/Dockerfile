FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["SampleWithIdentity/SampleWithIdentity.csproj", "SampleWithIdentity/"]
RUN dotnet restore "SampleWithIdentity/SampleWithIdentity.csproj"
COPY . .
WORKDIR "/src/SampleWithIdentity"
RUN dotnet build "SampleWithIdentity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SampleWithIdentity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SampleWithIdentity.dll"]