import { useTranslation } from "react-i18next";
import { Box, Typography } from "@mui/material";

type ErrorMessageProps = {
  value: string;
};

export default function ErrorMessage({ value }: ErrorMessageProps) {
  const { t } = useTranslation();

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        flexGrow: 1,
      }}
    >
      <Typography
        variant="h5"
        color="error"
      >
        {t(value)}
      </Typography>
    </Box>
  );
}