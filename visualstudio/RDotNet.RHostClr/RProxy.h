#pragma region Includes and Imports

#include "RuntimeProxy.h"

#include <R.h>
#include <Rinternals.h>

#pragma endregion

// Define types

typedef variant_t CLR_OBJ;

// Define exported methods.

#ifdef __cplusplus
extern "C" {
#endif
	// ClrEnvironment methods
	void rStartClr(char** appBaseDir);
	void rShutdownClr();
	void rRestartAppDomain();
	
	void rLoadAssembly(char** fileName);

	// Proxy methods
	SEXP rCallStaticMethod(SEXP p);
	SEXP rCreateObject(SEXP p);
	SEXP rCall(SEXP p);
	SEXP rGet(SEXP p);
	SEXP rSet(SEXP p);

#ifdef __cplusplus
} // end of extern "C" block
#endif

// Define internal methods.

// Helpers methods
char* readStringFromSexp(SEXP p);
SAFEARRAY* readParametersFromSexp(SEXP p);
IUnknown* readInstanceFromSexp(SEXP p);
SEXP convertToSEXP(CLR_OBJ &obj);