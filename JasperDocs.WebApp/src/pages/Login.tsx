import { useState } from 'react';
import { useNavigate } from '@tanstack/react-router';
import {
  TextInput,
  PasswordInput,
  Button,
  Paper,
  Title,
  Container,
  Stack,
  Text,
} from '@mantine/core';
import { useAuth } from '../hooks/useAuth';
import { usePostApiLogin } from '../api/generated/authentication/authentication';

export function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();
  const { mutateAsync: loginMutation, isPending } = usePostApiLogin();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      const response = await loginMutation({
        data: {
          email,
          password
        }
      });

      // Pass the entire token response to login for proper token management
      login(response, email);
      navigate({ to: '/' });
    } catch (err: unknown) {
      const errorMessage = (err as { response?: { data?: { detail?: string } } })?.response?.data?.detail || 'Login failed. Please check your credentials.';
      setError(errorMessage);
    }
  };

  return (
    <Container size={420} my={40}>
      <Title ta="center">Welcome to JasperDocs</Title>
      <Text c="dimmed" size="sm" ta="center" mt={5}>
        Sign in to access your documents
      </Text>

      <Paper withBorder shadow="md" p={30} mt={30} radius="md">
        <form onSubmit={handleSubmit}>
          <Stack gap="md">
            <TextInput
              label="Email"
              placeholder="you@example.com"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <PasswordInput
              label="Password"
              placeholder="Your password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            {error && <Text c="red" size="sm">{error}</Text>}
            <Button type="submit" fullWidth loading={isPending}>
              Sign in
            </Button>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}
