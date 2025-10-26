import { createFileRoute, redirect } from '@tanstack/react-router'
import { DocumentDetail } from '../../pages/DocumentDetail'

export const Route = createFileRoute('/documents/$documentId')({
  beforeLoad: () => {
    // Check if user is authenticated by checking localStorage
    const token = localStorage.getItem('authToken')

    if (!token) {
      // Redirect to login if not authenticated
      throw redirect({
        to: '/login',
        search: {
          redirect: '/documents',
        },
      })
    }
  },
  component: DocumentDetail,
})
