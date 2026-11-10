import { useState, type ChangeEvent } from "react";
import { useTranslation } from "react-i18next";
import { Box, Button, TextField } from "@mui/material";
import { useMutation } from "@tanstack/react-query";
import { registerUser } from "../services/usersService";
import { defaultUserModel, validateuserRegisterModel } from "../utils/user.utils";
import { login } from "../services/authService";
import Row from "../components/common/Row";
import Alert from "../components/common/Alert";
import type { AddressDto, UserRegisterDto } from "../types/user.types";

type UserRegisterForm = keyof (Omit<UserRegisterDto, "address"> & AddressDto);

export default function SignUp() {
  const [user, setUser] = useState<UserRegisterDto>(defaultUserModel());
  const [validationErrors, setValidationErrors] = useState<Partial<Record<UserRegisterForm, string | undefined>>>({});
  const [snackbarError, setSnackbarError] = useState<string | undefined>(undefined);
  const { t } = useTranslation();

  const mutation = useMutation({
    mutationFn: registerUser,
    onSuccess: login,
    onError: (e) => setSnackbarError(e.message),
  });

  function update(name: string, updateFn: (prev: UserRegisterDto) => UserRegisterDto) {
    setValidationErrors((prev) => ({
      ...prev,
      [name]: undefined,
    }));

    setUser(updateFn);
  }

  function updateUser(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const { name, value } = event.target;

    update(name, (prev) => ({
      ...prev,
      [name]: value,
    }));
  }

  function updateAddress(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const { name, value } = event.target;

    update(name, (prev) => ({
      ...prev,
      address: {
        ...prev.address,
        [name]: value,
      }
    }));
  }

  function signUp() {
    const { success, error } = validateuserRegisterModel(user);

    if (!success) {
      const errors: Partial<Record<UserRegisterForm, string>> = {};

      for (const issue of error.issues) {
        const field = issue.path[issue.path.length - 1] as UserRegisterForm;
        errors[field] = issue.message;
      }

      setValidationErrors(errors);
      return;
    }

    setValidationErrors({});
    mutation.mutate(user);
  }

  function closeErrorSnackbar() {
    setSnackbarError(undefined);
  }

  return (
    <>
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          minHeight: "calc(100vh - 18.5vh)",
          my: 3,
        }}
      >
        <Box
          component="form"
          sx={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            gap: 2,
            width: "100%",
            maxWidth: 600,
            mx: "auto",
          }}
        >
          <Row>
            <TextField
              name="firstName"
              label={t("registration.form.firstName")}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!validationErrors.firstName}
              helperText={validationErrors.firstName ? t(validationErrors.firstName) : undefined}
            />
            <TextField
              name="lastName"
              label={t("registration.form.lastName")}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!validationErrors.lastName}
              helperText={validationErrors.lastName ? t(validationErrors.lastName) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="email"
              label={t("registration.form.email")}
              variant="outlined"
              type="email"
              fullWidth
              required
              onChange={updateUser}
              error={!!validationErrors.email}
              helperText={validationErrors.email ? t(validationErrors.email) : undefined}
            />
            <TextField
              name="phoneNumber"
              label={t("registration.form.phoneNumber")}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!validationErrors.phoneNumber}
              helperText={validationErrors.phoneNumber ? t(validationErrors.phoneNumber) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="birthDate"
              label={t("registration.form.birthDate")}
              variant="outlined"
              type="date"
              fullWidth
              required
              slotProps={{
                inputLabel: {
                  shrink: true,
                },
              }}
              onChange={updateUser}
              error={!!validationErrors.birthDate}
              helperText={validationErrors.birthDate ? t(validationErrors.birthDate) : undefined}
            />
            <TextField
              name="password"
              label={t("registration.form.password")}
              variant="outlined"
              type="password"
              fullWidth
              required
              onChange={updateUser}
              error={!!validationErrors.password}
              helperText={validationErrors.password ? t(validationErrors.password) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="country"
              label={t("registration.form.country")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!validationErrors.country}
              helperText={validationErrors.country ? t(validationErrors.country) : undefined}
            />
            <TextField
              name="state"
              label={t("registration.form.state")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!validationErrors.state}
              helperText={validationErrors.state ? t(validationErrors.state) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="city"
              label={t("registration.form.city")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!validationErrors.city}
              helperText={validationErrors.city ? t(validationErrors.city) : undefined}
            />
            <TextField
              name="zipCode"
              label={t("registration.form.zipCode")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!validationErrors.zipCode}
              helperText={validationErrors.zipCode ? t(validationErrors.zipCode) : undefined}
            />
            <TextField
              name="street"
              label={t("registration.form.street")}
              variant="outlined"
              fullWidth
              onChange={updateAddress}
              error={!!validationErrors.street}
              helperText={validationErrors.street ? t(validationErrors.street) : undefined}
            />
          </Row>

          <Button onClick={signUp} disabled={mutation.isPending}>{t("registration.form.signUp")}</Button>
        </Box>
      </Box>

      <Alert
        open={!!snackbarError}
        message={snackbarError}
        onClose={closeErrorSnackbar}
      />
    </>
  );
}