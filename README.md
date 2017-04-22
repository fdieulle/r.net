# r.net
r.net is a r package which allows to use .Net assemblies from R environment.

The goal of this pacakge was driven by the idea to use prodution and industrial code implemented in .Net/C# in R envinronment, gets results in R to process statistics research on them.

The package provides you simple tools to interact with your .Net assemblies. A large basic data conversions between R and .Net are included and the package allows you to enrich or override data converters. A part of basic data conversions use mainly another github repository see: [https://github.com/jmp75/rdotnet-onboarding](https://github.com/jmp75/rdotnet-onboarding). Thank you to creators and contributors for this repository. Because of .Net is a POO language and you need to manipulate objects in R, the package provides also a R class generator from .Net classes. I choose R6 package to generate R classes.


## How to install package

r.net package is supported on Windows OS only.

To install the package in your R environment, you can found the built package on GitHub project releases [here](https://github.com/fdieulle/r.net/releases)


If your prefer build package from sources you should run the build.cmd script stored in the root folder.
You need to have the following prerequists:
* Visual Studio 2010 or higher.
* Framework .Net 4.0 or higher

Visual Studio express version should be enough and free if you don't have installed yet. You can download it [here](https://www.visualstudio.com/fr/downloads/).


## R/.Net basic interactions
r.net  package allows you to interact with .Net applications and libraries. To use this package you have to load it with `library(r.net)`. When the package is loaded a new instance of .Net CLR (Common Language Runtime) is created and loaded in R process.
### How to load .Net assemblies
The R function `netLoadAssembly(filePath)` loads a .Net assembly in your process.

If your custom library contains other assemblies dependencies I advise you to use this following code to load all mandatory dll.
```R
libraryr(r.net)
path <- "Your path where dlls are stored"
assemblies <- list.files(path = path, pattern = "*.dll$|*.exe$", full.names = TRUE)
lapply(assemblies, netLoadAssembly)
```

When the `r.net` package is loaded it contains by default all the .Net framework and access on your GAC stored assemblies. So you don't need to load them manually.

#### Technical consideration
With the Clr loaded a .Net AppDomain is loaded also, so all constraints linked to it are kept. The main constraint is that an AppDomain can't unload an assembly. It can't also load an assembly a second time. So if you want to update an assembly loaded you have to restart your R process and reload all assemblies.

### How to interact with static methods and properties
Once your assemblies are loaded in your process you can start to interact with them. A simplem entry point is to use static call. The package provides you 3 functions for that
* `netCallStatic(typeName, methodName, ...)`: Call a static method for a given .Net type name
* `netGetStatic(typeName, propertyName)`: Gets a static property value from a .Net type name
* `netSetStatic(typeName, propertyName, value)`: Sets a static property value from a .Net type name

#### netCallStatic

- `typeName`: Full .Net type name
- `methodName`: Method name to call
- `...`: Method arguments

The `typeName` should respect the full type name convention: `Namespace.TypeName`
If there is conflicts with a `methodName` like many definitions in your .Net type, a score selection is computed from your arguments orders and types to choose the best one. We consider as a higher priority single value compare to collection of values.
Ellipses `...` has to keep the .net arguments method order, the named arguments are not supported yet.
The methods result is converted in a R type if a converter is defined otherwise it returns an external pointer.

```R
library(r.net)

pckPath <- path.package("r.net")
f <- file.path(pckPath, "tests", "RDotNet.AssemblyTest.dll")
netLoadAssembly(f)

type <- "RDotNet.AssemblyTest.StaticClass"
netCallStatic(type, "CallWithInteger", 2L)
netCallStatic(type, "CallWithIntegerVector", c(2L, 3L))

# Method selection single value vs vector values
netCallStatic(type, "SameMethodName", 1.23)
netCallStatic(type, "SameMethodName", c(1.24, 1.25))
netCallStatic(type, "SameMethodName", c(1.24, 1.25), 12L)
```

#### netGetStatic/netSetStatic
Gets or sets a static property value from a .Net type name

- `typeName`: Full .Net type name
- `propertyName`: propertyName Property name to get value
- `value`: Value to set

The `typeName` should respect the full type name convention: `Namespace.TypeName`
Allows you to get or set a property value from .Net type name.
The input and ouput value will be converted between R type and .Net type. If the property value isn't a native C# type or a mapped conversion type you have to use an external pointer on .Net object.
```R
library(r.net)

pckPath <- path.package("r.net")
f <- file.path(pckPath, "tests", "RDotNet.AssemblyTest.dll")
netLoadAssembly(f)

dataConverter <- netGetStatic("RDotNet.ClrProxy.ClrProxy", "DataConverter")
netSetStatic("RDotNet.ClrProxy.ClrProxy", "DataConverter", dataConverter)
```
### How to interact with .Net objects

### How to test and Debug

## Data conversions

### Type converters

### How to override data converters

### How to wrap external pointer in R6 class

## R6 class generator
