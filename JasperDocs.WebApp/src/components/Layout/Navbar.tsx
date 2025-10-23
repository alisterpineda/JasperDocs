import { useNavigate } from '@tanstack/react-router';
import { Group, Button, Menu, Avatar, Text, UnstyledButton, Burger } from '@mantine/core';
import { IconLogout, IconUser } from '@tabler/icons-react';
import { useAuth } from '../../contexts/AuthContext';
import classes from './Navbar.module.css';

interface NavbarProps {
  opened: boolean;
  toggle: () => void;
}

export function Navbar({ opened, toggle }: NavbarProps) {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate({ to: '/login' });
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
    </header>
  );
}
