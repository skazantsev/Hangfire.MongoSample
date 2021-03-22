# Sample of using Hangfire with Mongo persistence

## How to run
1. Start MongoDB
```
cd docker
docker-compose up -d
```

2. Run app instances
``` powershell
start powershell -ArgumentList "dotnet run --urls=http://localhost:5000"
start powershell -ArgumentList "dotnet run --urls=http://localhost:5001"
start powershell -ArgumentList "dotnet run --urls=http://localhost:5002"
```

3. Use Swagger UI to create new jobs.
```
explorer http://localhost:5000/swagger
````

4. Monitor jobs using Hangfire UI
```
explorer http://localhost:5000/hangfire
````
