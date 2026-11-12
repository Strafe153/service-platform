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
        minHeight: "calc(100vh - 18.5vh)",
      }}
    >
      <Typography
        variant="h2"
        sx={{ color: "#f00" }}
      >
        {t(value)}
      </Typography>
    </Box>
  );
}