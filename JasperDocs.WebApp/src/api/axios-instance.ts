import axios, { AxiosError, AxiosRequestConfig } from 'axios';

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

// Response interceptor to handle auth errors
AXIOS_INSTANCE.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      // Clear auth token and redirect to login
      localStorage.removeItem('authToken');
      // You can add custom logic here, e.g., redirect to login page
      // window.location.href = '/login';
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
