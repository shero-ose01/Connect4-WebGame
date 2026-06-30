# Connect 4
A simple Multiplayer Connect 4. 

## How to play
Create a game/room, send the invite link to another person and you can play together.

## Stack
Backend: ASP.NET minimal Api, Websockets/SignalR hub
Frontend: Angular
Hosting: Docker compose, nginx, Cloudflare Tunnel hosted under <a href="http://connect4.s-ose.de">connect4.s-ose.de</a>
CI/CD: Github actions, xUnit + Vitest on every push, deploy via self-hosted runner

## Screenshots

![Connect 4 home screen](docs/screenshots/home.png)

### Gameplay

![Connect 4 gameplay](docs/screenshots/demo.gif)
