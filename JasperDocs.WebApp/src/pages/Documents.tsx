import { Title, Text, Container, Paper, Stack, Button, Group, Modal, FileInput, Table, Pagination, Center, Loader } from '@mantine/core';
import { IconPlus, IconUpload } from '@tabler/icons-react';
import { useState } from 'react';
import { usePostApiDocuments, useGetApiDocuments } from '../api/generated/documents/documents';
import { useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';

export function Documents() {
  const [opened, setOpened] = useState(false);
  const [file, setFile] = useState<File | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 25;

  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const createDocument = usePostApiDocuments();
  const { data, isLoading, isError, error } = useGetApiDocuments({ pageNumber: currentPage, pageSize });

  const handleSubmit = async () => {
    if (!file) return;

    try {
      await createDocument.mutateAsync({ data: { File: file } });
      setOpened(false);
      setFile(null);
      // Refetch documents after creating a new one
      queryClient.invalidateQueries({ queryKey: ['/api/Documents'] });
    } catch (error) {
      console.error('Failed to create document:', error);
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
          <Title order={1}>Documents</Title>
          <Button leftSection={<IconPlus size={16} />} onClick={() => setOpened(true)}>
            New Document
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
              Failed to load documents: {error?.message || 'Unknown error'}
            </Text>
          )}

          {!isLoading && !isError && data && data.totalCount === 0 && (
            <Text c="dimmed">
              No documents yet. Create your first document to get started.
            </Text>
          )}

          {!isLoading && !isError && data && data.totalCount > 0 && (
            <Stack gap="md">
              <Table striped highlightOnHover>
                <Table.Thead>
                  <Table.Tr>
                    <Table.Th>Title</Table.Th>
                    <Table.Th>Description</Table.Th>
                    <Table.Th>Created</Table.Th>
                  </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                  {data.data.map((document) => (
                    <Table.Tr
                      key={document.id}
                      onClick={() => navigate({ to: '/documents/$documentId', params: { documentId: document.id } })}
                      style={{ cursor: 'pointer' }}
                    >
                      <Table.Td>{document.title}</Table.Td>
                      <Table.Td>{document.description || '-'}</Table.Td>
                      <Table.Td>{formatDate(document.createdAt)}</Table.Td>
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
          setFile(null);
        }}
        title="Upload Document"
      >
        <Stack>
          <FileInput
            label="Select file"
            placeholder="Choose a file to upload"
            leftSection={<IconUpload size={16} />}
            value={file}
            onChange={setFile}
            required
          />
          <Group justify="flex-end">
            <Button variant="subtle" onClick={() => setOpened(false)}>
              Cancel
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={!file}
              loading={createDocument.isPending}
            >
              Upload
            </Button>
          </Group>
        </Stack>
      </Modal>
    </Container>
  );
}
