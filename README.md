# Shoping Cart recruitment test

## Instructions

Part 1:
Your company has decided to create a new line of business.  As a start to this effort, they’ve come to you to help develop a prototype.  It is expected that this prototype will be part of a beta test with some actual customers, and if successful, it is likely that the prototype will be expanded into a full product.

Your part of the prototype will be to develop a Web API that will be used by customers to manage a basket of items. The business describes the basic workflow is as follows:

This API will allow our users to set up and manage an order of items.  The API will allow users to add and remove items and change the quantity of the items they want.  They should also be able to simply clear out all items from their order and start again.

The functionality to complete the purchase of the items will be handled separately and will be written by a different team once this prototype is complete.

For the purpose of this exercise, you can assume there’s an existing data storage solution that the API will use, so you can either create stubs for that functionality or simply hold data in memory.

Feel free to make any assumptions whenever you are not certain about the requirements, but make sure your assumptions are made clear either through the design or additional documentation.

Part 2:
Create a client library that makes use of the API endpoints created in Part 1.  The purpose of this code to provide authors of client applications a simple framework to use in their applications.

If we decide to bring you in for further discussions, you should be prepared to explain and defend any coding and design decisions you make as a part of this exercise.

All code should be written in C# and target the .NET framework library version 4.5 or higher, or .NET core.  Please check all code into a publicly accessible repository on GitHub and send us a link to your repository.

## Build

From inside .\build folder run command build.ps1

Parameters can be set as command line arguments or as environment variables.
Major and minor version numbers can be set in .\build\version.txt file.
If required Http Clients can also be built for other frameworks than .NET Standard. This would need to be set in .csproj file.

Results:
- Self contained WebApi in publish folder
- SDK NuGet package with Http Clients to WebApi in publish folder
- Coverage report in Html format in coverage folder

## Postman Collections

Acceptance tests can be run from Postman or Newman console tool against local instance or any other deployment. Test Scenarios can be found in postman folder.
For ssl setup it might be necessary to disable certificate validation in Postman (File->General->SSL certificate verification).
Also to save environment variables between steps you may need to create environment.
