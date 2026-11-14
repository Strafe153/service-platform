import { Box, Divider, Typography } from "@mui/material";
import { useParams } from "react-router";
import { useTranslation } from "react-i18next";
import CenteredBox from "../../components/common/CenteredBox";
import ErrorMessage from "../../components/common/ErrorMessage";
import { isGuid } from "../../utils/common.utils";
import type { UserReadDto } from "../../types/user.types";

type UserViewProps = {
  user: UserReadDto | undefined;
  isError: boolean;
  error: Error | null;
  checkId: boolean;
};

export default function UserView({ user, isError, error, checkId }: UserViewProps) {
  const params = useParams();
  const { t } = useTranslation();

  if (checkId && !isGuid(params.id)) {
    return <ErrorMessage value="user.errors.incorrectId" />;
  }
  
  if (isError) {
    return <ErrorMessage value={error!.message} />;
  }

  return (
    <CenteredBox>
      <Box>
        <Typography variant="h5" sx={{ mb: 1 }}>{t("user.profile.personalInfo")}</Typography>

        <Typography>{t("user.profile.fullName")}: {user?.firstName} {user?.lastName}</Typography>
        <Typography>{t("user.profile.email")}: {user?.email}</Typography>
        <Typography>{t("user.profile.phoneNumber")}: {user?.phoneNumber}</Typography>
        <Typography>{t("user.profile.birthDate")}: {user?.birthDate.toString()}</Typography>

        <Divider sx={{ my: 2 }} />

        <Typography variant="h5" sx={{ mb: 1 }}>{t("user.profile.address")}</Typography>

        <Typography>{t("user.profile.phoneNumber")}: {user?.address.country}</Typography>
        <Typography>{t("user.profile.state")}: {user?.address.state}</Typography>
        <Typography>{t("user.profile.city")}: {user?.address.city}</Typography>
        <Typography>{t("user.profile.zipCode")}: {user?.address.zipCode}</Typography>

        {user?.address.street
          && <Typography>{t("user.profile.street")}: {user?.address.street}</Typography>}
      </Box>
    </CenteredBox>
  );
}