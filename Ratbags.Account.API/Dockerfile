# build with .net 8.0 sdk image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# github pat for packages
ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=${GITHUB_TOKEN}

ARG DB_CONNECTION_STRING
ENV DB_CONNECTION_STRING=${DB_CONNECTION_STRING}

ARG JWT_SECRET
ENV JWT_SECRET=${JWT_SECRET}

ARG CERTIFICATE_NAME
ENV CERTIFICATE_NAME=${CERTIFICATE_NAME}

ARG CERTIFICATE_PASSWORD
ENV CERTIFICATE_PASSWORD=${CERTIFICATE_PASSWORD}

# make sure vars gets picked up...
RUN echo "ACCOUNT GITHUB_TOKEN: $GITHUB_TOKEN"
RUN echo "ACCOUNT DB_CONNECTION_STRING: $DB_CONNECTION_STRING"
RUN echo "ACCOUNT JWT_SECRET: $JWT_SECRET"

RUN echo "ACCOUNT CERTIFICATE_NAME: $CERTIFICATE_NAME"
RUN echo "ACCOUNT CERTIFICATE_PASSWORD: $CERTIFICATE_PASSWORD"

# copy nuget.config and replace 'myPAT' with actual pat (using sed insead of powershell for image size)
COPY nuget.config .
RUN sed -i "s/myPAT/$GITHUB_TOKEN/g" nuget.config && cat nuget.config

# copy project file and restore dependencies using modified nuget.config file
COPY ["Ratbags.Account.API.csproj", "./"]
RUN dotnet restore "Ratbags.Account.API.csproj" --configfile nuget.config

# copy and build project
COPY . .
WORKDIR "/src/."
RUN dotnet build "Ratbags.Account.API.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "Ratbags.Account.API.csproj" -c Release -o /app/publish

# run app with .net 8.0 runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5077
EXPOSE 7158

# install certs to sim ssl locally
RUN apt-get update && apt-get install -y ca-certificates

# copy published app into image
COPY --from=publish /app/publish .
COPY aspnetapp.pfx /app/aspnetapp.pfx

# run!
ENTRYPOINT ["dotnet", "Ratbags.Account.API.dll"]
