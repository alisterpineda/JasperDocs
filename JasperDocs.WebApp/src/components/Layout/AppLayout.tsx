import { AppShell } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Outlet, useLocation } from 'react-router-dom';
import { Navbar } from './Navbar';
import { Sidebar } from './Sidebar';

export function AppLayout() {
  const [opened, { toggle }] = useDisclosure();
  const location = useLocation();

  // Don't show layout on login page
  const isLoginPage = location.pathname === '/login';

  if (isLoginPage) {
    return <Outlet />;
  }

  return (
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
  );
}
