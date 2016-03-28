#include "RProxy.h"

mscorlib::_AppDomain* appDomain = NULL;
char* appBaseDirectory = NULL;

void rStartClr(char** appBaseDir) {

	const char* clrVersion = "v4.0.30319";

	Rprintf("Load and start .Net runtime %s \n", clrVersion);

	appBaseDirectory = appBaseDir[0];

	char* errorMsg;
	HRESULT hr = startClr(clrVersion, &errorMsg);

	if(FAILED(hr)) {
		error("CLR failed to start: %s \n", errorMsg);
		return;
	}

	rRestartAppDomain();

	return;
}

void rShutdownClr() {
	
	Rprintf("Stop and shutting down .Net runtime \n");
	
	stopClr();
}

void rRestartAppDomain() {

	HRESULT hr;
	char* errorMsg;

	unloadClrProxy();

	if(appDomain) {
		Rprintf("Unload Appdomain ... \n");

		hr = unloadDomain(appDomain, &errorMsg);
		if(FAILED(hr)) {
			error("% \n", errorMsg);
			return;
		}

		appDomain->Release();
		appDomain = NULL;
	}

	hr = loadDomain("R.Net", appBaseDirectory, NULL, &appDomain, &errorMsg);
	if(FAILED(hr)) {
		error("Unexpected error when loading AppDomain: %s \n", errorMsg);
		return;
	}

	if(appDomain == NULL) {
		error("AppDomain is null: %s \n", errorMsg);
	}

	hr = loadClrProxy(appDomain, appBaseDirectory, &errorMsg);
	if(FAILED(hr)) {
		error("Unexpected error when loading ClrProxy assembly: %s \n", errorMsg);
		return;
	}

	Rprintf("R.Net AppDomain is loaded \n");
}

// Todo: Maybe we can use an SEXP struct instead of char** from .C() call from R
void rLoadAssembly(char** fileName) {

	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 1);
	long i=0;
	variant_t assemblyName(fileName[0]);
	SafeArrayPutElement(args, &i, &assemblyName);

	char* errorMsg;
	CLR_OBJ result;
	HRESULT hr = callProxy(L"LoadAssembly", args, &result, &errorMsg);
	
	if(FAILED(hr)) {
		error(errorMsg);
		free(errorMsg);
		return;
	}

	SafeArrayDestroy(args);
	return;
}