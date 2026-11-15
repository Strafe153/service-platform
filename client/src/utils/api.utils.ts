import { getToken } from "../services/authService";

type ErrorResponse = {
  errors: {
    error: string;
    property: string
  }[];
};

export async function fetchJson<T>(
  url: RequestInfo | URL,
  init?: RequestInit,
  useAuth: boolean = true,
): Promise<T> {
  const headers = new Headers(init?.headers);

  headers.append("Content-Type", "application/json");

  if (useAuth) {
    const token = await getToken();
    headers.append("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(url, {
    ...init,
    headers,
  });

  if (response.status >= 400) {
    const errorResponse: ErrorResponse = await response.json();
    const errorMessage = errorResponse.errors.map((e) => `${e.property}: ${e.error}`).join("\n");

    throw new Error(errorMessage);
  }

  const responseStr = await response.text();

  if (!responseStr) {
    return undefined as T;
  }

  return JSON.parse(responseStr);
}

export function populateQueryParameters<T extends object>(url: string, dto: T): URL {
  // Chose to have tiny memory overhead of copying the passed URL
  // over requiring callers to clone it when they use global baseUrl
  const newUrl = new URL(url);

  for (const [key, value] of Object.entries(dto)) {
    // != is used deliberately to filter both null and undefined
    if (value != null) {
      newUrl.searchParams.append(key, String(value));
    }
  }

  return newUrl;
}