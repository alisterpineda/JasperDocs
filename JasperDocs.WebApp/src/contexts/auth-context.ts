import { createContext } from 'react';
import type { AccessTokenResponse } from '../api/generated/api.schemas';

export interface AuthContextType {
  isAuthenticated: boolean;
  user: { username?: string } | null;
  login: (tokenResponse: AccessTokenResponse, username?: string) => void;
  logout: () => Promise<void>;
  refreshTokens: (tokenResponse: AccessTokenResponse) => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);
