#include "RuntimeHost.h"

// Todo: following values can be set as parameters
//PCWSTR pszClrVersion = L"v4.0.30319";

// CLR data
ICLRMetaHost* pMetaHost = NULL;
ICLRRuntimeInfo* pRuntimeInfo = NULL;
ICLRRuntimeHost* pRuntimeHost = NULL;
ICorRuntimeHost* pCorRuntimeHost = NULL;

HRESULT startClr(const char* clrVersion, char** errorMsg) {

	// 
    // Load and start the .NET runtime.
    // 


	HRESULT hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
	if(FAILED(hr)) {
		*errorMsg = "CLRCreateInstance failed";
		stopClr();
		return hr;
	}

	wchar_t* sClrVersion = convertToWChar(clrVersion);

	// Get the ICLRRuntimeInfo corresponding to a particular CLR version. It 
    // supersedes CorBindToRuntimeEx with STARTUP_LOADER_SAFEMODE.
	hr = pMetaHost->GetRuntime(sClrVersion, IID_PPV_ARGS(&pRuntimeInfo));
	free(sClrVersion);
	if(FAILED(hr)) {
		*errorMsg = "ICLRMetaHost->GetRuntime failed";
		stopClr();
		return hr;
	}
	
	// Check if the specified runtime can be loaded into the process. This 
    // method will take into account other runtimes that may already be 
    // loaded into the process and set pbLoadable to TRUE if this runtime can 
    // be loaded in an in-process side-by-side fashion. 
	BOOL fLoadable;
	hr = pRuntimeInfo->IsLoadable(&fLoadable);
	if(FAILED(hr)) {
		*errorMsg = "ICLRRuntimeInfo->IsLoadable failed";
		stopClr();
		return hr;
	}

	if(!fLoadable) {
		const char* m = ".NET runtime %s cannot be loaded";
		size_t l = strlen(m) + strlen(clrVersion);
		char* dest = new char[l];
		sprintf_s(dest, l, m, clrVersion);
		*errorMsg = dest;
		stopClr();
		return hr;
	}

	// Load the CLR into the current process and return a runtime interface 
    // pointer. ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting  
    // interfaces supported by CLR 4.0. Here we demo the ICLRRuntimeHost 
    // interface that was provided in .NET v2.0 to support CLR 2.0 new 
    // features. ICLRRuntimeHost does not support loading the .NET v1.x 
    // runtimes.
	hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&pRuntimeHost));
	if(FAILED(hr)) {
		*errorMsg = "ICLRRuntimeInfo->GetInterface failed";
		stopClr();
		return hr;
	}	

	// Start the CLR.
	hr = pRuntimeHost->Start();
    if (FAILED(hr)) {
		*errorMsg = "CLR failed to start";
        stopClr();
		return hr;
    }
		
	// Get ICorRuntimeHost pointer
	hr = pRuntimeInfo->GetInterface(CLSID_CorRuntimeHost, IID_PPV_ARGS(&pCorRuntimeHost));
	if(FAILED(hr)) {
		*errorMsg = "Error when getting the ICorRuntimeHost";
		stopClr();
		return hr;
	}
		
	// Obtain the default AppDomain object
	IUnknown* appDomainUnknown;
	hr = pCorRuntimeHost->GetDefaultDomain(&appDomainUnknown);
	if(FAILED(hr)) {
		*errorMsg = "Error when getting the ICorRuntimeHost";
		stopClr();
		return hr;
	}

	return hr;
}

void stopClr() {

	if(pMetaHost) {
		pMetaHost->Release();
		pMetaHost = NULL;
	}

	if(pRuntimeInfo) {
		pRuntimeInfo->Release();
		pRuntimeInfo = NULL;
	}

	if(pRuntimeHost) {
		pRuntimeHost->Release();
		pRuntimeHost = NULL;
	}
}

HRESULT loadDomain(const char* name, const char* appBaseDir, const char* appConfigFile, mscorlib::_AppDomain** appDomain, char** errorMsg) {

	IUnknown* appDomainSetupUnkown;
	HRESULT hr = pCorRuntimeHost->CreateDomainSetup(&appDomainSetupUnkown);
	if(FAILED(hr)) {
		*errorMsg = "ICorRuntimeHost->CreateDomainSetup failed.";
		return hr;
	}

	mscorlib::IAppDomainSetup* appDomainSetup = NULL;
	hr = appDomainSetupUnkown->QueryInterface(__uuidof(mscorlib::IAppDomainSetup), (void**)&appDomainSetup);
	if (FAILED(hr))	{
		*errorMsg = "IUnknown->QueryInterface failed getting IAppDomainSetup.";
		return hr;
	}

	if (appDomainSetup == NULL) {
		*errorMsg = "Getting IAppDomainSetup returned NULL.";
		return hr;
	}

	bstr_t appBaseDirectory(appBaseDir);
	appDomainSetup->put_ApplicationBase(appBaseDirectory);

	bstr_t shadowCopyFiles("false");
	appDomainSetup->put_ShadowCopyFiles(shadowCopyFiles);

	bstr_t domainName(name);
	appDomainSetup->put_ApplicationName(domainName);

	if(appConfigFile) {
		bstr_t appConfigFilePath(appConfigFile);
		appDomainSetup->put_ConfigurationFile(appConfigFilePath);
	}
	
	// Create the new AppDomain
	IUnknown* appDomainUnknown;
	hr = pCorRuntimeHost->CreateDomainEx(L".Net Proxy", appDomainSetupUnkown, NULL,  &appDomainUnknown);
	if(FAILED(hr)) {
		*errorMsg = "ICorRuntimeHost->CreateDomainEx failed.";
		return hr;
	}

	if (appDomainUnknown == NULL) {
		*errorMsg = "ICorRuntimeHost->CreateDomainEx returned NULL.";
		return hr;
	}

	mscorlib::_AppDomain* pAppDomain;
	hr = appDomainUnknown->QueryInterface(__uuidof(mscorlib::_AppDomain), (void**)&pAppDomain);
	if(FAILED(hr)) {
		*errorMsg = "Failed to get AppDomain interface.";
		return hr;
	}
	
	*appDomain = pAppDomain;

	return hr;
}

HRESULT unloadDomain(mscorlib::_AppDomain* appDomain, char** errorMsg) {
	HRESULT hr = pCorRuntimeHost->UnloadDomain(appDomain);
	if(FAILED(hr)) {
		*errorMsg = "Failed to unload appDomain";
	}

	return hr;
}

void printHResult(HRESULT hr) {
	
	if(FACILITY_WINDOWS == HRESULT_FACILITY(hr)) {
		hr = HRESULT_CODE(hr);
	}

	TCHAR* szErrMsg;
	if(FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM,
		NULL, hr, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&szErrMsg, 0, NULL) != 0) {
			wprintf(L"%s\n", szErrMsg);
			LocalFree(szErrMsg);
	} else {
		wprintf(L"Could not find a descirption for error # %#x.\n", hr);
	}
}

wchar_t* convertToWChar(const char* from) {

	size_t size = strlen(from) + 1;
	wchar_t* to = new wchar_t[size];
	size_t outSize;
    mbstowcs_s(&outSize, to, size, from, size-1);

	return to;
}