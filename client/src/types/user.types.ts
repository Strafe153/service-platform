import type { Guid } from "./common.types";

export interface GetUsersDto {
  pageNumber: number;
  pageSize: number;
}

export interface UserRegisterDto {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  birthDate: string;
  password: string;
  address: AddressDto;
}

export interface UserReadDto {
  id: Guid;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  birthDate: Date;
  address: AddressDto;
}

export interface AddressDto {
  country: string;
  state: string;
  city: string;
  zipCode: string;
  street?: string;
}