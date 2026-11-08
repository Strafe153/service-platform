import Keycloak from "keycloak-js";

const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: import.meta.env.VITE_KEYCLOAK_REALM,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
});

export async function initKeycloak(): Promise<void> {
  await keycloak.init({
    onLoad: "check-sso",
    silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`,
    silentCheckSsoFallback: false,
  });
}

export async function login(): Promise<void> {
  await keycloak.login({
    redirectUri: import.meta.env.VITE_BASE_URL,
  });
}

export async function logOut(): Promise<void> {
  return keycloak.logout();
}

export async function getToken(): Promise<string | undefined> {
  try {
    const isRefreshed = await keycloak.updateToken(import.meta.env.VITE_KEYCLOAK_MIN_TOKEN_VALIDITY);

    if (!isRefreshed) {
      console.error("The token was not refreshed");
    }
  } catch {
    console.error("Failed to refresh the token");
  }

  return keycloak.token;
}

export function isAuthenticated(): boolean {
  return keycloak.authenticated;
}