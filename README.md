# TASKS-API

## Technical References

The following architectural patterns were approached on this solution:: 

- [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) 
- [Domain Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)

Following the **layered architecture** from Domain Driven Design the solution contains the layers:

- Presentation (Rest API)
- Application (Command and Query Handlers representing the *use cases*)
- Domain (Agreggates, Repositories, Entities ...)
- Infrastructure (Persistence, Reading, Caching)

## Solution features

The features requested were implemented following the definition of *use cases* on the *Application Layer* and exposed via HTTP throught a REST API.

- Create Task
- Get Task By Id
- Change Task
- Delete Tasks
- Get Upcomming Tasks

Using the *writing* and *reading* segregation sides the features that represents the change of the state's application were represented by *commands* (Create Task, Change Task, Delete Task) and the actions without changes by *queries* (Get By Id, Get Upcoming) and [exposed by respective HTTP methods via a REST API](https://www.infoq.com/articles/rest-api-on-cqrs/).


## Running the solution

To run the solution will be necessary to have a **git** SCM to clone from Bitbucket and Docker to execute in a containerized environment.

### Requirements

- git
- docker

### Instructions

- clone the code from Bitbucket repository
- on the root folder run the **docker-compose** command


    ```shell
    > docker-compose up
    ```

Once the app is running locally via **Docker** the following links will available:

- [**Swagger**](http://localhost:5006/swagger)
- [**Seq Log**](http://localhost:5341/#/events)
- [**Redis Cache**](http://localhost:8001/redis-stack/browser)


