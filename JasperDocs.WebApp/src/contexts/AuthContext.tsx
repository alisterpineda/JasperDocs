import { useState, useEffect, useCallback } from 'react';
import type { ReactNode } from 'react';
import type { AccessTokenResponse } from '../api/generated/api.schemas';
import { postApiLogout } from '../api/generated/authentication/authentication';
import { tokenRefreshService } from '../services/tokenRefresh';
import { AuthContext } from './auth-context';

const AUTH_STORAGE_KEY = 'auth:state';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<{ username?: string } | null>(null);

  // Function to update auth state and notify other tabs
  const updateAuthState = useCallback((authenticated: boolean, username?: string) => {
    setIsAuthenticated(authenticated);
    setUser(authenticated ? { username } : null);

    // Broadcast state change to other tabs via localStorage event
    if (authenticated) {
      localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify({ authenticated: true, username }));
    } else {
      localStorage.removeItem(AUTH_STORAGE_KEY);
    }
  }, []);

  // Handle token refresh and schedule next refresh
  const refreshTokens = useCallback((tokenResponse: AccessTokenResponse) => {
    localStorage.setItem('authToken', tokenResponse.accessToken);
    localStorage.setItem('refreshToken', tokenResponse.refreshToken);

    // Schedule proactive refresh 5 minutes before expiry
    tokenRefreshService.scheduleRefresh(tokenResponse.expiresIn, (newTokenResponse) => {
      refreshTokens(newTokenResponse);
    });
  }, []);

  // Login function
  const login = useCallback((tokenResponse: AccessTokenResponse, username?: string) => {
    localStorage.setItem('authToken', tokenResponse.accessToken);
    localStorage.setItem('refreshToken', tokenResponse.refreshToken);
    if (username) {
      localStorage.setItem('username', username);
    }

    updateAuthState(true, username);

    // Schedule proactive refresh 5 minutes before expiry
    tokenRefreshService.scheduleRefresh(tokenResponse.expiresIn, (newTokenResponse) => {
      refreshTokens(newTokenResponse);
    });
  }, [updateAuthState, refreshTokens]);

  // Logout function
  const logout = useCallback(async () => {
    try {
      // Call backend logout endpoint
      await postApiLogout({});
    } catch (error) {
      console.error('Logout API call failed:', error);
      // Continue with local logout even if API call fails
    }

    // Clear all auth data
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('username');

    // Clear token refresh service state
    tokenRefreshService.clear();

    // Update state and notify other tabs
    updateAuthState(false);
  }, [updateAuthState]);

  // Initialize auth state on mount
  useEffect(() => {
    const token = localStorage.getItem('authToken');
    const refreshToken = localStorage.getItem('refreshToken');
    const username = localStorage.getItem('username');

    if (token && refreshToken) {
      setIsAuthenticated(true);
      setUser({ username: username || undefined });

      // Note: We can't schedule refresh here without knowing the expiry time
      // The refresh will be handled by the axios interceptor if needed
    }
  }, []);

  // Listen for auth changes from other tabs
  useEffect(() => {
    const handleStorageChange = (e: StorageEvent) => {
      // Handle auth state changes
      if (e.key === AUTH_STORAGE_KEY) {
        if (e.newValue) {
          const { authenticated, username } = JSON.parse(e.newValue);
          setIsAuthenticated(authenticated);
          setUser(authenticated ? { username } : null);
        } else {
          // Logout in another tab
          setIsAuthenticated(false);
          setUser(null);
          tokenRefreshService.clear();
        }
      }

      // Handle token updates (from refresh in another tab)
      if (e.key === 'authToken' && e.newValue) {
        // Token was updated in another tab, just update our state
        setIsAuthenticated(true);
      }

      // Handle logout
      if (e.key === 'authToken' && !e.newValue) {
        // Token was removed (logout in another tab)
        setIsAuthenticated(false);
        setUser(null);
        tokenRefreshService.clear();
      }
    };

    window.addEventListener('storage', handleStorageChange);

    return () => {
      window.removeEventListener('storage', handleStorageChange);
      tokenRefreshService.clear();
    };
  }, []);

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout, refreshTokens }}>
      {children}
    </AuthContext.Provider>
  );
}

