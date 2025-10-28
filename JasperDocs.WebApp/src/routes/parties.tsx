import { createFileRoute, redirect, Outlet } from '@tanstack/react-router'

export const Route = createFileRoute('/parties')({
  beforeLoad: () => {
    // Check if user is authenticated by checking localStorage
    const token = localStorage.getItem('authToken')

    if (!token) {
      // Redirect to login if not authenticated
      throw redirect({
        to: '/login',
        search: {
          // Optionally add redirect parameter to return to parties after login
          redirect: '/parties',
        },
      })
    }
  },
  component: () => <Outlet />,
})
