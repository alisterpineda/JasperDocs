import { Title, Container, Stack, Center, Loader, Text, TextInput, Group, Button, Paper, Box } from '@mantine/core';
import { useParams } from '@tanstack/react-router';
import { useGetApiPartiesId, usePutApiPartiesId } from '../api/generated/parties/parties';
import { useState } from 'react';
import { notifications } from '@mantine/notifications';

export function PartyDetail() {
  const { partyId } = useParams({ from: '/parties/$partyId' });
  const { data, isLoading, isError, error, refetch } = useGetApiPartiesId(partyId);

  // Edit mode state
  const [isEditMode, setIsEditMode] = useState(false);
  const [editName, setEditName] = useState('');

  // Update mutation
  const { mutateAsync: updateParty, isPending: isUpdating } = usePutApiPartiesId();

  const handleEdit = () => {
    if (data) {
      setEditName(data.name);
      setIsEditMode(true);
    }
  };

  const handleDiscard = () => {
    setIsEditMode(false);
    setEditName('');
  };

  const handleSave = async () => {
    try {
      await updateParty({
        id: partyId,
        data: {
          name: editName,
        },
      });

      notifications.show({
        title: 'Success',
        message: 'Party updated successfully',
        color: 'green',
      });

      setIsEditMode(false);
      refetch();
    } catch (err) {
      notifications.show({
        title: 'Error',
        message: err instanceof Error ? err.message : 'Failed to update party',
        color: 'red',
      });
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (isLoading) {
    return (
      <Container fluid py="xs">
        <Center p="xl">
          <Loader />
        </Center>
      </Container>
    );
  }

  if (isError) {
    return (
      <Container fluid py="xs">
        <Text c="red">Failed to load party: {error?.message || 'Unknown error'}</Text>
      </Container>
    );
  }

  if (!data) {
    return (
      <Container fluid py="xs">
        <Text c="dimmed">Party not found</Text>
      </Container>
    );
  }

  return (
    <Container fluid py="xs">
      <Stack gap="lg">
        <Title order={1}>{data.name}</Title>

        <Paper shadow="sm" p="md">
          <Stack gap="md">
            <Title order={3}>Information</Title>

            {isEditMode ? (
              <Stack gap="md">
                <TextInput
                  label="Name"
                  value={editName}
                  onChange={(e) => setEditName(e.target.value)}
                  required
                  placeholder="Enter party name"
                  error={editName.trim() === '' ? 'Name cannot be empty' : undefined}
                />
                <Group>
                  <Button
                    onClick={handleSave}
                    loading={isUpdating}
                    disabled={editName.trim() === ''}
                  >
                    Save
                  </Button>
                  <Button variant="outline" onClick={handleDiscard} disabled={isUpdating}>
                    Discard
                  </Button>
                </Group>
              </Stack>
            ) : (
              <Stack gap="md">
                <Box>
                  <Text size="sm" fw={500} c="dimmed">Name</Text>
                  <Text size="md">{data.name}</Text>
                </Box>
                <Box>
                  <Text size="sm" fw={500} c="dimmed">Created</Text>
                  <Text size="md">{formatDate(data.createdAt)}</Text>
                </Box>
                <Box>
                  <Text size="sm" fw={500} c="dimmed">Updated</Text>
                  <Text size="md">{formatDate(data.updatedAt)}</Text>
                </Box>
                <Group>
                  <Button onClick={handleEdit}>Edit</Button>
                </Group>
              </Stack>
            )}
          </Stack>
        </Paper>
      </Stack>
    </Container>
  );
}
