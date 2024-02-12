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

## Client
* In `ROID-Forum-Web-Client` folder
  * `npm ci --legacy-peer-deps`
  * `npm run build`

## Deploying Server
In `ROID-Forum-Server/ROID-Forum-Server` folder, run `docker-compose -f "server-docker-compose.yml" up --build forum-server`.  
The server docker instance will be accessible on 7779.  
This server expects signed certs to exist.