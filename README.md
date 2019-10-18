# CQRS On Microservices
Implementation of CQRS across multiple microservices. This architecture contains:

1. A read-only service that holds it's own representation of the data model and an internal cache of the data
2. A data manipulation service that has direct DB access to create/update/delete records
3. Gateway - A simple Ocelot API gateway to give a single query endpoint
