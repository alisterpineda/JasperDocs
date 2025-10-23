import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';

interface AuthContextType {
  isAuthenticated: boolean;
  user: { email?: string } | null;
  login: (token: string, email?: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<{ email?: string } | null>(null);

  useEffect(() => {
    // Check for existing auth token on mount
    const token = localStorage.getItem('authToken');
    const email = localStorage.getItem('userEmail');
    if (token) {
      setIsAuthenticated(true);
      setUser({ email: email || undefined });
    }
  }, []);

  const login = (token: string, email?: string) => {
    localStorage.setItem('authToken', token);
    if (email) {
      localStorage.setItem('userEmail', email);
    }
    setIsAuthenticated(true);
    setUser({ email });
  };

  const logout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
    setIsAuthenticated(false);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
