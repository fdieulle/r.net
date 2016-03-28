library(testthat)

x <- netNew("RDotNet.AssemblyTest.DefaultCtorData")
s <- netCall(x, "ToString")
expect_equal("RDotNet.AssemblyTest.DefaultCtorData", s)

x <- netNew("RDotNet.AssemblyTest.OneCtorData", 21L)
s <- netCall(x, "ToString")
expect_equal("RDotNet.AssemblyTest.OneCtorData #21", s)

x <- netNew("RDotNet.AssemblyTest.ManyCtorData")
s <- netCall(x, "ToString")
expect_equal("RDotNet.AssemblyTest.ManyCtorData Name=Default ctor #-1", s)

x <- netNew("RDotNet.AssemblyTest.ManyCtorData", 56L)
s <- netCall(x, "ToString")
expect_equal("RDotNet.AssemblyTest.ManyCtorData Name=Integer ctor #56", s)

x <- netNew("RDotNet.AssemblyTest.ManyCtorData", "instance name")
s <- netCall(x, "ToString")
expect_equal("RDotNet.AssemblyTest.ManyCtorData Name=String Ctor instance name", s)

type <- "RDotNet.AssemblyTest.DefaultCtorData"
x <- netNew(type)
netSet(x, "Name", "Test")
xName <- netGet(x, "Name")
expect_equal("Test", xName)

netSet(x, "Integers", c(12L, 23L))
xIntegers <- netGet(x, "Integers")
expect_equal(c(12L, 23L), xIntegers)

o <- netNew("RDotNet.AssemblyTest.OneCtorData", 24L)
netSet(x, "OneCtorData", o)
xO <- netGet(x, "OneCtorData")
expect_equal(o, xO)
expect_equal(netCall(o, "ToString"), netCall(xO, "ToString"))
