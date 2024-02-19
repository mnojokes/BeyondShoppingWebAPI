# .NET Academy Exam Solution

## What is this
A restful API implementing the following functionality simulating an ordering system:  
* An order is created when a user wants to buy an item on the marketplace
* The seller can deliver the item and mark the order as completed.
* The orders that are not paid within 2 hours of creation need to be automatically deleted from the system. (not implemented due to time contraints)
* A user is able to retrieve all of their orders.

User information is hosted on another system and can be fetched from: https://jsonplaceholder.typicode.com/users  

## What went well:
Generated thorough Swagger documentation around all endpoints using Swagger comments, Filters, Examples. Used Dbup and Dapper for database functionality locally, implemented indexes. Used Polly to ensure user data access client's resiliency with retry and exponential backoff policies. I used Moq to mock an Item Repository, since the task requirements did not involve keeping an entire inventory of items. The database contains an orders_items table to allow connecting multiple item purchases with orders.

## What went not so well:
Due to time constrains, I abandoned attempts to use Liquibase and containerization. There were some https certificate issues that I did not think I would be able to resolve within a reasonable amount of time, so I chose a more familiar tool from the Academy - Dbup to implement local database. Initially the project used records for contracts and models, but I abandoned them because it caused issues with Dapper and I was not able to resolve them on time, so I reverted back to using classes instead.  
Due to time constrains, I was not able to debug the project thoroughly after abandoning the use of records.
