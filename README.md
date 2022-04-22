# ocelot-application

## Introduction
Implementation of a microservice application in .NET 6.0
using Ocelot Gateway to communicate between services.

Each service and gateway is in a docker container, 
so access from outside is not possible.
Gateway allows you to process requests 
to services and return a response.

The gateway also allows you to perform authentication 
before executing a request, which allows you 
to restrict access to the service (endpoints). 
Authentication is implemented using 
the identity server([Duende](https://duendesoftware.com/products/identityserver)) 
based on Access([JWT](https://docs.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtsecuritytoken?view=azure-dotnet)) tokens

***

## Used dependecies
- [Asp.NET Core](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
- [Swashbuckle](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio) 
provide developer UI to debug your APIs
- [Ocelot](https://github.com/ThreeMammals/Ocelot) 
makes it possible to implement a gateway between microservices
- [Duende Identity Server](https://duendesoftware.com/products/identityserver)
 analogue of Base Identity Server with .NET 6.0 support
- [Serilog](https://serilog.net/) more convenient logger for Identity Server
## Examples
For example and tests, there are 4 requests: 
- Secured:  
	- ``your-ip:4000/secured`` GET request return "private information" if you are authorized  
	- ``your-ip:4000/secured/{number}`` GET request 
	return "private" number square if you are authorized.
	For example: ``your-ip:4000/secured/4`` return 16
	``your-ip:4000/secured`` POST request with body: 

            {
              "FirstName": "string",
			  "LastName": "string"
            }  
    	returns a combination of these two strings with correct authorization.
- Unsecured  
    - ``your-ip:4000/WeatherForecast`` GET request return WeatherForecast example

## Deploy
Download latest version from master:

	git clone https://github.com/Reikudo/ocelot-application.git

Open cmd in solution folder and execute:  

	docker-compose up --build

## Tests
### Postman
For getting auth in Postman you need create new request
and configure `Authorization` section:  

	 Type: OAuth 2.0
	 Header Prefix: Bearer
	 Token Name: Bearer
	 Grant Type: Client Credentials
	 Access Token URL: http://localhost:5000/connect/token
	 Client ID: client
	 Client Secret: secret_key
	 Scope: API
	 Client Authentication: Send as Basic Auth Header

Then click Get New Access Token. If your Identity Server works you will get new access token for your requests.

### Integrated tests
you can simply run tests to check if the services are working. For this:  
1.	Open sulution in Visual Studio  
2.  Choose `ocelot-tests` project
3.	Test -> Run All Tests
