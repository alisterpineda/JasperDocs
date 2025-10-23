import { createFileRoute, redirect } from '@tanstack/react-router'
import { Documents } from '../pages/Documents'

export const Route = createFileRoute('/documents')({
  beforeLoad: () => {
    // Check if user is authenticated by checking localStorage
    const token = localStorage.getItem('authToken')

    if (!token) {
      // Redirect to login if not authenticated
      throw redirect({
        to: '/login',
        search: {
          // Optionally add redirect parameter to return to documents after login
          redirect: '/documents',
        },
      })
    }
  },
  component: Documents,
})
