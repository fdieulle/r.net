library(r.net)
library(R6)
library(testthat)

path <- "E:/Workspace/GitHub/r.net/visualstudio/RDotNet.AssemblyTest/bin/Release"

netLoadAssembly(file.path(path, "RDotNet.AssemblyTest.dll"))
netGenerateR6(c(
  "RDotNet.AssemblyTest.Model.ClassWithDependencies",
  "RDotNet.AssemblyTest.Model.IData"), filePath = file.path(path, "R6-Generated.R"), withInheritedTypes = TRUE)
source(file.path(path, "R6-Generated.R"))

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
propertiesOnly$ArrayOfString <- c("Test", "Test2", "Test3")
expect_equal(propertiesOnly$ArrayOfString, c("Test", "Test2", "Test3"))
propertiesOnly$ArrayOfString <- character(0)
expect_equal(propertiesOnly$ArrayOfString, character(0))
propertiesOnly$ListOfString <- c("Test", "Test2", "Test3")
expect_equal(propertiesOnly$ListOfString, c("Test", "Test2", "Test3"))
propertiesOnly$ListOfString <- character(0)
expect_equal(propertiesOnly$ListOfString, character(0))


# Test properties with complex types
sample2 <- Sample2$new(Name = "Sample2", Id = 2L)
expect_equal(sample2$Name, "Sample2")
expect_equal(sample2$Id, 2L)
sample1 <- Sample1$new(Sample2 = sample2)
expect_equal(sample1$Sample2$Name, "Sample2")
expect_equal(sample1$Sample2$Id, 2L)
sample1$Sample2 <- Sample2$new(Name = "OtherSample2", Id = 5L)
expect_equal(sample1$Sample2$Name, "OtherSample2")

# Test properties with array or list of complex types
sample1$ArrayOfSample2 <- c(Sample2$new(Id = 1L), Sample2$new(Id = 2L), Sample2$new(Id = 3L))
expect_equal(length(sample1$ArrayOfSample2), 3)
expect_equal(sample1$ArrayOfSample2[[1]]$Id, 1L)
expect_equal(sample1$ArrayOfSample2[[2]]$Id, 2L)
expect_equal(sample1$ArrayOfSample2[[3]]$Id, 3L)
sample1$ArrayOfSample2 <- list(Sample2$new(Id = 4L), Sample2$new(Id = 5L), Sample2$new(Id = 6L))
expect_equal(length(sample1$ArrayOfSample2), 3)
expect_equal(sample1$ArrayOfSample2[[1]]$Id, 4L)
expect_equal(sample1$ArrayOfSample2[[2]]$Id, 5L)
expect_equal(sample1$ArrayOfSample2[[3]]$Id, 6L)
sample1$ListOfSample2 <- c(Sample2$new(Id = 1L), Sample2$new(Id = 2L), Sample2$new(Id = 3L))
expect_equal(length(sample1$ListOfSample2), 3)
expect_equal(sample1$ListOfSample2[[1]]$Id, 1L)
expect_equal(sample1$ListOfSample2[[2]]$Id, 2L)
expect_equal(sample1$ListOfSample2[[3]]$Id, 3L)
sample1$ListOfSample2 <- list(Sample2$new(Id = 4L), Sample2$new(Id = 5L), Sample2$new(Id = 6L))
expect_equal(length(sample1$ListOfSample2), 3)
expect_equal(sample1$ListOfSample2[[1]]$Id, 4L)
expect_equal(sample1$ListOfSample2[[2]]$Id, 5L)
expect_equal(sample1$ListOfSample2[[3]]$Id, 6L)


# Test Call method
sample1$MethodNoReturn()
sample1$MethofNoReturn("Test")
expect_equal(sample1$MethodReturnArgument("Test"), "Test")
expect_null(sample1$InvisibleMethod)

result <- sample1$MethodReturnSample2("MySample2Name")
expect_equal(class(result), c("Sample2", "NetObject", "R6"))
expect_equal(result$Name, "MySample2Name")

result <- sample1$MethodReturnListOfSample2(c(Sample2$new(Id = 1L), Sample2$new(Id = 2L), Sample2$new(Id = 3L)))
expect_true(is.list(result))
expect_equal(length(result), 3)
expect_equal(result[[1]]$Id, 1L)
expect_equal(result[[2]]$Id, 2L)
expect_equal(result[[3]]$Id, 3L)

result <- sample1$MethodReturnListOfSample2(list(Sample2$new(Id = 4L), Sample2$new(Id = 5L)))
expect_true(is.list(result))
expect_equal(length(result), 2)
expect_equal(result[[1]]$Id, 4L)
expect_equal(result[[2]]$Id, 5L)

result <- sample1$MethodReturnArrayOfSample2(c(Sample2$new(Id = 1L), Sample2$new(Id = 2L), Sample2$new(Id = 3L)))
expect_true(is.list(result))
expect_equal(length(result), 3)
expect_equal(result[[1]]$Id, 1L)
expect_equal(result[[2]]$Id, 2L)
expect_equal(result[[3]]$Id, 3L)

result <- sample1$MethodReturnArrayOfSample2(list(Sample2$new(Id = 4L), Sample2$new(Id = 5L)))
expect_true(is.list(result))
expect_equal(length(result), 2)
expect_equal(result[[1]]$Id, 4L)
expect_equal(result[[2]]$Id, 5L)

result <- sample1$MethoReturnInterface()
expect_equal(class(result), c("IData", "NetObject", "R6"))
expect_equal(result$Name, "Sample3")
sample3 <- result$as("Sample3")
expect_equal(class(sample3), c("Sample3", "IData", "NetObject", "R6"))

sample1$DataAsInterface <- Sample4$new("Sample4", 4L)
result <- sample1$DataAsInterface
expect_equal(class(result), c("IData", "NetObject", "R6"))
expect_equal(result$Name, "Sample4")
sample3 <- result$as("Sample3")
expect_equal(class(sample3), c("Sample3", "IData", "NetObject", "R6"))
sample3$Name <- "NewName"
expect_equal(sample1$DataAsInterface$Name, "NewName")
expect_equal(sample1$DataAsInterface$get("Id"), 4L)
sample4 <- result$as("Sample4")
expect_equal(class(sample4), c("Sample4", "Sample3", "IData", "NetObject", "R6"))
expect_equal(sample4$Id, 4L)


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
