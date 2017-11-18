# BittrexSharp

## Introduction

BittrexSharp is a thin wrapper around the Bittrex api which wraps each endpoint as a C# function and has a dedicated class for all returned objects. So BittrexSharp allows type-safe developing with the Bittrex api.  
It exposes the same fields, so its [documentation][1] of input and output is applicable for BittrexSharp too.  
BittrexSharp also handles authentication.

## Installing

BittrexSharp is listed as [Nuget package][2], so you can either use the package manager UI, or install it with the Package Manager CLI

```
Install-Package BittrexSharp -Version 0.1.0
```

or with .Net CLI

```
dotnet add package BittrexSharp --version 0.1.0
```

## Using BittrexSharp

The main class is `Bittrex`. It is the main wrapper for the api. The only things needed for its instantiation is an api key and its secret.

```C#
var apiKey = "...";
var apiSecret = "...";
var bittrex = new Bittrex(apiKey, apiSecret);
```

## Specialized Implementations

At the moment there is only one, `BittrexOrderSimulation`.  
It simulates every api call related to orders. So buy, sell and list orders calls are not sent to the Bittrex servers, and instead are locally registered. All other api calls work normally.  
The instance then continuously queries the Bittrex servers for the current rate, and if it is below the specified limit of an order, it treats the order as executed.  
This makes it easier to test trading algorithms.

Instantiating it works exactly the same as with the standard Bittrex wrapper.

```C#
var apiKey = "...";
var apiSecret = "...";
var bittrex = new BittrexOrderSimulation(apiKey, apiSecret);
```

[1]:https://bittrex.com/home/api
[2]:https://www.nuget.org/packages/BittrexSharp/