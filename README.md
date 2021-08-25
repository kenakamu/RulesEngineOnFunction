# Introduction

This repository contains a sample Azure Functions application for [RulesEngine](https://github.com/microsoft/RulesEngine). The rule definition is stored in Cosmos DB.

# Architecture

![](./assets/rulesenginearchitecture.png)

# Required Azure Service

- Azure Functions
- Cosmos DB (Core API)

## Cosmos DB containers

- This sample expects "rulesDb" database and "rulesEngineWorkflows" and "leases" container with "/id" as partition.
- leases container is used for [Cosmos DB ChangeFeed](https://docs.microsoft.com/en-us/azure/cosmos-db/change-feed).

# How to run in local

Create local.settings.json and add following config.
Update CosmosDBConnectionString.

```json
{
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDBConnectionString": "",
    "DatabaseId": "rulesDb",
    "ContainerId": "rulesEngineWorkflows"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*",
    "CORSCredentials": false
  },
  "IsEncrypted": false
}
```

# Read Rules Definition

1. When the function app started, it reads all rule definitions from CosmosDB in startup and store them in memory.

1. When rule(s) changes in Cosmos DB, the change is propagated to the function via Cosmos DB Change feed, then the function retrieves rules definition(s) to update rules in memory.

# Cosmos DB and Change Feed limitation

## Id property and schema limitation in Cosmos DB

As each documents in Cosmos DB requires "id" property and cannot store array as top level, I need to wrap the RulesEngine rule definition by using own class. See [Workflow.cs](./RulesEngineOnFunction/Models/Workflow.cs).

I use "id" as workflow name.

Example of rule definition in Cosmos DB. Please compare with [Discount.json in original repo](https://github.com/microsoft/RulesEngine/blob/main/demo/DemoApp/Workflows/Discount.json)

```json
{
    "id": "Discount",
    "rule": {
        "workflowName": "Discount",
        "rules": [
            {
                "ruleName": "GiveDiscount10",
                "successEvent": "10",
                "errorMessage": "One or more adjust rules failed.",
                "errorType": "Error",
                "ruleExpressionType": "Lambdaexpression",
                "expression": "input1.country == \"india\" AND input1.loyalityFactor <= 2 AND input1.totalPurchasesToDate >= 5000 AND input2.totalOrders > 2 AND input3.noOfVisitsPerMonth > 2"
            },
            {
                "ruleName": "GiveDiscount20",
                "successEvent": "20",
                "errorMessage": "One or more adjust rules failed.",
                "errorType": "Error",
                "ruleExpressionType": "Lambdaexpression",
                "expression": "input1.country == \"india\" AND input1.loyalityFactor == 3 AND input1.totalPurchasesToDate >= 10000 AND input2.totalOrders > 2 AND input3.noOfVisitsPerMonth > 2"
            },
            {
                "ruleName": "GiveDiscount25",
                "successEvent": "25",
                "errorMessage": "One or more adjust rules failed.",
                "errorType": "Error",
                "ruleExpressionType": "Lambdaexpression",
                "expression": "input1.country != \"india\" AND input1.loyalityFactor >= 4 AND input1.totalPurchasesToDate >= 10000 AND input2.totalOrders > 2 AND input3.noOfVisitsPerMonth > 5"
            },
            {
                "ruleName": "GiveDiscount30",
                "successEvent": "30",
                "errorMessage": "One or more adjust rules failed.",
                "errorType": "Error",
                "ruleExpressionType": "Lambdaexpression",
                "expression": "input1.loyalityFactor > 3 AND input1.totalPurchasesToDate >= 50000 AND input1.totalPurchasesToDate <= 100000 AND input2.totalOrders > 5 AND input3.noOfVisitsPerMonth > 15"
            },
            {
                "ruleName": "GiveDiscount30NestedOrExample",
                "successEvent": "30",
                "errorMessage": "One or more adjust rules failed.",
                "errorType": "Error",
                "operator": "OrElse",
                "rules": [
                    {
                        "ruleName": "IsLoyalAndHasGoodSpend",
                        "errorMessage": "One or more adjust rules failed.",
                        "errorType": "Error",
                        "ruleExpressionType": "Lambdaexpression",
                        "expression": "input1.loyalityFactor > 3 AND input1.totalPurchasesToDate >= 50000 AND input1.totalPurchasesToDate <= 100000"
                    },
                    {
                        "ruleName": "OrHasHighNumberOfTotalOrders",
                        "errorMessage": "One or more adjust rules failed.",
                        "errorType": "Error",
                        "ruleExpressionType": "Lambdaexpression",
                        "expression": "input2.totalOrders > 15"
                    }
                ]
            },
            {
                "ruleName": "GiveDiscount35NestedAndExample",
                "successEvent": "35",
                "errorMessage": "One or more adjust rules failed.",
                "errorType": "Error",
                "operator": "AndAlso",
                "rules": [
                    {
                        "ruleName": "IsLoyal",
                        "errorMessage": "One or more adjust rules failed.",
                        "errorType": "Error",
                        "ruleExpressionType": "Lambdaexpression",
                        "expression": "input1.loyalityFactor > 3"
                    },
                    {
                        "ruleName": "AndHasTotalPurchased100000",
                        "errorMessage": "One or more adjust rules failed.",
                        "errorType": "Error",
                        "ruleExpressionType": "Lambdaexpression",
                        "expression": "input1.totalPurchasesToDate >= 100000"
                    },
                    {
                        "ruleName": "AndOtherConditions",
                        "errorMessage": "One or more adjust rules failed.",
                        "errorType": "Error",
                        "ruleExpressionType": "Lambdaexpression",
                        "expression": "input2.totalOrders > 15 AND input3.noOfVisitsPerMonth > 25"
                    }
                ]
            }
        ]
    }
}
```

## Change Feed

Cosmos DB Change Feed gives changed content with Change Feed notification, thus we usually don't need to query the database to get latest document. However, when the model is incompatibile with the Framework, it failes to bind the data as input. Thus I use basic class (which only conatins id) to receive the change, then retrieve the document on purpose.

# Feedback

Please feel free to give us feedback from issues.