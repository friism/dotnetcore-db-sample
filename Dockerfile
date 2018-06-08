FROM microsoft/dotnet:2.1-sdk-stretch as builder

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore -r linux-x64

COPY . ./

RUN dotnet publish -c Release -o out -r linux-x64 /p:TrimUnusedDependencies=true /p:RootPackageReference=false

FROM microsoft/dotnet:2.1-runtime-deps-stretch

WORKDIR /app
COPY --from=builder /app/out .
CMD ASPNETCORE_URLS=http://+:$PORT ./MyApp
