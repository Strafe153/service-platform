import { Box, Button, TextField } from "@mui/material";
import CenteredBox from "../../components/common/CenteredBox";
import type { UpdateUserFn, UserReadDto, UserUpdateDto } from "../../types/user.types";
import Row from "../../components/common/Row";
import { useTranslation } from "react-i18next";
import { useState, type ChangeEvent } from "react";
import { validateUserUpdateModel } from "../../utils/user.utils";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useParams } from "react-router";
import type { Guid, ParameterlessAction } from "../../types/common.types";
import { isGuid } from "../../utils/common.utils";
import ErrorMessage from "../../components/common/ErrorMessage";
import { getSubId } from "../../services/authService";
import useValidation from "../../hooks/useValidation";
import useAlertStore from "../../store/alertStore";

type EditUserProps = {
  user: UserReadDto | undefined;
  checkId: boolean;
  updateUserFn: UpdateUserFn;
  setViewMode: ParameterlessAction;
}

export default function EditUser({
  user,
  checkId,
  updateUserFn,
  setViewMode
}: EditUserProps) {
  const [updateDto, setUpdateDto] = useState<UserUpdateDto>({
    firstName: user?.firstName ?? "",
    lastName: user?.lastName ?? "",
    phoneNumber: user?.phoneNumber?? "",
    birthDate: user?.birthDate.toString() ?? "",
  });
  const { validate, errors, setErrors } = useValidation(() => validateUserUpdateModel(updateDto));
  const setAlertError = useAlertStore((state) => state.setError);

  const params = useParams();
  const queryClient = useQueryClient();
  const { t } = useTranslation();

  const { mutate, isPending } = useMutation({
    mutationFn: () => updateUserFn(params.id as Guid, updateDto),
    onSuccess: () => {
      const subKey = params.id || getSubId();
      queryClient.invalidateQueries({ queryKey: ["users", subKey] });

      setViewMode();
    },
    onError: (e) => setAlertError(e.message),
  });

  if (checkId && !isGuid(params.id)) {
    return <ErrorMessage value="user.errors.incorrectId" />;
  }

  function updateUser(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const { name, value } = event.target;

    setErrors((prev) => ({
      ...prev,
      [name]: undefined,
    }));

    setUpdateDto((prev) => ({
      ...prev,
      [name]: value,
    }));
  }

  function update() {
    if (validate()) {
      mutate();
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
              label={t("user.update.firstName")}
              value={updateDto.firstName}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.firstName}
              helperText={errors.firstName ? t(errors.firstName) : undefined}
            />
            <TextField
              name="lastName"
              label={t("user.update.lastName")}
              value={updateDto.lastName}
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
              name="phoneNumber"
              label={t("user.update.phoneNumber")}
              value={updateDto.phoneNumber}
              variant="outlined"
              fullWidth
              required
              onChange={updateUser}
              error={!!errors.phoneNumber}
              helperText={errors.phoneNumber ? t(errors.phoneNumber) : undefined}
            />
            <TextField
              name="birthDate"
              label={t("user.update.birthDate")}
              value={updateDto.birthDate}
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
          </Row>

          <Row>
            <Button
              color="error"
              onClick={setViewMode}
              disabled={isPending}
              sx={{width: "50%"}}
            >
              {t("user.update.cancel")}
            </Button>

            <Button
              onClick={update}
              disabled={isPending}
              sx={{width: "50%"}}
            >
              {t("user.update.update")}
            </Button>
          </Row>
        </Box>
      </CenteredBox>
    </>
  );
}