import { createContext, useContext } from 'react';
import { type UserRole } from '../types/api';

export interface AuthUser {
  userId: string;
  email: string;
  role: UserRole;
  displayName: string;
}

export interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
}

export interface JwtPayload {
  sub: string;
  email: string;
  role: UserRole;
  displayName: string;
  exp: number;
}

export const AuthContext = createContext<AuthContextType | null>(null);

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
