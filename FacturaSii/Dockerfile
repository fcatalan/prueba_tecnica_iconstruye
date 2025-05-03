# 1) Imagen base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 2) Imagen build SDK para compilar y publicar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar y restaurar
COPY FacturaSii.csproj ./
RUN dotnet restore "FacturaSii.csproj"

# Copiar todo y publicar
COPY . ./
RUN dotnet publish "FacturaSii.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# 3) Imagen final
FROM base AS final
WORKDIR /app

USER root

# Incluir el certificado en la imagen
COPY https/certificado-prueba.pfx /https/certificado-prueba.pfx  

COPY caf/caf_actual.xml caf/caf_actual.xml 

# Asignar permisos de lectura a todos
RUN chmod 644 /https/certificado-prueba.pfx  
RUN chmod 644 /app/caf/caf_actual.xml

# Copiar artefactos publicados
COPY --from=build /app/publish ./

# Arrancar la aplicación; las migraciones corren en Program.cs
ENTRYPOINT ["dotnet", "FacturaSii.dll"]
