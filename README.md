# TodoAppWasm

## Local Deployment

### Prerequisites
- [Docker](https://www.docker.com/) installed on your machine.

### Pull the image
```bash
docker pull ghcr.io/<your-namespace>/todoappwasm:latest
```
Replace `<your-namespace>` with the appropriate container registry namespace.

### Run the container
```bash
docker run -d -p 8080:8080 --name todoappwasm \
  ghcr.io/<your-namespace>/todoappwasm:latest
```

### Configuration
The container exposes port `8080`. You can configure the runtime environment by setting the `ASPNETCORE_ENVIRONMENT` variable (defaults to `Production`). For example:
```bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development \
  ghcr.io/<your-namespace>/todoappwasm:latest
```
