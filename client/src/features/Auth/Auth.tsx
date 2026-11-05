import { useState, useEffect, type PropsWithChildren } from "react";
import { isAuthenticated, login } from "../../services/authService";

export default function Auth({ children }: PropsWithChildren) {
  const [authenticated, setAuthenticated] = useState(isAuthenticated());
  
  useEffect(() => {
    if (!authenticated) {
      login()
        .then(() => setAuthenticated(true))
        .catch(console.error);
    }
  }, [authenticated]);

  return children;
}