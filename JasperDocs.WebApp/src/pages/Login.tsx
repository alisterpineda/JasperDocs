import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
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
import { useAuth } from '../contexts/AuthContext';

export function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      // TODO: Import and use the actual postLogin function from the generated API client
      // For now this is a mock implementation until the login endpoint returns proper tokens

      // Example of how to integrate:
      // import { postLogin } from '../api/generated/authentication/authentication';
      // const response = await postLogin({ email, password });
      // login(response.accessToken, email);

      await new Promise(resolve => setTimeout(resolve, 1000));
      const mockToken = 'mock-jwt-token';
      login(mockToken, email);
      navigate('/');
    } catch (err) {
      setError('Login failed. Please try again.');
    } finally {
      setLoading(false);
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
            <Button type="submit" fullWidth loading={loading}>
              Sign in
            </Button>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}
