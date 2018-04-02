# DynamicCore
A OWIN sefl-host application that could host web api with Web API controller dll files stored in indicated folders.

## What is DynamicCore want to achieve
* Provide a hot plugging supported Web API hosting environment to reduce downtime.
* Provide a easy and configurable hosting environment that provide the following functionalities:
    * Turn on/off controllers through editing the configuration file at runtime.
    * Provide help page and with xml description files supported.
    * Logging both requests and responses.
    * Encrypt/Decrypt both requests and responses.
    * Able to extend for hosting additional services such as SignalR.
 
## The Libraries & Architecture
### DynamicCore.DynamicController.Common
The shared library for utilities, helpers and controller base class for creating controller dlls.

### DynamicCore.DynamicHostConsole
The OWIN application and configurations.

### DynamicCore.HelpPageLibrary
The customized T4 template and utilities for generate help pages automatically.

### ProductsApiLibrary & UsersApiLibrary
The sample controller project to create dll files for DynamicHostConsole to host.
