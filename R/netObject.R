#' @title 
#' NetObject class
#' 
#' @description
#' NetObject class to wrap a .Net external pointer 
#'
#' @section Properties:
#'
#' * `Ptr`: External pointer of the wrapped .Net object
#'
#' @section Methods:
#'
#' * `get(propertyName)`: Gets a property value
#' * `set(propertyName, value)`: Sets a property value
#' * `call(methodName, ...)`: Call a method
#' * `unwrap(value)`: Unwrap any NetObject or collection of NetObjects to external pointers
#' * `as(className)`: Cast the current R6 class to another by keeping the same .Net pointer
#' * `getType()`: Gets `NetType` description of wrapped .Net object
#'
#' @md
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' x <- netNew("RDotNet.AssemblyTest.OneCtorData", 21L)
#' netCall(x, "ToString")
#' }
NetObject <- R6Class("NetObject",
  private = list(
    ptr = NULL,
		type = NULL,
    getPtr = function(...) {
      if(!is.null(private$ptr)) {
        return(TRUE)
      }
      items = list(...)
      if("ptr" %in% names(items) & inherits(items[["ptr"]], "externalptr")) {
        private$ptr <- items[["ptr"]]
        return(TRUE)
      }
      return(FALSE)
    }
  ),
  active = list(
    Ptr = function(value) {
      if(missing(value)) return(private$ptr)
      else invisible(private$ptr <- value)
    }
  ),
  public = list(
    initialize = function(...) {
      private$getPtr(...)
      
      if(!is.null(private$ptr)) {
        items = list(...)
        for(name in names(items)) {
          if(name != "ptr") {
            self$set(name, items[[name]])
          }
        }
      }
    },
    get = function(propertyName) {
      return(netGet(private$ptr, propertyName))
    },
    set = function(propertyName, value) {
      value <- self$unwrap(value)
			invisible(netSet(private$ptr, propertyName, value))
    },
    call = function(methodName, ...) {
      parameters <- list(
        private$ptr,
        methodName)
			parameters <- c(parameters, self$unwrap(list(...)))
			return(do.call(netCall, parameters))
    },
    unwrap = function(value) {
      if(inherits(value, "NetObject")) {
        return(value$Ptr)
      } else if(is.list(value) & length(value) > 0) {
        for(i in 1:length(value)) {
          value[[i]] <- self$unwrap(value[[i]])
        }
      }
      return(value)
    },
		as = function(className) {
			return(do.call(get(className)$new, list(ptr = private$ptr))) 
		},
		getType = function() {
			type <- self$call("GetType")
			return(NetType$new(netGet(type, "Name"), netGet("Namespace")))
		}
  )
)

#' @title 
#' NetType class
#' 
#' @description
#' NetType provide the name and namespace of a .Net type
#'
#' @section Properties:
#'
#' * `Name`: .Net type
#' * `Namespace`: .Net type namespace
#' * `FullName`: Full .Net type
#'
#' @md
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' x <- netNew("RDotNet.AssemblyTest.OneCtorData", 21L)
#' netCall(x, "ToString")
#' }
NetType <- R6Class("NetType",
	private = list(
		name = NULL,
		namespace = NULL,
		fullName = NULL
	),
	active = list(
    Name = function(value) {
      if(missing(value)) return(name)
    },
		Namespace = function(value) {
      if(missing(value)) return(namespace)
    },
		FullName = function(value) {
      if(missing(value)) return(fullName)
    }
  ),
	public = list(
		initialize = function(namespace, name) {
			private$namespace = namespace
			private$name = name
			private$fullName = paste(namespace, name, sep = ".")
		}
	)
)
