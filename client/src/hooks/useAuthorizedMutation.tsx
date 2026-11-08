// import { useMutation, type MutationFunctionContext, type UseMutationResult } from "@tanstack/react-query";
// import { getToken } from "../services/authService";

// export default function useAuthorizedMutation<T>(
//   url: string,
//   headers: Record<string, string> = {},
//   onSuccess?: ((data: void, variables: void, onMutateResult: unknown, context: MutationFunctionContext) => Promise<unknown> | unknown) | undefined
// ): UseMutationResult<T, Error, void, unknown> {
//   return useMutation({
//     mutationFn: async (newItem) => {
//       const token = await getToken();

//       await fetch(url, {
//         headers: {
//           ...headers,
//           "Authorization": `Bearer ${token}`,
//         },
//       });
//     },
//     onSuccess: onSuccess
//   });
// }