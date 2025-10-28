import { createFileRoute, redirect } from '@tanstack/react-router'
import { PartyDetail } from '../../pages/PartyDetail'

export const Route = createFileRoute('/parties/$partyId')({
  beforeLoad: () => {
    // Check if user is authenticated by checking localStorage
    const token = localStorage.getItem('authToken')

    if (!token) {
      // Redirect to login if not authenticated
      throw redirect({
        to: '/login',
        search: {
          redirect: '/parties',
        },
      })
    }
  },
  component: PartyDetail,
})
