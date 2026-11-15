import { useState } from "react";
import { z } from "zod";

type ValidationFn<T> = () => z.ZodSafeParseResult<T>;

export default function useValidation<T, T2 = T>(validationFn: ValidationFn<T>) {
  const [errors, setErrors] = useState<Partial<Record<keyof T2, string | undefined>>>({});

  function validate(): boolean {
    const { success, error } = validationFn();

    setErrors({});

    if (!success) {
      const errors: Partial<Record<keyof T2, string>> = {};

      for (const issue of error.issues) {
        const field = issue.path[issue.path.length - 1] as keyof T2;
        errors[field] = issue.message;
      }

      setErrors(errors);
    }

    return success;
  }

  return {
    validate,
    errors,
    setErrors,
  };
}