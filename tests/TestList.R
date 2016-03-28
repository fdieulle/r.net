library(testthat)

testList <- function(expected, value) {
  
  expect_equal(class(expected), class(value))
  expect_equal(length(expected), length(value))
  for(i in 1:length(expected)) {
    if(is.list(expected[[i]])) {
      testList(expected[[i]], value[[i]])
    } else {
        expect_equal(expected[[i]], value[[i]])
        expect_equal(netCall(expected[[i]], "ToString"), netCall(value[[i]], "ToString")) 
    }
  }
}

testDico <- function(expected, value) {
 
  expect_equal(names(expected), names(value)) 
  testList(expected, value)
}

type <- "RDotNet.AssemblyTest.OneCtorData"
l1 <- list(
  netNew(type, 100L),
  netNew(type, 101L),
  netNew(type, 102L)
)

# Test list to Array
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedArray", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToArray", l1)
testList(l1, result)


# Test list to List<>
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedList", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIList", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedICollection", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIEnumerable", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToIList", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToICollection", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToIEnumerable", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIReadOnlyList", l1)
testList(l1, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIReadOnlyCollection", l1)
testList(l1, result)

# Test named list to Dictionary<string,>
l2 <- list(
  netNew(type, 1L),
  netNew(type, 2L),
  netNew(type, 3L)
)
names(l2) <- c("Col 1", "Col 2", "Col 3")
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToDictionary", l2)
testDico(l2, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToIDictionary", l2)
testDico(l2, result)
#result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedICollection", l2)
#testList(l2, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIEnumerable", l2)
testList(l2, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToDicoICollection", l2)
testList(l2, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToDicoIEnumerable", l2)
testList(l2, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIReadOnlyDictionary", l2)
testDico(l2, result)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedIReadOnlyCollection", l2)
testList(l2, result)

# Test recursive list to List<List<List<>>>
l3 <- list(
  list(
    list(netNew(type, 1L), netNew(type, 2L), netNew(type, 3L)),
    list(netNew(type, 4L))
  ),
  list(
    list(netNew(type, 5L), netNew(type, 6L), netNew(type, 7L), netNew(type, 8L)),
    list(netNew(type, 9L), netNew(type, 10L))
  ),
  list(
    list(netNew(type, 11L), netNew(type, 12L), netNew(type, 13L), netNew(type, 14L), netNew(type, 15L)),
    list(netNew(type, 16L), netNew(type, 17L))
  )
)
result <- netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithListToTypedListRecurse", l3)
testList(l3, result)
