# Authorize

## Account operation authorizer service

### Description

The application was written in C# and .NET Core.

#### How to run it

Just run `dotnet run` inside Authorizer folder

To run in a similar mode like in the spec document just run `dotnet run` and the application will show the character `>`.
That's indicator means the application started.

Write the operation e.g:

> `>` --operation account --input {"activeCard": true, "limit": 100 }
> `>` --operation transaction --input {"merchant": "Nubank", "amount": 60, "time": "2020-10-19T01:09:59.656Z"}

#### How to run the tests

From the Authorizer.Tests directory, run `dotnet test`. It runs all the test cases.

#### How to run using docker

From the project directory, run `docker build -t authorize:latest .`

After the docker build finish, you can run `docker run -it authorize:latest` and it will immediately start reading stdin

### There's More - API Project

In addition, I created an API project that's use that same specification. To run, go to Authorizer.API folder, in a prompt, write `dotnet run` open your favorite's browser and type `https://localhost:5001/swagger`

Have fun =]

## About some Design Decisions

### Setup for Account

The specs do not mention a violation rule for when there's no Account already (in case a transaction is attempt before Account Creation).

So I decided to create a violation rule called `account-not-initialized`

### AccountService

For the sake of simplicity I choosing a single service, responsible for both account state and authorization rules.

### Domain Driven Design

I'm using Domain Driven Design to validate the state of domain and all business logic is in the account class.
