import Keycloak from "keycloak-js";

const userResource = "user";

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

export function login(): Promise<void> {
  return keycloak.login({
    redirectUri: import.meta.env.VITE_BASE_URL,
  });
}

export function logOut(): Promise<void> {
  return keycloak.logout();
}

export async function getToken(): Promise<string | undefined> {
  await keycloak.updateToken(import.meta.env.VITE_KEYCLOAK_MIN_TOKEN_VALIDITY);
  return keycloak.token;
}

export function isAuthenticated(): boolean {
  return keycloak.authenticated;
}

export function isAdmin(): boolean {
  return keycloak.hasResourceRole("admin", userResource);
}

export function getSubId(): string | undefined {
  return keycloak.subject;
}