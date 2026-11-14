import { Navigate, Outlet, useParams, type Params } from "react-router";
import { isAuthenticated, login } from "../../services/authService";
import { useEffect } from "react";

type ProtectedRouteProps = {
  checkFn?: (params: Readonly<Params<string>>) => boolean;
  checkAuthorized?: boolean;
};

export default function ProtectedRoute({ checkFn, checkAuthorized = true }: ProtectedRouteProps) {
  const params = useParams();

  const requireLogin = checkAuthorized && !isAuthenticated();

  useEffect(() => {
    if (requireLogin) {
      login();
    }
  }, [requireLogin]);

  if (requireLogin) {
    return null;
  }

  if (checkFn && !checkFn(params)) {
    return <Navigate to="/forbidden" replace />
  }

  return <Outlet />;
}