import { useState, useMemo, useCallback, type ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import { AuthContext, type AuthUser, type JwtPayload } from './useAuth';

export function AuthProvider({ children }: { children: ReactNode }) {
  // Validate token on mount; discard expired/invalid ones immediately
  const [token, setToken] = useState<string | null>(() => {
    const stored = localStorage.getItem('jwt_token');
    if (!stored) return null;
    try {
      const { exp } = jwtDecode<JwtPayload>(stored);
      if (exp * 1000 < Date.now()) {
        localStorage.removeItem('jwt_token');
        return null;
      }
      return stored;
    } catch {
      localStorage.removeItem('jwt_token');
      return null;
    }
  });

  // Derive user from token — no effect needed
  const user = useMemo<AuthUser | null>(() => {
    if (!token) return null;
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return {
        userId: decoded.sub,
        email: decoded.email,
        role: decoded.role,
        displayName: decoded.displayName,
      };
    } catch {
      return null;
    }
  }, [token]);

  const logout = useCallback(() => {
    localStorage.removeItem('jwt_token');
    setToken(null);
  }, []);

  const login = useCallback((newToken: string) => {
    localStorage.setItem('jwt_token', newToken);
    setToken(newToken);
  }, []);

  return (
    <AuthContext.Provider
      value={{ user, token, isAuthenticated: !!user, login, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}
