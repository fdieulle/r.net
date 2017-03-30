library(r.net)
library(R6)
library(testthat)

path <- "E:/Workspace/GitHub/r.net/visualstudio/RDotNet.ClrProxyTests/bin/Release"
source(file.path(path, "R6-Generated.R"))
netLoadAssembly(file.path(path, "RDotNet.ClrProxyTests.dll"))

# Test properties with value types
propertiesOnly <- PropertiesOnly$new(Name = "Foo", Id = 4L)
expect_equal(propertiesOnly$Name, "Foo")
expect_equal(propertiesOnly$Id, 4L)
propertiesOnly$Price <- 1.23
expect_equal(propertiesOnly$Price, 1.23)
propertiesOnly$Timestamp <- as.POSIXct("2017-03-25")
expect_equal(propertiesOnly$Timestamp, as.POSIXct("2017-03-25"))
propertiesOnly$Elapsed <- as.difftime(150, units = "secs")
expect_equal(propertiesOnly$Elapsed, as.difftime(150, units = "secs"))
expect_equal(propertiesOnly$GetOnly, 1.23)
propertiesOnly$SetOnly <- 1.26

# Test properties with complex types
sample1 <- Sample1$new(PropertiesOnly = propertiesOnly)
expect_equal(sample1$PropertiesOnly$Name, "Foo")
sample1$PropertiesOnly <- PropertiesOnly$new(Name = "OtherFoo", Id = 5L)
expect_equal(sample1$PropertiesOnly$Name, "OtherFoo")

# Test Call method
sample1$MethodWithSimpleArgsNoReturn(1.23, "Args")
expect_equal(sample1$MethodWithSimpleArgs(1.23, "Args"), TRUE)
result <- sample1$MethodReturnsFullClass("MyFullClass")
expect_equal(result$Name, "MyFullClass")

# Test returns property or method as a list
l1 <- list(
  FullClass$new(Name = "Item1"),
  FullClass$new(Name = "Item2"),
  FullClass$new(Name = "Item3")
)
expect_null(sample1$ListOfFullClass)
sample1$ListOfFullClass <- l1
expect_true(is.list(sample1$ListOfFullClass))
expect_equal(length(sample1$ListOfFullClass), 3)
for(i in 1:length(sample1$ListOfFullClass)) {
  expect_true(inherits(sample1$ListOfFullClass[[i]], "FullClass"))
  expect_equal(sample1$ListOfFullClass[[i]]$Name, paste0("Item", i))
}

result <- sample1$MethodReturnListOfPropertiesOnly(3L)
expect_true(is.list(result))
expect_equal(length(result), 3)
for(i in 1:length(result)) {
  expect_true(inherits(result[[i]], "PropertiesOnly"))
  expect_equal(result[[i]]$Id, i)
}

result <- sample1$MethodTakeAListOfFullClass(l1)
expect_equal(result, 3L)
