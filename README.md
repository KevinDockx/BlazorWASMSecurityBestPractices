# Blazor WASM: Security Best Practice
Sample showing a best practice security approach for Blazor WASM apps via the BFF pattern.  To run the sample, ensure all three solutions (IDP, API, BlazorBFF) are started.  

- Marvin.IDP: the OIDC identity provider
- Marvin.API: an demo "external" API
- Marvin.BlazorBFF: a Blazor WASM app, secured via the BFF (backend-for-frontend) pattern.  Tokens are never exposed to the browser, the host (BFF) handles the full OIDC flow + token management.  Requests from the Blazor WASM app to the BFF (including "local" APIs) are secured via the host cookie.  Downstream API access is proxied via the BFF and secured via an access token.
