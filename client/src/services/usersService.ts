import type { Guid, Page } from "../types/common.types";
import type { GetUsersDto, UserReadDto, UserRegisterDto } from "../types/user.types";
import { fetchJson, populateQueryParameters } from "../utils/api.utils";

const baseUrl = import.meta.env.VITE_USERS_URL;

export function registerUser(dto: UserRegisterDto): Promise<void> {
  return fetchJson(baseUrl, {
    method: "POST",
    body: JSON.stringify(dto),
  }, false);
}

export function getUsers(dto: GetUsersDto): Promise<Page<UserReadDto>> {
  const parametrizedUrl = populateQueryParameters(baseUrl, dto);
  return fetchJson(parametrizedUrl);
}

export function getUser(id: Guid): Promise<UserReadDto> {
  return fetchJson(`${baseUrl}/${id}`);
}

export function getCurrentUser(): Promise<UserReadDto> {
  return fetchJson(`${baseUrl}/me`);
}