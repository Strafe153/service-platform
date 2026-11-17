import { useState, type ChangeEvent } from "react";
import { useTranslation } from "react-i18next";
import { Box, Button, TextField } from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { registerUser } from "../services/usersService";
import { defaultUserRegisterModel, validateuserRegisterModel } from "../utils/user.utils";
import { login } from "../services/authService";
import Row from "../components/common/Row";
import type { AddressDto, UserRegisterDto } from "../types/user.types";
import CenteredBox from "../components/common/CenteredBox";
import useValidation from "../hooks/useValidation";
import useAlertStore from "../store/alertStore";

type UserRegisterForm = (Omit<UserRegisterDto, "address"> & AddressDto);

export default function SignUp() {
  const [user, setUser] = useState<UserRegisterDto>(defaultUserRegisterModel());
  const { validate, errors, setErrors } = useValidation<UserRegisterDto, UserRegisterForm>(
    () => validateuserRegisterModel(user));
  const setAlertError = useAlertStore((state) => state.setError);
  
  const queryClient = useQueryClient();
  const { t } = useTranslation();

  const { mutate, isPending } = useMutation({
    mutationFn: registerUser,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["users"] });
      login();
    },
    onError: (e) => setAlertError(e.message),
  });

  function update(name: string, updateFn: (prev: UserRegisterDto) => UserRegisterDto) {
    setErrors((prev) => ({
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
    if (validate()) {
      mutate(user);
    }
  }

  return (
    <>
      <CenteredBox>
        <Box
          component="form"
          sx={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            gap: 2,
            maxWidth: 600,
          }}
        >
          <Row>
            <TextField
              name="firstName"
              label={t("registration.firstName")}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.firstName}
              helperText={errors.firstName ? t(errors.firstName) : undefined}
            />
            <TextField
              name="lastName"
              label={t("registration.lastName")}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.lastName}
              helperText={errors.lastName ? t(errors.lastName) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="email"
              label={t("registration.email")}
              variant="outlined"
              type="email"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.email}
              helperText={errors.email ? t(errors.email) : undefined}
            />
            <TextField
              name="phoneNumber"
              label={t("registration.phoneNumber")}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.phoneNumber}
              helperText={errors.phoneNumber ? t(errors.phoneNumber) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="birthDate"
              label={t("registration.birthDate")}
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
              error={!!errors.birthDate}
              helperText={errors.birthDate ? t(errors.birthDate) : undefined}
            />
            <TextField
              name="password"
              label={t("registration.password")}
              variant="outlined"
              type="password"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.password}
              helperText={errors.password ? t(errors.password) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="country"
              label={t("registration.country")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!errors.country}
              helperText={errors.country ? t(errors.country) : undefined}
            />
            <TextField
              name="state"
              label={t("registration.state")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!errors.state}
              helperText={errors.state ? t(errors.state) : undefined}
            />
          </Row>

          <Row>
            <TextField
              name="city"
              label={t("registration.city")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!errors.city}
              helperText={errors.city ? t(errors.city) : undefined}
            />
            <TextField
              name="zipCode"
              label={t("registration.zipCode")}
              variant="outlined"
              fullWidth
              required
              onChange={updateAddress}
              error={!!errors.zipCode}
              helperText={errors.zipCode ? t(errors.zipCode) : undefined}
            />
            <TextField
              name="street"
              label={t("registration.street")}
              variant="outlined"
              fullWidth
              onChange={updateAddress}
              error={!!errors.street}
              helperText={errors.street ? t(errors.street) : undefined}
            />
          </Row>

          <Button
            onClick={signUp}
            disabled={isPending}
          >
            {t("registration.signUp")}
          </Button>
        </Box>
      </CenteredBox>
    </>
  );
}