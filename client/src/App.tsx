import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ThemeProvider } from "@mui/material";
import Footer from "./components/layout/Footer/Footer";
import Header from "./components/layout/Header/Header";
import theme from "./styles/theme";
import AppRoutes from "./routes/AppRoutes";

const queryClient = new QueryClient();

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <QueryClientProvider client={queryClient}>
        <Header />
        <main>
          <AppRoutes />
        </main>
        <Footer />
      </QueryClientProvider>
    </ThemeProvider>
  );
}
