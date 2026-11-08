type ErrorResponse = {
  errors: {
    error: string;
    property: string
  }[];
};

export async function fetchJson(url: RequestInfo | URL, init?: RequestInit): Promise<Response> {
  const response = await fetch(url, {
    ...init,
    headers: {
      ...init?.headers,
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    const errorResponse: ErrorResponse = await response.json();
    const errorMessage = errorResponse.errors.map((e) => `${e.property}: ${e.error}`).join("\n");

    throw new Error(errorMessage);
  }

  return response.json();
}
