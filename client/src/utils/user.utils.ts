import { z } from "zod";
import type { UserRegisterDto, UserUpdateDto } from "../types/user.types";

export function defaultUserRegisterModel(): UserRegisterDto {
  return {
    email: "",
    firstName: "",
    lastName: "",
    phoneNumber: "",
    birthDate: "",
    password: "",
    address: {
      country: "",
      state: "",
      city: "",
      zipCode: "",
    },
  };
}

export function validateuserRegisterModel(
  value: UserRegisterDto
): z.ZodSafeParseResult<UserRegisterDto> {
  return userRegisterModelValidator.safeParse(value);
}

export function validateUserUpdateModel(value: UserUpdateDto): z.ZodSafeParseResult<UserUpdateDto> {
  return userUpdateModelValidator.safeParse(value);
}

const phoneNumberRegex = /^[+]*[(]?[0-9]{1,4}[)]?[-\s./0-9]*$/;

const userRegisterModelValidator = z.object({
  email: z
    .email("registration.errors.email.format")
    .max(50, "registration.errors.email.max"),
  firstName: z
    .string()
    .nonempty("registration.errors.firstName.required")
    .max(25, "registration.errors.firstName.max"),
  lastName: z
    .string()
    .nonempty("registration.errors.lastName.required")
    .max(25, "registration.errors.lastName.max"),
  phoneNumber: z
    .string()
    .regex(phoneNumberRegex, "registration.errors.phoneNumber.format")
    .min(10, "registration.errors.phoneNumber.min")
    .max(15, "registration.errors.phoneNumber.max"),
  birthDate: z.iso.date("registration.errors.birthDate.format"),
  password: z
    .string()
    .min(8, "registration.errors.password.min")
    .max(64, "registration.errors.password.max"),
  address: z.object({
    country: z
      .string()
      .min(2, "registration.errors.country.min")
      .max(60, "registration.errors.country.max"),
    state: z
      .string()
      .nonempty("registration.errors.state.required")
      .max(100, "registration.errors.state.max"),
    city: z
      .string()
      .nonempty("registration.errors.city.required")
      .max(100, "registration.errors.city.max"),
    zipCode: z
      .string()
      .nonempty("registration.errors.zipCode.required")
      .max(10, "registration.errors.zipCode.max"),
    street: z
      .string()
      .max(100, "registration.errors.street.max")
      .optional(),
  }),
}) satisfies z.ZodType<UserRegisterDto>;

const userUpdateModelValidator = z.object({
  firstName: z
    .string()
    .nonempty("registration.errors.firstName.required")
    .max(25, "registration.errors.firstName.max"),
  lastName: z
    .string()
    .nonempty("registration.errors.lastName.required")
    .max(25, "registration.errors.lastName.max"),
  phoneNumber: z
    .string()
    .regex(phoneNumberRegex, "registration.errors.phoneNumber.format")
    .min(10, "registration.errors.phoneNumber.min")
    .max(15, "registration.errors.phoneNumber.max"),
  birthDate: z.iso.date("registration.errors.birthDate.format"),
}) satisfies z.ZodType<UserUpdateDto>;
