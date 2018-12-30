FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 8080

ENV ROOT_PASSWORD Password1
RUN apt-get update
RUN apt-get install -y openssh-server
RUN sed -i s/#PermitRootLogin.*/PermitRootLogin\ yes/ /etc/ssh/sshd_config
RUN echo "root:${ROOT_PASSWORD}" | chpasswd
RUN ssh-keygen -A
RUN mkdir /run/sshd

#RUN echo "#!/bin/bash\ndotnet TwitterTracker.dll -tx \#news | dotnet TwitterTracker.Extract.Urls.dll | dotnet TwitterTracker.Utilities.Frequency.dll" > /app/docker-run.sh
RUN echo "#!/bin/bash\ndotnet TwitterTracker.dll -tx \#news | dotnet TwitterTracker.Extract.Urls.dll" > /app/docker-run.sh
#COPY docker-run.sh /app
RUN chmod +x /app/docker-run.sh

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY TwitterTracker.Core/TwitterTracker.Core.csproj TwitterTracker.Core/
COPY TwitterTracker.Extract.Urls/TwitterTracker.Extract.Urls.csproj TwitterTracker.Extract.Urls/
COPY TwitterTracker.Utilities.Frequency/TwitterTracker.Utilities.Frequency.csproj TwitterTracker.Utilities.Frequency/
COPY TwitterTracker/TwitterTracker.csproj TwitterTracker/
RUN dotnet restore TwitterTracker.Core/TwitterTracker.Core.csproj
RUN dotnet restore TwitterTracker.Extract.Urls/TwitterTracker.Extract.Urls.csproj
RUN dotnet restore TwitterTracker.Utilities.Frequency/TwitterTracker.Utilities.Frequency.csproj
RUN dotnet restore TwitterTracker/TwitterTracker.csproj
COPY . .

RUN dotnet build TwitterTracker/TwitterTracker.csproj -c Release -o /app
RUN dotnet build TwitterTracker.Extract.Urls/TwitterTracker.Extract.Urls.csproj -c Release -o /app
RUN dotnet build TwitterTracker.Utilities.Frequency/TwitterTracker.Utilities.Frequency.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TwitterTracker/TwitterTracker.csproj -c Release -o /app
RUN dotnet publish TwitterTracker.Extract.Urls/TwitterTracker.Extract.Urls.csproj -c Release -o /app
RUN dotnet publish TwitterTracker.Utilities.Frequency/TwitterTracker.Utilities.Frequency.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["/bin/bash", "/app/docker-run.sh"]
