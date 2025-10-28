import { Title, Text, Container, Paper, Stack, Button, Group, Modal, TextInput, Table, Pagination, Center, Loader } from '@mantine/core';
import { IconPlus } from '@tabler/icons-react';
import { useState } from 'react';
import { usePostApiParties, useGetApiParties } from '../api/generated/parties/parties';
import { useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';

export function Parties() {
  const [opened, setOpened] = useState(false);
  const [partyName, setPartyName] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 25;

  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const createParty = usePostApiParties();
  const { data, isLoading, isError, error } = useGetApiParties({ pageNumber: currentPage, pageSize });

  const handleSubmit = async () => {
    if (!partyName.trim()) return;

    try {
      await createParty.mutateAsync({ data: { name: partyName.trim() } });
      setOpened(false);
      setPartyName('');
      // Refetch parties after creating a new one
      queryClient.invalidateQueries({ queryKey: ['/api/Parties'] });
    } catch (error) {
      console.error('Failed to create party:', error);
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

  return (
    <Container fluid py="xs">
      <Stack gap="lg">
        <Group justify="space-between">
          <Title order={1}>Parties</Title>
          <Button leftSection={<IconPlus size={16} />} onClick={() => setOpened(true)}>
            New Party
          </Button>
        </Group>

        <Paper shadow="sm" p="md">
          {isLoading && (
            <Center p="xl">
              <Loader />
            </Center>
          )}

          {isError && (
            <Text c="red">
              Failed to load parties: {error?.message || 'Unknown error'}
            </Text>
          )}

          {!isLoading && !isError && data && data.totalCount === 0 && (
            <Text c="dimmed">
              No parties yet. Create your first party to get started.
            </Text>
          )}

          {!isLoading && !isError && data && data.totalCount > 0 && (
            <Stack gap="md">
              <Table striped highlightOnHover>
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>Name</Table.Th>
                    <Table.Th>Created</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {data.data.map((party) => (
                    <Table.Tr
                      key={party.id}
                      onClick={() => navigate({ to: '/parties/$partyId', params: { partyId: party.id } })}
                      style={{ cursor: 'pointer' }}
                    >
                      <Table.Td>{party.name}</Table.Td>
                      <Table.Td>{formatDate(party.createdAt)}</Table.Td>
                    </Table.Tr>
                  ))}
                </Table.Tbody>
              </Table>

              {data.totalPages > 1 && (
                <Center>
                  <Pagination
                    total={data.totalPages}
                    value={currentPage}
                    onChange={setCurrentPage}
                  />
                </Center>
              )}
            </Stack>
          )}
        </Paper>
      </Stack>

      <Modal
        opened={opened}
        onClose={() => {
          setOpened(false);
          setPartyName('');
        }}
        title="Create Party"
      >
        <Stack>
          <TextInput
            label="Party name"
            placeholder="Enter party name"
            value={partyName}
            onChange={(e) => setPartyName(e.currentTarget.value)}
            required
            data-autofocus
          />
          <Group justify="flex-end">
            <Button variant="subtle" onClick={() => setOpened(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={!partyName.trim()}
              loading={createParty.isPending}
            >
              Create
            </Button>
          </Group>
        </Stack>
      </Modal>
    </Container>
  );
}
