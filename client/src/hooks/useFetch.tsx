import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { getToken } from "../services/authService";

export default function useAuthorizedQuery<T>(
  url: string,
  keys: string[] = [],
  headers: Record<string, string> = {},
): UseQueryResult<T, Error> {
  return useQuery({
    queryKey: keys,
    queryFn: async () => {
      const token = await getToken();

      await fetch(url, {
        headers: {
          ...headers,
          'Authorization': `Bearer ${token}`,
        },
      });
    },
  });
}