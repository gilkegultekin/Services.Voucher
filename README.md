# Introduction 
The Voucher API contains all the base functionality to retrieve information about vouchers. Apart from the usual Get and GetById operations, you can also look up vouchers by their name, search for a voucher whose name contains the search term and get the cheapest voucher with a certain product code.

## Usage
 The solution is configured to run two projects at startup, the API and the auth server. To test it, you can use the provided postman collection or the Swagger interface.

### Authentication using the Postman collection
Before you can send an authenticated request to the API, you need to make sure that the "accessToken" variable is populated. In order to achieve that, all you have to do is to execute the "GetBearerToken" request once. Afterwards your requests to the API will be authenticated until your access token expires. The lifetime of the access token is 1 hour.

# Design choices

## Database and the ORM
I chose In-Memory Sql as the database (due to the ease of setup) and Entity Framework v3 as the ORM because I'm experienced with it (or so I thought. Also my plans to enhance the API factored into my decision as well). I was under the impression that performance would not be an issue with the current data set. Apparently, I was not aware of the Cartesian explosion problem introduced in v3, which significantly slows down queries with the Include expression.
As I did not want to break down the data (because it seemed unnecessary), I only modeled a many-to-many relationship between product codes and vouchers, the only entities in the database. My initial queries with the Include expression were immensely slow.
I tried to get around this problem by switching to explicit loading. Although this did improve query performance, the Get and GetByName queries are still quite slow, taking about 1.5 secs in Postman, on my machine.

I had three solutions in mind, which I could not try due to time constraints. 
- Upgrading to EF Core 6 could have alleviated the issue but it would also have required me to upgrade the entire solution to .NET 6. 
- Using a different ORM, namely Dapper was another alternative. Since the API only supports GET operations, a full-fledged ORM like EF is certainly overkill and writing efficient queries with Dapper could have fixed the performance problem.
- Using a NoSQL database, in my case MongoDB. I think that the data structure and the operations performed by the API lend themselves well to a NoSQL database solution.

That being said, one of the main reasons I chose EF was that I wanted to improve and enhance the API with extra funcionality, e.g. earmarking and using vouchers for a given order, triggering domain events, etc. With some write operations in the picture EF made more sense.

## Architecture
I tried to use the Clean Architecture approach as much as I could. I have a domain project at the heart of the solution, even though the domain is very anemic. I generally prefer to have an application layer, which houses the interfaces the controllers depend upon and the dtos. Usually, in a more complex domain it would also contain classes that implement application-specific logic.
Then there is the API itself and in the outermost circle we have the database provider.

## Testing
I used an in-memory database for testing purposes. The unit tests were pretty straightforward. In the provided controller tests, you were mocking the repository dependency, but since my repository implementation was basically a thin wrapper for the db context and because of the in-memory database, I decided against it. If the dependency graph was more complex, I would certainly have mocked the top level dependencies.
I also refactored the performance tests to support cancellation tokens, so that they would stop executing after the allotted time.