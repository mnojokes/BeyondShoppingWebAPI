# Beyond Shopping Web API

## What is this
Beyond Shopping is a RESTful API simulating a simple ordering system, which I created and presented as a final exam solution in Adform .NET Academy. It can be run entirely from Docker. Since this is a showcase project, the Docker database is intended to be temporary, no persistent data is stored on the user's device. The original solution was created in a single day, I have since made some changes and bug fixes to the solution to make it more suitable as a portfolio project accompanying my resume.

### What's implemented
* N-layer architecture;
* Swagger documentation: comments, filters, examples;
* Remote user repository (uses [JSONPlaceholder](https://jsonplaceholder.typicode.com/users) for demonstration), accessed via HTTPClient;
* Polly library to ensure HTTPClient resilience;
* PostgreSQL database for storing orders and items;
* Dbup for database migrations;
* Dapper for database commands and queries;
* Database transaction logic to ensure data integrity between different tables;
* Hosted service for periodic data cleanup in the background;
* Docker-Compose;
* HTTP request file;
* Exception handling middleware;
* FluentValidation library for input validation;
* Moq library to create mock infrastructure components (for testing mostly).

## Functionality
### Orders
* Orders can be placed (status set as "Pending");
* Orders can be completed (status set to "Completed");
* Orders can be filtered by user ID.

Orders can remain Pending temporarily. After a specified period of time (120 minutes by default, configured in AppSettings), orders expire and can no longer be completed; the background service deletes them during its next periodic run (every 15 minutes by default, configured in AppSettings).

User data is designed to be accessed from a remote repository. Currently uses [JSONPlaceholder](https://jsonplaceholder.typicode.com/users) as a user repository for demo purposes, so IDs will be validated with what's available there.
