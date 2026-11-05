import Keycloak from "keycloak-js";

const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: import.meta.env.VITE_KEYCLOAK_REALM,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
});

export async function initKeycloak(): Promise<void> {
  await keycloak.init({
    onLoad: 'login-required',
  });
}

export async function login(): Promise<void> {
  await keycloak.login({
    redirectUri: import.meta.env.BASE_URL,
  });
}

export async function logOut(): Promise<void> {
  return keycloak.logout();
}

export async function getToken(): Promise<string | undefined> {
  await keycloak.updateToken(import.meta.env.VITE_KEYCLOAK_MIN_TOKEN_VALIDITY);
  return keycloak.token;
}

export function isAuthenticated(): boolean {
  return keycloak.authenticated;
}