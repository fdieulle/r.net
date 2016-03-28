pckPath <- "E:/Wokspace/Svn/R/R.Net"

source(file.path(pckPath, 'R/zzz.R'))
source(file.path(pckPath, 'R/net-exported.R'))

.onLoad()

#netRestartAppDomain()

f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")

netLoadAssembly(f)

source(file.path(pckPath, 'tests/TestCallStaticMethods.R'))
source(file.path(pckPath, 'tests/TestNetObjectInstances.R'))
source(file.path(pckPath, 'tests/TestList.R'))

type <- "RDotNet.AssemblyTest.StaticClass"

x <- netCallStatic(type, "CallWithEnum", "Default")
expect_equal("Default", netCall(x, "ToString"))
x <- netCallStatic(type, "CallWithEnum", "Value1")
expect_equal("Value1", netCall(x, "ToString"))
x <- netCallStatic(type, "CallWithEnum", "Value2")
expect_equal("Value2", netCall(x, "ToString"))
x <- netCallStatic(type, "CallWithEnum", "value2")
expect_equal("Value2", netCall(x, "ToString"))

enums <- c("Default", "Value1", "Value2")
x <- netCallStatic(type, "CallWithEnumVector", enums)
for(i in 1:length(enums))
  expect_equal(enums[i], netCall(x[[i]], "ToString"))