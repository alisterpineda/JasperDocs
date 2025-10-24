import { createContext } from 'react';
import type { AccessTokenResponse } from '../api/generated/api.schemas';

export interface AuthContextType {
  isAuthenticated: boolean;
  user: { email?: string } | null;
  login: (tokenResponse: AccessTokenResponse, email?: string) => void;
  logout: () => Promise<void>;
  refreshTokens: (tokenResponse: AccessTokenResponse) => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);
