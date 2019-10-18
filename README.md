# CQRS On Microservices
Implementation of CQRS across multiple microservices. This architecture contains:

1. A read-only service that holds it's own representation of the data model and an internal cache of the data
2. A data manipulation service that has direct DB access to create/update/delete records
3. Gateway - A simple Ocelot API gateway to give a single query endpoint

# Depdencies
1. MS SQL Server
2. RabbitMQ event bus
3. Redis Cache

# Usage

All infrasructure requirements are held within the docker-compose file, they can be started by running in the root of the repo 

```
docker-compose up -d
```

To start each of the three individual services, you can run

```
dotnet run -p .\src\apps\Jeasthamdev.Cqrs.Gateway\Jeasthamdev.Cqrs.Gateway.csproj
dotnet run -p .\src\apps\Jeasthamdev.Cqrs.Manipulator\Jeasthamdev.Cqrs.Manipulator.csproj
dotnet run -p .\src\apps\Jeasthamdev.Cqrs.Read\Jeasthamdev.Cqrs.Read.csproj
```

## Create

To create a new order, send the following to http://localhost:5000/api/order

``` json
{
    "orderNumber": "12345",
    "orderDate": "2019-10-18",
    "orderLines": [
        {
        	"product": "MYPRODUCT",
        	"value": 100
        }
    ]
}
```

This will create a new order in the SQL database, and then raise a NewOrderAdded event to the RabbitMQ event bus.

The NewOrderAdded event is handled by the read service. The newly added record is picked up directly from the SQL database and added to the cached data.

## Get

To query existing orders, send a get request to http://localhost:5000/api/order