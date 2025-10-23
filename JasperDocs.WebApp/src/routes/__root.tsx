import { createRootRoute, Outlet, useMatchRoute } from '@tanstack/react-router'
import { TanStackRouterDevtools } from '@tanstack/router-devtools'
import { AppShell } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { Navbar } from '../components/Layout/Navbar'
import { Sidebar } from '../components/Layout/Sidebar'

export const Route = createRootRoute({
  component: RootComponent,
})

function RootComponent() {
  const [opened, { toggle }] = useDisclosure()
  const matchRoute = useMatchRoute()

  // Don't show layout on login page
  const isLoginPage = matchRoute({ to: '/login' })

  if (isLoginPage) {
    return (
      <>
        <Outlet />
        {import.meta.env.DEV && <TanStackRouterDevtools />}
      </>
    )
  }

  return (
    <>
      <AppShell
        header={{ height: 60 }}
        navbar={{
          width: 250,
          breakpoint: 'sm',
          collapsed: { mobile: !opened },
        }}
        padding="md"
      >
        <AppShell.Header>
          <Navbar opened={opened} toggle={toggle} />
        </AppShell.Header>

        <AppShell.Navbar>
          <Sidebar />
        </AppShell.Navbar>

        <AppShell.Main>
          <Outlet />
        </AppShell.Main>
      </AppShell>
      {import.meta.env.DEV && <TanStackRouterDevtools />}
    </>
  )
}
