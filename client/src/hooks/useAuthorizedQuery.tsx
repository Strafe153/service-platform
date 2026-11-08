import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { getToken } from "../services/authService";
import { fetchJson } from "../utils/api.utils";

export default function useAuthorizedQuery<T>(
  url: RequestInfo | URL,
  keys: string[] = [],
  headers: [string, string][] | Record<string, string> | Headers = {},
): UseQueryResult<T, Error> {
  return useQuery({
    queryKey: keys,
    queryFn: async () => {
      const token = await getToken();

      await fetchJson(url, {
        headers: {
          ...headers,
          "Authorization": `Bearer ${token}`,
        },
      });
    },
  });
}