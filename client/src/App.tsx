import { useState } from "react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Box, ThemeProvider } from "@mui/material";
import { Header, Footer, Sidebar } from "./components/layout";
import AppRoutes from "./routes/AppRoutes";
import theme from "./styles/theme";
import "./i18n/i18n";

const queryClient = new QueryClient();

export default function App() {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  return (
    <ThemeProvider theme={theme}>
      <QueryClientProvider client={queryClient}>
        <Box
          sx={{
            display: "flex",
            flexDirection: "row",
            minHeight: "100vh",
          }}
        >
          <Sidebar
            isOpen={isSidebarOpen}
            onClose={() => setIsSidebarOpen(false)}
          />
          <Box
            sx= {{
              display: "flex",
              flexDirection: "column",
              flexGrow: 1,
            }}
          >
            <Header openSidebar={() => setIsSidebarOpen(true)} />
            <Box component="main" sx={{ flexGrow: 1 }}>
              <AppRoutes />
            </Box>
            <Footer />
          </Box>
        </Box>
      </QueryClientProvider>
    </ThemeProvider>
  );
}
