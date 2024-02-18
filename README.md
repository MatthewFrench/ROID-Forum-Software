# ROID-Forum-Software
Realtime online interactive dynamic forum software.

Technologies:
- C# Server
- Typescript and SASS PWA/SWA
- Cassandra Database

Client has a persistent connection and gets data pushed automatically with information on the forum. Information auto-updating includes people online, friends online, people typing, comments on threads, new thread creation and messages.

Goals:
• Server needs to connect to a database(s) through a configuration file.
• Forum web client should have a default template usage page for testing the forum.
• Forum web client should be an embeddable TS/(Compiled JS) library so it can be inserted and used in either a Typescript project or a Javascript project.

## Server
* Install dotnet 2.0
  * `brew tap isen-ng/dotnet-sdk-versions`
  * `brew install --cask dotnet-sdk2-2-100`
* In `ROID-Forum-Server/ROID-Forum-Server`
  * `dotnet build`
  * `dotnet run`

If running locally, specify `environment=local` environment variable.

## Client
* In `ROID-Forum-Web-Client` folder
  * `npm ci --legacy-peer-deps`
  * `npm run build`

## Deploying Server
In `ROID-Forum-Server/ROID-Forum-Server` folder, run `docker-compose -f "production-server-and-db-docker-compose.yml" up`.  
The server docker instance will be accessible on 7779.  
This server expects signed certs to exist.
Created pfx cert using "openssl pkcs12 -export -out cert.pfx -inkey privkey1.pem -in fullchain1.pem".

## Deploying server locally (without certs)
Run `docker-compose -f "local-server-and-db-docker-compose.yml" up -d`. 