# MyApp API

Property management API with MongoDB.

## Local Development

### With Docker Compose
```bash
docker-compose up --build
```

### Without Docker
```bash
# Restore dependencies
dotnet restore

# Run the application
cd src/Api
dotnet run
```

## Render Deployment

### Command to start the server on Render:
```bash
dotnet MyApp.Api.dll
```

### Render Configuration:
1. Connect your GitHub repository
2. Select "Docker" as environment
3. Render will automatically detect the Dockerfile
4. Configure environment variables:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ASPNETCORE_URLS=http://0.0.0.0:8080`
   - `ConnectionStrings__DefaultConnection=your_mongodb_connection_string`
   - `AllowedOrigins=https://million-properties-front.vercel.app`

### Required Environment Variables:
- `ConnectionStrings__DefaultConnection`: MongoDB connection string
- `AllowedOrigins`: Allowed domains for CORS (comma-separated)

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
