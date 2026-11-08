import { Routes, Route } from "react-router";
import Home from "../pages/Home";
import SignUp from "../pages/SignUp";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/sign-up" element={<SignUp />} />
    </Routes>
  );
}