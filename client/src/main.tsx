import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router';
import { initKeycloak } from './services/authService.ts';
import App from './App.tsx';
import './index.css';
import Auth from './features/Auth/Auth.tsx';

await initKeycloak();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Auth>
        <App />
      </Auth>
    </BrowserRouter>
  </StrictMode>
);
