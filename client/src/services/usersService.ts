import type { Guid, Page } from "../types/common.types";
import type { GetUsersDto, UserReadDto, UserRegisterDto, UserUpdateDto } from "../types/user.types";
import { fetchJson, populateQueryParameters } from "../utils/api.utils";

const baseUrl = import.meta.env.VITE_USERS_URL;
const meUrl = `${baseUrl}/me`;

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

export function updateUser(id: Guid, dto: UserUpdateDto): Promise<void> {
  return fetchJson(`${baseUrl}/${id}`, {
    method: "PUT",
    body: JSON.stringify(dto),
  });
}

export function deleteUser(id: Guid): Promise<void> {
  return fetchJson(`${baseUrl}/${id}`, {
    method: "DELETE",
  });
}

export function getCurrentUser(): Promise<UserReadDto> {
  return fetchJson(meUrl);
}

export function updateCurrentUser(dto: UserUpdateDto): Promise<void> {
  return fetchJson(meUrl, {
    method: "PUT",
    body: JSON.stringify(dto),
  });
}

export function deleteCurrentUser(): Promise<void> {
  return fetchJson(meUrl, {
    method: "DELETE",
  });
}