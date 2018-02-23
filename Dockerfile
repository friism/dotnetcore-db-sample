FROM microsoft/dotnet:2.0-sdk-stretch as builder

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore -r linux-x64

COPY . ./

RUN dotnet publish -c Debug -o out -r linux-x64

FROM microsoft/dotnet:2.0-runtime-stretch

WORKDIR /app
COPY --from=builder /app/out .
# ENV ASPNETCORE_URLS=http://+:80
CMD ASPNETCORE_URLS=http://+:$PORT dotnet MyApp.dll
