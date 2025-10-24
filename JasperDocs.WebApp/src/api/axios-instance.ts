import axios, { AxiosError } from 'axios';
import type { AxiosRequestConfig } from 'axios';
import { tokenRefreshService } from '../services/tokenRefresh';

const AXIOS_INSTANCE = axios.create({
  baseURL: '/',
});

// Request interceptor to add auth token
AXIOS_INSTANCE.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle auth errors and token refresh
AXIOS_INSTANCE.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

    // Check if error is 401 and we haven't already tried to refresh
    if (error.response?.status === 401 && !originalRequest._retry) {
      // Don't retry for login, refresh, or logout endpoints
      const isAuthEndpoint = originalRequest.url?.includes('/api/login') ||
                            originalRequest.url?.includes('/api/refresh') ||
                            originalRequest.url?.includes('/api/logout');

      if (isAuthEndpoint) {
        return Promise.reject(error);
      }

      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          // No refresh token available, clear auth and reject
          localStorage.removeItem('authToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('userEmail');
          window.location.href = '/login';
          return Promise.reject(error);
        }

        // Attempt to refresh the token
        const response = await tokenRefreshService.refresh(refreshToken);

        // Update the original request with the new token
        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${response.accessToken}`;
        }

        // Retry the original request
        return AXIOS_INSTANCE(originalRequest);
      } catch (refreshError) {
        // Refresh failed, clear auth and redirect to login
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userEmail');
        tokenRefreshService.clear();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export const customAxiosInstance = <T>(
  config: AxiosRequestConfig
): Promise<T> => {
  const source = axios.CancelToken.source();
  const promise = AXIOS_INSTANCE({
    ...config,
    cancelToken: source.token,
  }).then(({ data }) => data);

  // @ts-expect-error - Adding cancel method for orval
  promise.cancel = () => {
    source.cancel('Query was cancelled');
  };

  return promise;
};
