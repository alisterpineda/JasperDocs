import { useNavigate } from '@tanstack/react-router';
import { Group, Button, Menu, Avatar, Text, UnstyledButton, Burger, ActionIcon, Tooltip } from '@mantine/core';
import { IconLogout, IconUser, IconSun, IconMoon, IconSunMoon } from '@tabler/icons-react';
import { useMantineColorScheme } from '@mantine/core';
import { useAuth } from '../../hooks/useAuth';
import classes from './Navbar.module.css';

interface NavbarProps {
  opened: boolean;
  toggle: () => void;
}

export function Navbar({ opened, toggle }: NavbarProps) {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();
  const { colorScheme, setColorScheme } = useMantineColorScheme();

  const handleLogout = async () => {
    await logout();
    navigate({ to: '/login' });
  };

  const cycleColorScheme = () => {
    if (colorScheme === 'light') {
      setColorScheme('dark');
    } else if (colorScheme === 'dark') {
      setColorScheme('auto');
    } else {
      setColorScheme('light');
    }
  };

  const getThemeIcon = () => {
    if (colorScheme === 'light') {
      return <IconSun size={20} stroke={1.5} />;
    } else if (colorScheme === 'dark') {
      return <IconMoon size={20} stroke={1.5} />;
    } else {
      return <IconSunMoon size={20} stroke={1.5} />;
    }
  };

  const getThemeLabel = () => {
    if (colorScheme === 'light') {
      return 'Switch to dark mode';
    } else if (colorScheme === 'dark') {
      return 'Switch to auto mode';
    } else {
      return 'Switch to light mode';
    }
  };

  return (
    <header className={classes.header}>
      <Group h="100%" px="md" justify="space-between">
        <Group>
          <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
          <Text size="xl" fw={700} className={classes.logo}>
            JasperDocs
          </Text>
        </Group>

        <Group gap="sm">
          <Tooltip label={getThemeLabel()}>
            <ActionIcon
              variant="subtle"
              size="lg"
              onClick={cycleColorScheme}
              aria-label="Cycle color scheme"
            >
              {getThemeIcon()}
            </ActionIcon>
          </Tooltip>

          {isAuthenticated ? (
          <Menu shadow="md" width={200}>
            <Menu.Target>
              <UnstyledButton className={classes.userButton}>
                <Group gap="xs">
                  <Avatar size={32} radius="xl" color="blue">
                    {user?.email?.charAt(0).toUpperCase() || 'U'}
                  </Avatar>
                  <Text size="sm" fw={500} visibleFrom="sm">
                    {user?.email || 'User'}
                  </Text>
                </Group>
              </UnstyledButton>
            </Menu.Target>

            <Menu.Dropdown>
              <Menu.Label>Account</Menu.Label>
              <Menu.Item leftSection={<IconUser size={16} />}>
                Profile
              </Menu.Item>
              <Menu.Divider />
              <Menu.Item
                leftSection={<IconLogout size={16} />}
                onClick={handleLogout}
                color="red"
              >
                Logout
              </Menu.Item>
            </Menu.Dropdown>
          </Menu>
          ) : (
            <Button onClick={() => navigate({ to: '/login' })}>
              Sign In
            </Button>
          )}
        </Group>
      </Group>
    </header>
  );
}
