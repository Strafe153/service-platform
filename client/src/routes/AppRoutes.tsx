import { Routes, Route } from "react-router";
import Home from "../pages/Home";
import SignUp from "../pages/SignUp";
import Users from "../pages/Users/Users";
import Forbidden from "../pages/Forbidden";
import ProtectedRoute from "../components/common/ProtectedRoute";
import { isAdmin } from "../services/authService";
import UserProfile from "../pages/Users/UserProfile";
import CurrentUserProfile from "../pages/Users/CurrentUserProfile";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/sign-up" element={<SignUp />} />

      <Route element={<ProtectedRoute checkFn={isAdmin} />}>
        <Route path="/users" element={<Users />} />
        <Route path="/users/:id" element={<UserProfile />} />
      </Route>

      <Route element={<ProtectedRoute />}>
        <Route path="/me" element={<CurrentUserProfile />} />
      </Route>

      <Route path="/forbidden" element={<Forbidden />} />
    </Routes>
  );
}