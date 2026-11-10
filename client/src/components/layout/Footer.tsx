import { AppBar, Box, Toolbar, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";

export default function Footer() {
  const { t } = useTranslation();

  const currentYear = new Date().getFullYear();

  return (
    <Box component="footer">
      <AppBar position="static">
        <Toolbar sx={{ justifyContent: "center" }}>
          <Typography align="center">{t("footer.copyrightName")} &#169; {currentYear}</Typography>
        </Toolbar>
      </AppBar>
    </Box>
  );
}