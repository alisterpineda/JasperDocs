import { Title, Text, Container, Paper, Stack } from '@mantine/core';
import { useAuth } from '../contexts/AuthContext';

export function Home() {
  const { user, isAuthenticated } = useAuth();

  return (
    <Container size="lg" py="xl">
      <Stack gap="lg">
        <Title order={1}>Welcome to JasperDocs</Title>
        {isAuthenticated ? (
          <Paper shadow="sm" p="md">
            <Text size="lg">Hello, {user?.email || 'User'}!</Text>
            <Text c="dimmed" mt="sm">
              Get started by navigating to Documents in the sidebar.
            </Text>
          </Paper>
        ) : (
          <Paper shadow="sm" p="md">
            <Text size="lg">Please log in to access your documents.</Text>
          </Paper>
        )}
      </Stack>
    </Container>
  );
}
