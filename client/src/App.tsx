import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Box, ThemeProvider } from "@mui/material";
import Footer from "./components/layout/Footer/Footer";
import Header from "./components/layout/Header/Header";
import theme from "./styles/theme";
import AppRoutes from "./routes/AppRoutes";
import "./i18n/i18n";

const queryClient = new QueryClient();

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <QueryClientProvider client={queryClient}>
        <Box sx=
          {{
            minHeight: "100vh",
            display: "flex",
            flexDirection: "column",
          }}
        >
          <Header />
          <Box component="main" sx={{ flexGrow: 1 }}>
            <AppRoutes />
          </Box>
          <Footer />
        </Box>
      </QueryClientProvider>
    </ThemeProvider>
  );
}
