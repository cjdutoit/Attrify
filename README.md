# Attrify

[![Build](https://github.com/cjdutoit/Attrify/actions/workflows/build.yml/badge.svg)](https://github.com/cjdutoit/Attrify/actions/workflows/build.yml)
[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?filter=v2.10.3&style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f)](https://github.com/hassanhabib/The-Standard)
[![The Standard Community](https://img.shields.io/discord/934130100008538142?color=%237289da&label=The%20Standard%20Community&logo=Discord)](https://discord.gg/vdPZ7hS52X)
[![NuGet version (Attrify)](https://img.shields.io/nuget/v/Attrify.svg?style=flat-square)](https://www.nuget.org/packages/Attrify/)
[![NuGet downloads (Attrify)](https://img.shields.io/nuget/dt/Attrify.svg?style=flat-square)](https://www.nuget.org/packages/Attrify/)



A library to enhancing REST API functionality through attributes.

## Attributes in this library

---

### 1. [InvisibleApi](Documentation/InvisibleApi/README.md)

The `Attrify - Invisible API` is a solution designed to hide specific API endpoints, primarily for acceptance testing purposes. 
Developers can uses the `[invisibleApi]` attribute to mark `Controllers` or `Actions` as invisible, eliminating the need for 
manual endpoint configuration in middleware. It also introduces an additional security layer by requiring that an authenticated 
user must belong to a certain role to access the endpoint. If these conditions are not met, the middleware intercepts the request 
and returns a `404 Not Found` response, making the endpoint invisible to unauthorized users.

---

### 2. [DeprecatedApi](Documentation/DeprecatedApi/README.md)

The `Deprecated API` solution is designed to help developers manage the lifecycle of API endpoints by 
signaling when they are outdated or set to be decommissioned. By using the `Deprecated API Attribute` 
and its accompanying `Middleware`, developers can mark API endpoints as deprecated and provide users 
with advance notice of when an API will be removed. This includes a "Sunset" date, which specifies 
when the API will no longer be available, and a warning message encouraging users to migrate to 
newer versions. Additionally, developers can provide a link to migration resources to assist users 
in transitioning. Once the sunset date passes, the middleware automatically returns a 410 Gone HTTP 
status code to indicate that the API is no longer available, ensuring clear communication and minimizing 
disruption for consumers. This solution helps ensure that users are well-informed and have sufficient time 
to adapt, making the deprecation process smooth and reducing the impact on API consumers.

---