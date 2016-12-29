#include "RProxy.h"

SEXP rCallStaticMethod(SEXP p) {
	
	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	char* strTypeName = readStringFromSexp(p); p = CDR(p);
	char* strMethodName = readStringFromSexp(p); p = CDR(p);
	SAFEARRAY* parameters = readParametersFromSexp(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
	long i = 0;
	
	// 2.1 - Type name
	variant_t typeName(strTypeName);
	SafeArrayPutElement(args, &i, &typeName); i++;
	
	// 2.2 - Method name
	variant_t methodName(strMethodName);
	SafeArrayPutElement(args, &i, &methodName); i++;

	// 2.3 parameters
	VARIANT* pvt = new variant_t();
	VariantInit(pvt);
	pvt->vt = VT_ARRAY | VT_I8;
	pvt->parray = parameters;
	SafeArrayPutElement(args, &i, pvt); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"CallStaticMethod", args, &result, &errorMsg);
	
	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netCallStatic !");
		free(errorMsg);
		return R_NilValue;
	}

	// 4 - Free memory
	SafeArrayDestroy(parameters);
	SafeArrayDestroy(args);
	//free_variant_array(pvt);
	//free(strTypeName);
	//free(strMethodName);

	return convertToSEXP(result);
}

SEXP rGetStatic(SEXP p) {

	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	char* strTypeName = readStringFromSexp(p); p = CDR(p);
	char* strPropertyName = readStringFromSexp(p); p = CDR(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 2);
	long i = 0;

	// 2.1 - Type name
	variant_t typeName(strTypeName);
	SafeArrayPutElement(args, &i, &typeName); i++;

	// 2.2 - Method name
	variant_t propertyName(strPropertyName);
	SafeArrayPutElement(args, &i, &propertyName); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"GetStaticProperty", args, &result, &errorMsg);
	
	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netGetStatic !");
		free(errorMsg);
		return R_NilValue;
	}

	// 4 - Free memory
	SafeArrayDestroy(args);

	return convertToSEXP(result);
}

SEXP rSetStatic(SEXP p) {
	
	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	char* strTypeName = readStringFromSexp(p); p = CDR(p);
	char* strPropertyName = readStringFromSexp(p); p = CDR(p);
	LONGLONG valueAddresse = (LONGLONG)CAR(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
	long i = 0;

	// 2.1 - Type name
	variant_t typeName(strTypeName);
	SafeArrayPutElement(args, &i, &typeName); i++;

	// 2.2 - Method name
	variant_t propertyName(strPropertyName);
	SafeArrayPutElement(args, &i, &propertyName); i++;

	// 2.3 property value
	variant_t value(valueAddresse);
	SafeArrayPutElement(args, &i, &value); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"SetStaticProperty", args, &result, &errorMsg);

	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netSetStatic !");
		free(errorMsg);
		return R_NilValue;
	}

	SafeArrayDestroy(args);
	return R_NilValue;
}

SEXP rCreateObject(SEXP p) {

	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	char* strTypeName = readStringFromSexp(p); p = CDR(p);
	SAFEARRAY* parameters = readParametersFromSexp(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 2);
	long i = 0;
	
	// 2.1 - Type name
	variant_t typeName(strTypeName);
	SafeArrayPutElement(args, &i, &typeName); i++;

	// 2.2 parameters
	VARIANT* pvt = new variant_t();
	VariantInit(pvt);
	pvt->vt = VT_ARRAY | VT_I8;
	pvt->parray = parameters;
	SafeArrayPutElement(args, &i, pvt); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"CreateInstance", args, &result, &errorMsg);
	
	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netNew !");
		free(errorMsg);
		return R_NilValue;
	}

	// 4 - Free memory
	SafeArrayDestroy(parameters);
	SafeArrayDestroy(args);
	//free_variant_array(pvt);
	//free(strTypeName);
	//free(strMethodName);

	return convertToSEXP(result);
}

SEXP rCall(SEXP p) {
	
	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	IUnknown* instancePtr = readInstanceFromSexp(p); p = CDR(p);
	char* strMethodName = readStringFromSexp(p); p = CDR(p);
	SAFEARRAY* parameters = readParametersFromSexp(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
	long i = 0;

	// 2.1 - external pointer on .Net object 
	variant_t instance(instancePtr);
	SafeArrayPutElement(args, &i, &instance); i++;

	// 2.2 - Method name
	variant_t methodName(strMethodName);
	SafeArrayPutElement(args, &i, &methodName); i++;

	// 2.3 parameters
	VARIANT* pvt = new variant_t();
	VariantInit(pvt);
	pvt->vt = VT_ARRAY | VT_I8;
	pvt->parray = parameters;
	SafeArrayPutElement(args, &i, pvt); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"CallMethod", args, &result, &errorMsg);
	
	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netCall !");
		free(errorMsg);
		return R_NilValue;
	}

	// 4 - Free memory
	SafeArrayDestroy(parameters);
	SafeArrayDestroy(args);

	return convertToSEXP(result);
}

SEXP rGet(SEXP p) {

	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	IUnknown* instancePtr = readInstanceFromSexp(p); p = CDR(p);
	char* strPropertyName = readStringFromSexp(p); p = CDR(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 2);
	long i = 0;

	// 2.1 - external pointer on .Net object 
	variant_t instance(instancePtr);
	SafeArrayPutElement(args, &i, &instance); i++;

	// 2.2 - Method name
	variant_t propertyName(strPropertyName);
	SafeArrayPutElement(args, &i, &propertyName); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"GetProperty", args, &result, &errorMsg);
	
	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netGet !");
		free(errorMsg);
		return R_NilValue;
	}

	// 4 - Free memory
	SafeArrayDestroy(args);

	return convertToSEXP(result);
}

SEXP rSet(SEXP p) {
	
	// 1 - Get data from SEXP
	p = CDR(p); // Skip the first parameter: because function name
	IUnknown* instancePtr = readInstanceFromSexp(p); p = CDR(p);
	char* strPropertyName = readStringFromSexp(p); p = CDR(p);
	LONGLONG valueAddresse = (LONGLONG)CAR(p);

	// 2 - Prepare arguments to call proxy
	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
	long i = 0;

	// 2.1 - external pointer on .Net object 
	variant_t instance(instancePtr);
	SafeArrayPutElement(args, &i, &instance); i++;

	// 2.2 - Method name
	variant_t propertyName(strPropertyName);
	SafeArrayPutElement(args, &i, &propertyName); i++;

	// 2.3 property value
	variant_t value(valueAddresse);
	SafeArrayPutElement(args, &i, &value); i++;

	// 3 - Call the proxy
	CLR_OBJ result;
	char* errorMsg;
	HRESULT hr = callProxy(L"SetProperty", args, &result, &errorMsg);

	if(FAILED(hr)) {
		Rprintf(errorMsg);
		error("Exception during netSet !");
		free(errorMsg);
		return R_NilValue;
	}

	SafeArrayDestroy(args);
	return R_NilValue;
}