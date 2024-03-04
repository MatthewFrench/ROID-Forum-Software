# ROID-Forum-Software
Realtime online interactive dynamic forum software.

Technologies:
- C# Server
- Typescript and SASS PWA/SWA
- Cassandra Database

Client has a persistent connection and gets data pushed automatically with information on the forum. Information auto-updating includes people online, friends online, people typing, comments on threads, new thread creation and messages.

Goals:
* Server needs to connect to a database(s) through a configuration file.
* Forum web client should have a default template usage page for testing the forum.
* Forum web client should be an embeddable TS/(Compiled JS) library so it can be inserted and used in either a Typescript project or a Javascript project.

Future Improvements:
* Use an async logger to not block on the server side
* Use parallelism to run multiple Cassandra queries in parallel on the server-side
* Cache certain non-important data structures with TTL in order to optimize query speed
* Add pagination
* Add Google/Facebook/Apple login
* Add event publisher/subscriber so the client side can subscribe to events that the server side publishes. This would greatly reduce the manual network message configuration
* Use server-side event publishing to better decouple local caching of users viewing threads/sections/online and to allow full independent forum microservice scaling
* Use a binary message to code generator, like proto or flatpak. That would greatly reduce manual configuration and accidental message mismatch
* Add a file storage service so files can be uploaded and references (like avatar pictures and thread images/videos)
* Add websocket wait on reply, so I don't need specific messages for sending and listening for a response, instead I can send the message and then wait for a reply using promises
* Limit submitted string sizes on server and client side
* Use a UI framework that automatically updates UI values, such as React, to avoid manual updating code and callbacks
* Add a mini-game that people can join in together for fun, something simple like jumping around a 2D platformer with obstacles and shooting cannonballs at each other
* Add admin tools to allow easy moderation
* Re-write in Rust for maximum memory safety

## Server
* Install ~~dotnet 2.0~~, this has been updated to dotnet 8, this section is out of date
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