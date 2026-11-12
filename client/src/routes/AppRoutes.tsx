import { Routes, Route } from "react-router";
import Home from "../pages/Home";
import SignUp from "../pages/SignUp";
import Users from "../pages/Users";
import Forbidden from "../pages/Forbidden";
import ProtectedRoute from "../components/common/ProtectedRoute";
import { isAdmin } from "../services/authService";
import User from "../pages/User";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/sign-up" element={<SignUp />} />

      <Route element={<ProtectedRoute checkFn={isAdmin} />}>
        <Route path="/users" element={<Users />} />
        <Route path="/users/:id" element={<User />} />
      </Route>

      <Route path="/forbidden" element={<Forbidden />} />
    </Routes>
  );
}