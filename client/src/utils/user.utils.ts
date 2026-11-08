import { z } from "zod";
import type { UserRegisterDto } from "../types/user.types";

export function defaultUserModel(): UserRegisterDto {
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

const phoneNumberRegex = /^[+]*[(]?[0-9]{1,4}[)]?[-\s./0-9]*$/;

const userRegisterModelValidator = z.object({
  email: z
    .email("registration.form.errors.email.format")
    .max(50, "registration.form.errors.email.max"),
  firstName: z
    .string()
    .nonempty("registration.form.errors.firstName.required")
    .max(25, "registration.form.errors.firstName.max"),
  lastName: z
    .string()
    .nonempty("registration.form.errors.lastName.required")
    .max(25, "registration.form.errors.lastName.max"),
  phoneNumber: z
    .string()
    .regex(phoneNumberRegex, "registration.form.errors.phoneNumber.format")
    .min(10, "registration.form.errors.phoneNumber.min")
    .max(15, "registration.form.errors.phoneNumber.max"),
  birthDate: z.iso.date("registration.form.errors.birthDate.format"),
  password: z
    .string()
    .min(8, "registration.form.errors.password.min")
    .max(64, "registration.form.errors.password.max"),
  address: z.object({
    country: z
      .string()
      .min(2, "registration.form.errors.country.min")
      .max(60, "registration.form.errors.country.max"),
    state: z
      .string()
      .nonempty("registration.form.errors.state.required")
      .max(100, "registration.form.errors.state.max"),
    city: z
      .string()
      .nonempty("registration.form.errors.city.required")
      .max(100, "registration.form.errors.city.max"),
    zipCode: z
      .string()
      .nonempty("registration.form.errors.zipCode.required")
      .max(10, "registration.form.errors.zipCode.max"),
    street: z
      .string()
      .max(100, "registration.form.errors.street.max")
      .optional(),
  }),
}) satisfies z.ZodType<UserRegisterDto>;
