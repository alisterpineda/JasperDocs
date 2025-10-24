import { postApiRefresh } from '../api/generated/authentication/authentication';
import type { AccessTokenResponse } from '../api/generated/api.schemas';

type PendingRequest = {
  resolve: (value: string) => void;
  reject: (error: unknown) => void;
};

class TokenRefreshService {
  private isRefreshing = false;
  private pendingRequests: PendingRequest[] = [];
  private refreshTimer: number | null = null;

  /**
   * Schedule a proactive token refresh 5 minutes before expiry
   */
  scheduleRefresh(expiresIn: number, onRefresh: (response: AccessTokenResponse) => void): void {
    // Clear existing timer
    this.cancelScheduledRefresh();

    // Schedule refresh 5 minutes (300 seconds) before token expiry
    const refreshTime = Math.max(0, (expiresIn - 300) * 1000);

    this.refreshTimer = window.setTimeout(async () => {
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const response = await this.refresh(refreshToken);
          onRefresh(response);
        }
      } catch (error) {
        console.error('Proactive token refresh failed:', error);
      }
    }, refreshTime);
  }

  /**
   * Cancel any scheduled refresh
   */
  cancelScheduledRefresh(): void {
    if (this.refreshTimer !== null) {
      window.clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  /**
   * Refresh the access token
   * Queues concurrent requests to avoid multiple refresh calls
   */
  async refresh(refreshToken: string): Promise<AccessTokenResponse> {
    if (this.isRefreshing) {
      // Wait for the ongoing refresh to complete
      return new Promise((resolve, reject) => {
        this.pendingRequests.push({ resolve, reject });
      }).then(() => {
        // After refresh completes, get the new token from storage
        const newAccessToken = localStorage.getItem('authToken');
        if (!newAccessToken) {
          throw new Error('Token refresh failed');
        }
        // Return the current token state
        return {
          accessToken: newAccessToken,
          refreshToken: localStorage.getItem('refreshToken') || '',
          tokenType: 'Bearer',
          expiresIn: 0, // Not used in this context
        };
      });
    }

    this.isRefreshing = true;

    try {
      const response = await postApiRefresh({ refreshToken });

      // Update tokens in localStorage
      localStorage.setItem('authToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);

      // Notify all pending requests that refresh is complete
      this.pendingRequests.forEach(({ resolve }) => resolve(response.accessToken));
      this.pendingRequests = [];

      return response;
    } catch (error) {
      // Notify all pending requests that refresh failed
      this.pendingRequests.forEach(({ reject }) => reject(error));
      this.pendingRequests = [];

      throw error;
    } finally {
      this.isRefreshing = false;
    }
  }

  /**
   * Clear all state (useful for logout)
   */
  clear(): void {
    this.cancelScheduledRefresh();
    this.isRefreshing = false;
    this.pendingRequests = [];
  }
}

// Export singleton instance
export const tokenRefreshService = new TokenRefreshService();
