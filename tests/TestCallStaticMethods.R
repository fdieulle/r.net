library(testthat)
type <- "RDotNet.AssemblyTest.StaticClass"

testTypeMapping <- function() {
  netCallStatic(type, "SameMethodName")
  
  netCallStatic(type, "SameMethodName", 1L)
  netCallStatic(type, "SameMethodName", 1.23)
  netCallStatic(type, "SameMethodName", c(2L, 3L))
  netCallStatic(type, "SameMethodName", c(1.24, 1.25))
  
  netCallStatic(type, "SameMethodName", 2.13, 1L)
  netCallStatic(type, "SameMethodName", 2.14, c(2L, 3L))
  netCallStatic(type, "SameMethodName", c(1.24, 1.25), 14L)
  netCallStatic(type, "SameMethodName", c(1.24, 1.25), c(14L, 15L))
}

testCallStatic <- function(typeName, methodName, parameter) {
  x <- netCallStatic(typeName, methodName, parameter)
  expect_equal(class(x), class(parameter))
  expect_equal(x, parameter)
}

testCallStaticPrimitiveTypes <- function() {
  testCallStatic(type, "CallWithInteger", 2L)
  testCallStatic(type, "CallWithIntegerVector", c(2L, 3L))
  testCallStatic(type, "CallWithIntegerMatrix", matrix(nrow = 7, ncol = 5, data = 24L))
  testCallStatic(type, "CallWithNumeric", 23)
  testCallStatic(type, "CallWithNumericVector", c(1.24, 1.25))
  testCallStatic(type, "CallWithNumericMatrix", matrix(nrow = 3, ncol = 7, data = 12.235))
  testCallStatic(type, "CallWithLogical", TRUE)
  testCallStatic(type, "CallWithLogicalVector", c(TRUE, FALSE, TRUE))
  testCallStatic(type, "CallWithLogicalMatrix", matrix(nrow = 3, ncol = 5, data = TRUE))
  testCallStatic(type, "CallWithCharacter", "Hello")
  testCallStatic(type, "CallWithCharacterVector", c("Hello", "C#", "It's R"))
  #Todo: fix this uc: testCallStatic(type, "CallWithCharacterMatrix", matrix(nrow = 3, ncol = 3, data = "Toto"))
  
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
}

testTimezone <- function(timezone, convertedtimezone = NULL) {
  if(is.null(convertedtimezone)) {
    convertedtimezone <- timezone
  }
  
  offset = 0
  for(i in 1:1000) {
    offset = offset + 77760000
    p <- as.POSIXct(offset, origin = "1960-01-01", tz = timezone)
    x <- netCallStatic(type, "CallWithPOSIXctOrPOSIXltOrDate", p)
    expect_equal(class(x), class(p))
    expect_equal(attr(x, "tzone"), convertedtimezone)
    d <- as.POSIXct(as.numeric(x), origin = "1970-01-01", tz = timezone)
    expect_equal(x, d)
  }
}

testDateTime <- function() {
  x <- netCallStatic(type, "GetNow")
  expect_equal(class(x), class(Sys.time()))
  expect_equal(attr(x, "tzone"), Sys.timezone(location = TRUE))
  d <- as.POSIXct(as.numeric(x), origin = "1970-01-01", tz = Sys.timezone(location = TRUE))
  expect_equal(x, d)
  
  x <- netCallStatic(type, "GetUtcNow")
  expect_equal(class(x), class(Sys.time()))
  expect_equal(attr(x, "tzone"), "Etc/GMT")
  d <- as.POSIXct(as.numeric(x), origin = "1970-01-01", tz = "Etc/GMT")
  expect_equal(x, d)
  
  testTimezone("Europe/Paris")
  #testTimezone("UTC")
  #testTimezone("America/New_York")
  #testTimezone("UTC", "Europe/Paris")
}

