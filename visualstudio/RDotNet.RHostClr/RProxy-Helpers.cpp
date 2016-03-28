#include "RProxy.h"

char* readStringFromSexp(SEXP p) {
	SEXP e = CAR(p);
	if(TYPEOF(e) != STRSXP || LENGTH(e) != 1) 
		error("[ERROR] ReadStringFromSexp: cannot parse string from SEXP: need a STRSXP of length 1\n");
	
	return (char*)CHAR(STRING_ELT(e, 0));
}

SAFEARRAY* readParametersFromSexp(SEXP p) {
	int length = Rf_length(p);
	if(length == 0) {
		return NULL;
	}

	SAFEARRAY* result = SafeArrayCreateVector(VT_I8, 0, length);

	LONG i;
	LONGLONG address;
	SEXP el;
	for(i = 0; i < length && p != R_NilValue; i++, p = CDR(p)) {
		el = CAR(p);
		address = (LONGLONG)el;
		SafeArrayPutElement(result, &i, &address);
	}

	return result;
}

IUnknown* readInstanceFromSexp(SEXP p) {
	SEXP e = CAR(p);
	
	if (e==R_NilValue) 
		error("[ERROR] ReadInstanceFromSexp: cannot parse .net object pointer from SEXP is null\n");

	if(TYPEOF(e) != EXTPTRSXP) 
		error("[ERROR] ReadInstanceFromSexp: cannot parse .net object pointer from SEXP: need a EXTPTRSXP\n");
	
	return (IUnknown*)R_ExternalPtrAddr(e);
}

LONGLONG readSingleParameterFromSexp(SEXP p) {
	SEXP e = CAR(p);

	return (LONGLONG)e;
}

SEXP convertToSEXP(CLR_OBJ& clrObj) {

	// Hold a reference on managed clrObj 
	CLR_OBJ* pclrObj = new CLR_OBJ(clrObj);
	
	// Rprintf("convert to SEXP from Type: %d\n", pclrObj->vt);
	
	switch (pclrObj->vt)
	{
		case VT_INT:
			return (SEXP) pclrObj->ullVal;
		default:
			return R_NilValue;
	}
}