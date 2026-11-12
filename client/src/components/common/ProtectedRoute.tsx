import { Navigate, Outlet } from "react-router";
import { isAuthenticated, login } from "../../services/authService";
import { useEffect } from "react";

type ProtectedRouteProps = {
  checkFn?: () => boolean;
  checkAuthorized?: boolean;
};

export default function ProtectedRoute({ checkFn, checkAuthorized = true }: ProtectedRouteProps) {
  const requireLogin = checkAuthorized && !isAuthenticated();

  useEffect(() => {
    if (requireLogin) {
      login();
    }
  }, [requireLogin]);

  if (requireLogin) {
    return null;
  }

  if (checkFn && !checkFn()) {
    return <Navigate to="/forbidden" replace />
  }

  return <Outlet />;
}