# MyApp API

Property management API with MongoDB.

## Local Development

```bash
# Restore dependencies
dotnet restore

# Run the application
cd src/Api
dotnet run
```

## Endpoints

- `GET /properties` - List properties with filters
- `GET /properties/{id}` - Get property with details
- `POST /properties` - Create property
- `PUT /properties/{id}` - Update property
- `DELETE /properties/{id}` - Delete property

## Available filters for GET /properties:
- `name`: Filter by name
- `address`: Filter by address
- `minPrice`: Minimum price
- `maxPrice`: Maximum price

Example: `GET /properties?name=house&minPrice=100000&maxPrice=500000`
