import type { UserReadDto, UserRegisterDto as UserRegisterDto } from "../types/user.types";
import { fetchJson } from "../utils/api.utils";

export async function registerUser(dto: UserRegisterDto): Promise<UserReadDto> {
  const response = await fetchJson(`${import.meta.env.VITE_USERS_URL}`, {
    method: "POST",
    body: JSON.stringify(dto),
  });

  return response.json();
}