import { createFileRoute } from '@tanstack/react-router'
import { Parties } from '../../pages/Parties'

export const Route = createFileRoute('/parties/')({
  component: Parties,
})