testDifftime <- function() {
  type <- "RDotNet.AssemblyTest.StaticClass"
  
  t1 <- as.POSIXct("2016-03-20 10:32:21")
  t2 <- as.POSIXct("2016-04-20 10:42:00")
  
  dt <- difftime(t2, t1, units = "auto")
  result <- netCallStatic(type, "CallWithDifftime", dt)
  expect_equal(difftime(t2, t1, units = "secs"), result)
  dt <- difftime(t2, t1, units = "secs")
  result <- netCallStatic(type, "CallWithDifftime", dt)
  expect_equal(difftime(t2, t1, units = "secs"), result)
  dt <- difftime(t2, t1, units = "mins")
  result <- netCallStatic(type, "CallWithDifftime", dt)
  expect_equal(difftime(t2, t1, units = "secs"), result)
  dt <- difftime(t2, t1, units = "hours")
  result <- netCallStatic(type, "CallWithDifftime", dt)
  expect_equal(difftime(t2, t1, units = "secs"), result)
  dt <- difftime(t2, t1, units = "days")
  result <- netCallStatic(type, "CallWithDifftime", dt)
  expect_equal(difftime(t2, t1, units = "secs"), result)
  dt <- difftime(t2, t1, units = "weeks")
  result <- netCallStatic(type, "CallWithDifftime", dt)
  expect_equal(difftime(t2, t1, units = "secs"), result)
  
  x <- seq(Sys.time(), by = '10 min', length = 10)
  dt <- difftime(head(x, -1), tail(x, -1), units = "auto")
  result <- netCallStatic(type, "CallWithDifftimeVector", dt)
  units(dt) <- "secs"
  expect_equal(dt, result)
  dt <- difftime(head(x, -1), tail(x, -1), units = "secs")
  result <- netCallStatic(type, "CallWithDifftimeVector", dt)
  units(dt) <- "secs"
  expect_equal(dt, result)
  dt <- difftime(head(x, -1), tail(x, -1), units = "mins")
  result <- netCallStatic(type, "CallWithDifftimeVector", dt)
  units(dt) <- "secs"
  expect_equal(dt, result)
  dt <- difftime(head(x, -1), tail(x, -1), units = "hours")
  result <- netCallStatic(type, "CallWithDifftimeVector", dt)
  units(dt) <- "secs"
  expect_equal(dt, result)
  dt <- difftime(head(x, -1), tail(x, -1), units = "days")
  result <- netCallStatic(type, "CallWithDifftimeVector", dt)
  units(dt) <- "secs"
  expect_equal(dt, result)
  dt <- difftime(head(x, -1), tail(x, -1), units = "weeks")
  result <- netCallStatic(type, "CallWithDifftimeVector", dt)
  units(dt) <- "secs"
  expect_equal(dt, result)
  
  x <- seq(Sys.time(), by = '10 min', length = 16)
  dt <- difftime(head(x, -1), tail(x, -1))
  m <- as.difftime(matrix(nrow = 3, ncol = 5, data = dt), units="secs")
  result <- netCallStatic(type, "CallWithDifftimeMatrix", m)
  units(m) <- "secs"
  expect_equal(m, result)
  m <- as.difftime(matrix(nrow = 3, ncol = 5, data = dt), units="mins")
  result <- netCallStatic(type, "CallWithDifftimeMatrix", m)
  units(m) <- "secs"
  expect_equal(m, result)
  m <- as.difftime(matrix(nrow = 3, ncol = 5, data = dt), units="hours")
  result <- netCallStatic(type, "CallWithDifftimeMatrix", m)
  units(m) <- "secs"
  expect_equal(m, result)
  m <- as.difftime(matrix(nrow = 3, ncol = 5, data = dt), units="days")
  result <- netCallStatic(type, "CallWithDifftimeMatrix", m)
  units(m) <- "secs"
  expect_equal(m, result)
  m <- as.difftime(matrix(nrow = 3, ncol = 5, data = dt), units="weeks")
  result <- netCallStatic(type, "CallWithDifftimeMatrix", m)
  units(m) <- "secs"
  expect_equal(m, result)
}

testTypeMapping()
testCallStaticPrimitiveTypes()
testDateTime()
testDifftime()

